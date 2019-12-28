using System;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private int Import(int maxadd, IProgress<string> progress)
        {
            AppVars.SuspendEvent.Reset();

            progress.Report($"Loading collection...");
            var added = 0;
            var moved = 0;
            var skipped = 0;
            var counter = 0;
            var dt = DateTime.Now;
            var directoryInfo = new DirectoryInfo(AppConsts.PathCollection);
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
                var folder = HelperPath.GetFolder(filename);
                var name = Path.GetFileNameWithoutExtension(filename);
                if (_imgList.TryGetValue(name, out var imgfound))
                {
                    if (imgfound.Folder.Equals(folder))
                    {
                        continue;
                    }

                    if (File.Exists(imgfound.DataFile))
                    {
                        skipped++;
                        HelperRecycleBin.Delete(filename);
                        continue;
                    }
                    else
                    {
                        moved++;
                        imgfound.Folder = folder;
                        imgfound.Id = GetNextId();
                        ResetNextName(imgfound);
                        ResetRefers(imgfound.Name);
                        continue;
                    }
                }

                if (!HelperImages.GetBitmapFromFile(filename, out var jpgdata, out var bitmap, out var needwrite))
                {
                    skipped++;
                    continue;
                }

                name = HelperHash.CalculateHash(jpgdata);
                if (!HelperOrbs.ComputeOrbs(jpgdata, out var orbs))
                {
                    skipped++;
                    continue;
                }

                var lastview = GetMinLastView();
                var lastcheck = GetMinLastCheck();
                var lastchange = lastcheck;
                var id = GetNextId();
                var orbsslot = _availableOrbsSlots.FirstOrDefault().Key;
                var orbslenght = orbs.Width * orbs.Height;

                var img = new Img(
                    name: name,
                    folder: folder,
                    lastview: lastview,
                    lastcheck: lastcheck,
                    lastchange: lastchange,
                    nextname: name,
                    distance: 256f,
                    id: id,
                    lastid: 0,
                    orbsslot: orbsslot,
                    orbslength: orbslenght);

                img.Orbs = orbs;
                if (!_imgList.TryAdd(name, img))
                {
                    return 0;
                }

                _availableOrbsSlots.TryRemove(orbsslot, out _);
                SqlAdd(img);

                var filenamejpg = HelperPath.GetFileName(name, folder);
                if (!filename.Equals(filenamejpg))
                {
                    if (needwrite)
                    {
                        WriteFileData(name, folder, jpgdata);
                        HelperRecycleBin.Delete(filename);
                    }
                    else
                    {
                        File.Move(filename, filenamejpg);
                    }
                }

                added++;
                if (added >= maxadd)
                {
                    break;
                }
            }

            AppVars.SuspendEvent.Set();

            return added;
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
            Import(AppConsts.MaxImport, progress);
            var legacy = Path.Combine(AppConsts.PathCollection, AppConsts.FolderLegacy);
            ProcessDirectory(legacy, progress);
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
