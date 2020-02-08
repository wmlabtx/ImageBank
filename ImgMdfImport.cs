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
            var dt = DateTime.Now;

            var directoryInfo = new DirectoryInfo(AppConsts.PathCollection);
            var fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            foreach (var fileInfo in fileInfos) {
                var filename = fileInfo.FullName;
                var name = HelperPath.GetName(filename);
                var path = HelperPath.GetPath(filename);
                if (_nameList.TryGetValue(name, out var imgfound)) {
                    if (path.Equals(imgfound.Path)) {
                        continue;
                    }
                }

                if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse) {
                    dt = DateTime.Now;
                    var file = filename.Substring(AppConsts.PathCollection.Length);
                    progress?.Report($"{file} (a:{added}/f:{found})...");
                }

                var extension = Path.GetExtension(filename);

                if (!HelperImages.GetChecksumFromFile(
                    filename,
                    out var jpgdata,
                    out var checksum)) {
                    progress?.Report($"Corrupted image: {path}\\{name}{extension}");
                    return;

                }

                if (_checksumList.TryGetValue(checksum, out imgfound)) {
                    found++;
                    if (extension.Equals(AppConsts.DatExtension)) {
                        HelperRecycleBin.Delete(filename);
                        continue;
                    }

                    Delete(imgfound.Id);
                }

                if (!HelperImages.GetBitmapFromFile(
                    filename, 
                    out jpgdata, 
                    out var bitmap, 
                    out checksum,
                    out var suggestedname,
                    out var needwrite)) {
                    progress?.Report($"Corrupted image: {path}\\{name}{extension}");
                    return;
                }

                if (_checksumList.TryGetValue(checksum, out imgfound)) {
                     found++;
                    if (extension.Equals(AppConsts.DatExtension)) {
                        HelperRecycleBin.Delete(filename);
                        continue;
                    }

                    Delete(imgfound.Id);
                }

                var lastview = GetMinLastView();
                var id = AllocateId();
                var generation = 0;
                if (extension.Equals(AppConsts.DatExtension)) {
                    generation = 1;
                    name = $"{AppConsts.PrefixMzx}{id:D6}";
                    path = GetSuggestedLegacyPath();
                }

                name = HelperPath.AddChecksum(name, checksum);
                var img = new Img(
                    id: id,
                    name: name,
                    path: path,
                    checksum: checksum,
                    generation: generation,
                    lastview: lastview,
                    nextid: id,
                    match: 0,
                    lastid: -1,
                    lastchange: lastview,
                    descriptors: Array.Empty<uint>());

                Add(img);
                if (needwrite) {
                    WriteJpgData(name, path, jpgdata);
                    HelperRecycleBin.Delete(filename);
                }
                else {
                    if (!filename.Equals(img.File)) {
                        File.Move(filename, img.File);
                    }
                }

                if (_imgList.Count >= AppConsts.MaxImages) {
                    break;
                }

                added++;
                if (added >= maxadd) {
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
