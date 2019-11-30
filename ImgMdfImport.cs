﻿using System;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Import(int maxadd, IProgress<string> progress)
        { 
            AppVars.SuspendEvent.Reset();

            var added = 0;
            var moved = 0;
            var skipped = 0;
            var counter = 0;
            var dt = DateTime.Now;

            var directoryInfo = new DirectoryInfo(AppConsts.PathSource);
            var fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            foreach (var fileInfo in fileInfos)
            {
                counter++;

                if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse)
                {
                    dt = DateTime.Now;
                    if (progress != null)
                    {
                        progress.Report($"{_imgList.Count}: analysing files (a:{added}/m:{moved}/s:{skipped}/{counter})...");
                    }
                }

                var filename = fileInfo.FullName;

                if (!HelperImages.GetJpgFromFile(filename, out var jpgdata))
                {
                    skipped++;
                    continue;
                }

                var name = HelperCrc.GetCrc(jpgdata);
                if (_imgList.TryGetValue(name, out var imgFound))
                {
                    skipped++;
                    HelperRecycleBin.Delete(filename);
                    continue;
                }

                if (!HelperDescriptors.ComputeVector(jpgdata, out var vector))
                {
                    skipped++;
                    continue;
                }

                var id = _imgList.Max(e => e.Value.Id) + 1;
                var lastview = DateTime.Now.AddDays(-1000);
                var lastchecked = lastview;
                var lastchanged = lastchecked;
                var array = HelperEncrypting.Encrypt(jpgdata, name);

                var img = new Img(
                    name,
                    id,
                    0,
                    lastview,
                    lastchecked,
                    lastchanged,
                    name,
                    vector,
                    256);

                Add(img);
                img.WriteData(jpgdata);
                ResetNextName(img);
                added++;
                HelperRecycleBin.Delete(filename);

                if (added >= maxadd)
                {
                    break;
                }
            }

            AppVars.SuspendEvent.Set();
        }

        public static void ProcessDirectory(string startLocation, IProgress<string> progress)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                ProcessDirectory(directory, progress);
                if (Directory.GetFiles(directory).Length != 0 || Directory.GetDirectories(directory).Length != 0)
                    continue;

                progress.Report($"{directory} deleting...");
                try
                {
                    Directory.Delete(directory, false);
                }
                catch (IOException)
                {
                }
            }
        }

        public void Import(IProgress<string> progress)
        {
            Import(1000000, progress);
            ProcessDirectory(AppConsts.PathSource, progress);
            progress.Report(string.Empty);
        }
    }
}
