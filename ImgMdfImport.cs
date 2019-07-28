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

            AppVars.SuspendEvent.Reset();

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
                var ofolder = HelperPath.GetFolder(ofilename);
                var oextension = Path.GetExtension(ofilename);

                if (oextension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_imgList.TryGetValue(oname, out var imgJpgFound))
                    {
                        if (imgJpgFound.FileName.Equals(ofilename, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                        else
                        {
                            if (HelperPath.IsLegacy(imgJpgFound.Folder) && !HelperPath.IsLegacy(ofolder))
                            {
                                HelperRecycleBin.Delete(imgJpgFound.FileName);
                                imgJpgFound.Folder = ofolder;
                                HelperSql.UpdateFolder(imgJpgFound);
                                ResetNextName(imgJpgFound);
                                moved++;
                                continue;
                            }

                            if (HelperPath.IsLegacy(ofolder))
                            {
                                HelperRecycleBin.Delete(ofilename);
                            }

                            skipped++;
                            continue;
                        }
                    }
                }
                
                if (!HelperImages.GetJpgFromFile(ofilename, out var jpgdata))
                {
                    skipped++;
                    continue;
                }

                var lastview = GetMinLastView();
                var lastchecked = GetMinLastChecked();
                var name = HelperCrc.GetCrc(jpgdata);

                if (!HelperDescriptors.ComputeDescriptors(jpgdata, out var descriptors))
                {
                    skipped++;
                    continue;
                }

                if (_imgList.Count >= AppConsts.MaxImages)
                {
                    break;
                }

                var img = new Img(
                    oname,
                    ofolder,
                    2,
                    lastview,                    
                    lastchecked,
                    descriptors,
                    oname,
                    0f);

                Add(img);               
                added++;

                var filename = img.FileName;
                if (!filename.Equals(ofilename))
                {
                    HelperSql.SetData(img, jpgdata);
                    HelperRecycleBin.Delete(ofilename);
                }

                if (added >= maxadd)
                {
                    break;
                }
            }

            AppVars.SuspendEvent.Set();
        }

        public void Import(IProgress<string> progress)
        {
            Import(100000, progress);
        }
    }
}
