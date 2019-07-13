using System;
using System.Drawing;
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
                    progress.Report($"{_imgList.Count}: analysing files (a:{added}/m:{moved}/s:{skipped}/{counter})...");
                }

                var ofilename = fileInfo.FullName;
                var oname = HelperPath.GetName(ofilename);
                var oextension = Path.GetExtension(ofilename);
                if (oextension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_imgList.TryGetValue(oname, out var imgJpgFound))
                    {
                        if (imgJpgFound.FileName.Equals(ofilename, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                    }
                }

                var ofolder = HelperPath.GetFolder(ofilename);
                if (oextension.Equals(AppConsts.DatExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    var encrypteddata = File.ReadAllBytes(ofilename);
                    var decrypteddata = HelperEncrypting.Decrypt(encrypteddata, oname);
                    if (decrypteddata == null || decrypteddata.Length == 0)
                    {
                        skipped++;
                        continue;
                    }

                    var jpgfilename = Path.ChangeExtension(ofilename, AppConsts.JpgExtension);
                    if (File.Exists(jpgfilename))
                    {
                        HelperRecycleBin.Delete(ofilename);
                        skipped++;
                        continue;
                    }

                    File.WriteAllBytes(jpgfilename, decrypteddata);
                    HelperRecycleBin.Delete(ofilename);
                    ofilename = jpgfilename;
                    oextension = AppConsts.JpgExtension;
                }

                if (oextension.Equals(AppConsts.JpegExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    var jpgfilename = Path.ChangeExtension(ofilename, AppConsts.JpgExtension);
                    if (File.Exists(jpgfilename))
                    {
                        skipped++;
                        continue;
                    }

                    File.Move(ofilename, jpgfilename);
                    ofilename = jpgfilename;
                    oextension = AppConsts.JpgExtension;
                }

                if (
                    !oextension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase) &&
                    !oextension.Equals(AppConsts.PngExtension, StringComparison.InvariantCultureIgnoreCase) &&
                    !oextension.Equals(AppConsts.BmpExtension, StringComparison.InvariantCultureIgnoreCase) &&
                    !oextension.Equals(AppConsts.WebpExtension, StringComparison.InvariantCultureIgnoreCase)
                   )
                {
                    skipped++;
                    continue;
                }


                if (!HelperImages.GetDataAndBitmap(ofilename, out byte[] data, out Bitmap bitmap))
                {
                    skipped++;
                    continue;
                }

                var lastview = DateTime.MinValue;
                var lastchecked = DateTime.MinValue;
                var lastupdated = DateTime.MinValue;
                var name = HelperCrc.GetCrc(data);
                if (_imgList.TryGetValue(name, out var imgFound))
                {
                    if (!File.Exists(imgFound.FileName))
                    {
                        imgFound.Folder = ofolder;
                        SqlUpdateFolder(name, imgFound.Folder);
                        moved++;
                        continue;
                    }

                    if (HelperPath.IsLegacy(imgFound.Folder))
                    {
                        lastview = imgFound.LastView;
                        lastchecked = imgFound.LastChecked;
                        lastupdated = imgFound.LastUpdated;
                        DeleteImgAndFile(name);
                    }
                    else
                    {
                        if (HelperPath.IsLegacy(ofolder) || imgFound.Folder.Equals(ofolder))
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

                var filename = HelperPath.GetFileName(name, ofolder);
                if (!filename.Equals(ofilename))
                {
                    File.Move(ofilename, filename);
                    //File.WriteAllBytes(filename, data);
                    //HelperRecycleBin.Delete(ofilename);
                }

                if (lastview == DateTime.MinValue)
                {
                    lastview = (_imgList.Count > 0 ? _imgList.Min(e => e.Value.LastView) : DateTime.Now).AddMinutes(-1);
                }

                if (lastchecked == DateTime.MinValue)
                {
                    lastchecked = (_imgList.Count > 0 ? _imgList.Min(e => e.Value.LastChecked) : DateTime.Now).AddMinutes(-1);
                }

                if (lastupdated == DateTime.MinValue)
                {
                    lastupdated = lastchecked;
                }

                var img = new Img(
                    name,
                    ofolder,
                    lastview,
                    lastchecked,
                    lastupdated,
                    descriptors,
                    name,
                    0f);

                Add(img);
                added++;
                if (added >= maxadd)
                {
                    break;
                }
            }
        }

        public void Import(IProgress<string> progress)
        {
            Import(1000, progress);
        }
    }
}
