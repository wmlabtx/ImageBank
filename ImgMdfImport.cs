﻿using System;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Import(int maxadd, IProgress<string> progress)
        {
            var directoryInfo = new DirectoryInfo(AppConsts.PathSource);

            AppVars.SuspendEvent.Reset();

            var added = 0;
            var moved = 0;
            var skipped = 0;
            var counter = 0;
            var dt = DateTime.Now;

            var dirInfos = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories).ToArray();
            foreach (var dirInfo in dirInfos)
            {
                var fileInfos = dirInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
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

                    if (!HelperDescriptors.ComputeDescriptors(jpgdata, out var descriptors))
                    {
                        skipped++;
                        continue;
                    }

                    var lastview = GetMinLastView();
                    var lastchecked = GetMinLastChecked();
                    var lastchanged = lastchecked;
                    var array = HelperEncrypting.Encrypt(jpgdata, name);
                    var crc = HelperCrc.GetCrc(array);
                    var offset = GetSuggestedOffset(array.Length);
                    WriteData(offset, array);

                    var img = new Img(
                        name,
                        0,
                        lastview,
                        lastchecked,
                        lastchanged,
                        descriptors,
                        name,
                        0f,
                        offset,
                        array.Length,
                        crc,
                        string.Empty);

                    Add(img);
                    ResetNextName(img);
                    added++;
                    HelperRecycleBin.Delete(filename);

                    if (added >= maxadd)
                    {
                        break;
                    }
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
