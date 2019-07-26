using System;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Import(int maxadd, IProgress<string> progress)
        {
            var directoryInfo = new DirectoryInfo(AppConsts.PathRoot);
            var fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();

            var added = 0;
            var moved = 0;
            var skipped = 0;
            var counter = 0;
            var dt = DateTime.Now;
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

                var ofilename = fileInfo.FullName;
                var oname = HelperPath.GetName(ofilename);
                var oextension = Path.GetExtension(ofilename);
                var ofolder = HelperPath.GetFolder(ofilename);
                if (!HelperImages.GetJpgFromFile(ofilename, out var jpgdata))
                {
                    skipped++;
                    continue;
                }

                var lastview = DateTime.MinValue;
                var lastchecked = DateTime.MinValue;
                var name = HelperCrc.GetCrc(jpgdata);
                if (_imgList.TryGetValue(name, out var imgFound))
                {
                    if (HelperPath.IsLegacy(imgFound.Folder) && !HelperPath.IsLegacy(ofolder))
                    {
                        lastview = imgFound.LastView;
                        lastchecked = imgFound.LastChecked;
                        DeleteImg(name);
                    }
                    else
                    {
                        if (HelperPath.IsLegacy(ofolder))
                        {
                            HelperRecycleBin.Delete(ofilename);
                        }                       

                        skipped++;
                        continue;
                    }
                }

                if (!HelperDescriptors.ComputeDescriptors(data, out var descriptors))
                {
                    skipped++;
                    continue;
                }

                if (_imgList.Count >= AppConsts.MaxImages)
                {
                    break;
                }

                if (lastview == DateTime.MinValue)
                {
                    lastview = GetMinLastView();
                }

                if (lastchecked == DateTime.MinValue)
                {
                    lastchecked = GetMinLastChecked();
                }

                var img = new Img(
                    name,
                    ofolder,
                    lastview,
                    lastchecked,
                    descriptors,
                    name,
                    0f);

                Add(img, data);               
                added++;

                HelperRecycleBin.Delete(ofilename);

                if (added >= maxadd)
                {
                    break;
                }
            }
        }

        public void Import(IProgress<string> progress)
        {
            Import(100000, progress);
        }
    }
}
