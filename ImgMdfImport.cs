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
            var moved = 0;
            var skipped = 0;
            var dt = DateTime.Now;

            var directoryInfo = new DirectoryInfo(AppConsts.PathCollection);
            var fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            foreach (var fileInfo in fileInfos)
            {
                if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse)
                {
                    dt = DateTime.Now;
                    if (progress != null)
                    {
                        progress.Report($"{_imgList.Count}: analysing files (a:{added}/m:{moved}/s:{skipped})...");
                    }
                }

                var filename = fileInfo.FullName;
                var name = HelperPath.GetName(filename);
                if (_imgList.TryGetValue(name, out var imgfound))
                {
                    continue;
                }

                if (!HelperImages.GetBitmapFromFile(filename, out var jpgdata, out var bitmap, out var needwrite))
                {
                    skipped++;
                    continue;
                }
                
                if (!HelperDescriptors.ComputeDescriptors(jpgdata, out var descriptors))
                {
                    skipped++;
                    continue;
                }

                var hash = HelperHash.CalculateHash(jpgdata);
                var id = _imgList.Count == 0 ? 1 : _imgList.Max(e => e.Value.Id) + 1;
                var ratio = (float)bitmap.Width / bitmap.Height;
                var lastview = GetMinLastView();
                var img = new Img(
                    hash: hash,
                    id : id,
                    ratio: ratio,
                    generation: 0,
                    stars: 0,
                    lastview: lastview,
                    nexthash: hash,
                    sim: 0f,
                    lastid: 0,
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

                added++;
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
