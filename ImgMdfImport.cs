using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
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
                var name = Helper.GetName(filename);
                var path = Helper.GetPath(filename);
                Img imgfound;
                lock (_imglock) {
                    if (_nameList.TryGetValue(name, out imgfound)) {
                        if (path.Equals(imgfound.Path, StringComparison.OrdinalIgnoreCase)) {
                            continue;
                        }
                    }
                }

                if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse) {
                    dt = DateTime.Now;
                    var file = filename.Substring(AppConsts.PathCollection.Length);
                    progress?.Report($"{file} (a:{added}/f:{found})...");
                }

                var extension = Helper.GetExtension(filename);
                if (!Helper.GetImageDataFromFile(
                    filename,
                    out var imgdata,
                    out var bitmap,
                    out var checksum,
                    out var needwrite)) {
                    progress?.Report($"Corrupted image: {path}\\{name}{extension}");
                    return;
                }

                lock (_imglock) {
                    if (_checksumList.TryGetValue(checksum, out imgfound)) {
                        found++;
                        Helper.DeleteToRecycleBin(filename);
                        continue;
                    }
                }

                if (!Helper.GetPhashOrbv(imgdata, out ulong phash, out ulong[] orbv)) {
                    progress?.Report($"Cannot get descriptors: {path}\\{name}{extension}");
                    return;
                }

                var id = AllocateId();
                string suggestedname;
                var namelenght = 0;
                lock (_imglock) {
                    do {
                        namelenght++;
                        suggestedname = string.Concat(AppConsts.PrefixName, checksum.Substring(0, namelenght));
                    } while (_nameList.ContainsKey(suggestedname));
                }

                var suggestedfilename = Helper.GetFileName(suggestedname.ToLowerInvariant(), path);
                var lastview = GetMinLastView();
                var img = new Img(
                    id: id,
                    name: suggestedname,
                    path: path,
                    checksum: checksum,
                    generation: 1,
                    lastview: lastview,
                    nextid: id,
                    distance: 256,
                    lastid: -1,
                    lastchange: lastview,
                    phash: phash,
                    orbv: orbv);

                Add(img);
                if (needwrite) {
                    Helper.WriteData(suggestedfilename, imgdata);
                    Helper.DeleteToRecycleBin(filename);
                }
                else {
                    if (!filename.Equals(img.File, StringComparison.OrdinalIgnoreCase)) {
                        File.Move(filename, img.File);
                    }
                }

                FindNext(id, out var lastid, out var lastchange, out var nextid, out var distance);

                if (distance != img.Distance) {
                    img.Distance = distance;
                }

                if (nextid != img.NextId) {
                    img.NextId = nextid;
                }

                if (lastchange != img.LastChange) {
                    img.LastChange = lastchange;
                }

                if (lastid != img.LastId) {
                    img.LastId = lastid;
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

        public void Import(IProgress<string> progress)
        {
            Contract.Requires(progress != null);
            Helper.CleanupDirectories(AppConsts.PathCollection, progress);
            Import(1000000, progress);
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
