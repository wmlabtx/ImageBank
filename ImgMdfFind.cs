using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(string nameX, IProgress<string> progress)
        {
            while (true)
            {                
                if (string.IsNullOrEmpty(nameX))
                {
                    if (_imgList.Count == 0)
                    {
                        return;
                    }

                    var imgX = _imgList.Count(e => e.Value.LastView < e.Value.LastUpdated) > 0 ?
                        _imgList.Where(e => e.Value.LastView < e.Value.LastUpdated).OrderByDescending(e => e.Value.Sim).FirstOrDefault().Value :
                        _imgList.OrderBy(e => e.Value.LastView).FirstOrDefault().Value;
/*
                    var imgX =
                        _imgList.Count(e => e.Value.LastView < e.Value.LastUpdated) > 0 ?
                        Find(_imgList.Where(e => e.Value.LastView < e.Value.LastUpdated)) :
                        Find(_imgList);
*/

                    if (imgX == null)
                    {
                        nameX = null;
                        continue;
                    }

                    nameX = imgX.Name;

                    if (AppVars.ImgPanel[0] != null)
                    {
                        AppVars.ImgPanel[0].Dispose();
                    }

                    AppVars.ImgPanel[0] = GetImgPanel(nameX);
                    if (AppVars.ImgPanel[0] == null)
                    {
                        progress.Report($"{_imgList.Count}: {nameX}: corrupted!");
                        nameX = null;
                        continue;
                    }
                }

                var oldname = AppVars.ImgPanel[0].Img.NextName;
                var oldsim = AppVars.ImgPanel[0].Img.Sim;

                if (AppVars.ImgPanel[1] != null)
                {
                    AppVars.ImgPanel[1].Dispose();
                }

                AppVars.ImgPanel[1] = GetImgPanel(AppVars.ImgPanel[0].Img.NextName);
                if (AppVars.ImgPanel[1] == null)
                {
                    var nameY = AppVars.ImgPanel[0].Img.NextName;
                    progress.Report($"{_imgList.Count}: {nameY}: corrupted!");
                    continue;
                }

                var sb = new StringBuilder();
                var countnotvievedyet = _imgList.Count(e => e.Value.LastView < e.Value.LastUpdated);
                sb.Append($"{countnotvievedyet}/{_imgList.Count}: ");
                sb.Append($" {_avgTimes:F2}s");
                progress.Report(sb.ToString());
                break;
            }
        }

        /*
        private Img Find(IEnumerable<KeyValuePair<string, Img>> scopeX)
        {
            var folders = new SortedDictionary<string, DateTime>();
            foreach (var img in scopeX)
            {
                if (HelperPath.IsLegacy(img.Value.Folder) || DateTime.Now.Subtract(img.Value.LastView).TotalHours < 1.0)
                {
                    continue;
                }

                if (folders.ContainsKey(img.Value.Folder))
                {
                    if (folders[img.Value.Folder] < img.Value.LastView)
                    {
                        folders[img.Value.Folder] = img.Value.LastView;
                    }
                }
                else
                {
                    folders.Add(img.Value.Folder, img.Value.LastView);
                }
            }

            var minfolder = AppConsts.FolderLegacy;
            var minlastview = DateTime.Now;
            foreach (var folder in folders.Keys)
            {
                if (folders[folder] < minlastview)
                {
                    minfolder = folder;
                    minlastview = folders[folder];
                }
            }

            var scope = HelperPath.IsLegacy(minfolder) ?
                _imgList.Where(e => HelperPath.IsLegacy(e.Value.Folder)) :
                _imgList.Where(e => e.Value.Folder.Equals(minfolder));

            var imgX = scope.Count(e => e.Value.LastView < e.Value.LastUpdated) > 0 ?
                scope.Where(e => e.Value.LastView < e.Value.LastUpdated).OrderByDescending(e => e.Value.Sim).FirstOrDefault().Value :
                scope.OrderBy(e => e.Value.LastView).FirstOrDefault().Value;

            return imgX;
        }
        */
    }
}
