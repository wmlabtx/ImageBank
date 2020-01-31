using System;
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
            var found = 0;
            var skipped = 0;
            var dt = DateTime.Now;

            var directoryInfo = new DirectoryInfo(AppConsts.PathCollection);
            var fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            foreach (var fileInfo in fileInfos)
            {
                var filename = fileInfo.FullName;
                var name = HelperPath.GetName(filename);
                var extension = Path.GetExtension(filename);
                if (_imgList.ContainsKey(name))
                {
                    continue;
                }

                if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse)
                {
                    dt = DateTime.Now;
                    var file = filename.Substring(AppConsts.PathCollection.Length);
                    var count = _imgList.Count;
                    var freshcount = GetFreshCount();
                    progress?.Report($"{freshcount:X}/{count:X}: {file} (a:{added}/f:{found}/s:{skipped})...");
                }

                if (!HelperImages.GetBitmapFromFile(filename, out var jpgdata, out var bitmap, out var needwrite))
                {
                    if (File.Exists(filename))
                    {                        
                        var corruptedfile = $"{AppConsts.PathRecycle}{name}{extension}";
                        File.Move(filename, corruptedfile);
                        skipped++;
                    }
                    
                    continue;
                }

                var hash = HelperHash.ComputeName(jpgdata);
                if (_imgList.ContainsKey(hash))
                {
                    found++;
                    HelperRecycleBin.Delete(filename);
                    continue;
                }

                if (!HelperDescriptors.ComputeHashes(jpgdata, out var descriptors))
                {
                    var corruptedfile = $"{AppConsts.PathRecycle}{name}{AppConsts.JpgExtension}";
                    File.WriteAllBytes(corruptedfile, jpgdata);
                    HelperRecycleBin.Delete(filename);
                    skipped++;
                    continue;
                }

                var lastview = GetMinLastView();
                var id = AllocateId();                
                var generation = extension.Equals(AppConsts.DatExtension) ? 1 : 0;
                var img = new Img(
                    hash: hash,
                    id: id,
                    generation: generation,
                    lastview: lastview,
                    nexthash: hash,
                    sim: 0f,
                    lastid: -1,
                    lastchange: DateTime.Now,
                    descriptors: descriptors);

                if (!img.File.Equals(filename))
                {
                    if (needwrite)
                    {
                        WriteJpgData(hash, img.Directory, jpgdata);
                        HelperRecycleBin.Delete(filename);
                    }
                    else
                    {
                        File.Move(filename, img.File);
                    }
                }

                Add(img);
                if (_imgList.Count >= AppConsts.MaxImages)
                {
                    break;
                }

                added++;
                if (added >= maxadd)
                {
                    break;
                }
            }

            AppVars.SuspendEvent.Set();
        }

        private static void ProcessDirectory(string startLocation, IProgress<string> progress)
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
            Import(AppConsts.MaxImportImages, progress);
            ProcessDirectory(AppConsts.PathCollection, progress);
            progress.Report(string.Empty);
        }

        public void Export(IProgress<string> progress)
        {
            /*
            var name = AppVars.ImgPanel[0].Img.Name;
            var jpgdata = AppVars.ImgPanel[0].Img.GetData();
            var filename = $"{AppConsts.PathSource}{name}.jpeg";
            File.WriteAllBytes(filename, jpgdata);
            progress.Report($"{filename} exported!");
            */
        }
    }
}
