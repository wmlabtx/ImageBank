using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(string nameX, IProgress<string> progress)
        {
            progress.Report(string.Empty);
            while (true)
            {                
                if (string.IsNullOrEmpty(nameX))
                {
                    if (_imgList.Count == 0)
                    {
                        return;
                    }

                    var scope = _imgList
                        .Where(e => _imgList.ContainsKey(e.Value.NextName))
                        .Select(e => e.Value)
                        .ToArray();

                    var mingeneration = scope.Min(e => e.Generation);
                    var scopeming = scope.Where(e => e.Generation == mingeneration);
                    var imgX = mingeneration < 2 ?
                        scopeming.OrderByDescending(e => e.Sim).FirstOrDefault() :
                        scopeming.OrderBy(e => e.LastView).FirstOrDefault();

                    nameX = imgX.Name;
                    AppVars.ImgPanel[0] = GetImgPanel(nameX);
                    if (AppVars.ImgPanel[0] == null)
                    {
                        progress.Report($"{_imgList.Count}: {nameX}: corrupted!");
                        nameX = null;
                        continue;
                    }
                }

                AppVars.ImgPanel[1] = GetImgPanel(AppVars.ImgPanel[0].Img.NextName);
                if (AppVars.ImgPanel[1] == null)
                {
                    var nameY = AppVars.ImgPanel[0].Img.NextName;
                    progress.Report($"{_imgList.Count}: {nameY}: corrupted!");

                    var imgX = AppVars.ImgPanel[0].Img;
                    ResetNextName(imgX);

                    continue;
                }
                
                break;
            }
        }

        /*
        private Img FindInRotation(Img[] scopeX)
        {
            var scope = scopeX;
            var level = 0;
            Img imgX = null;
            while (true)
            {
                var folders = new SortedDictionary<string, DateTime>();
                foreach (var img in scope)
                {
                    var pars = img.Folder.Split('\\');
                    var folder = pars[level];

                    if (folders.ContainsKey(folder))
                    {
                        if (folders[folder] < img.LastView)
                        {
                            folders[folder] = img.LastView;
                        }
                    }
                    else
                    {
                        folders.Add(folder, img.LastView);
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

                var scopenew = new List<Img>();
                foreach (var img in scope)
                {
                    var pars = img.Folder.Split('\\');
                    var folder = pars[level];

                    if (folder.Equals(minfolder))
                    {
                        scopenew.Add(img);
                    }
                }

                level++;
                var nextpars = scopenew[0].Folder.Split('\\');
                if (nextpars.Length <= level)
                {
                    imgX = scopenew.OrderBy(e => e.LastView).FirstOrDefault();
                    break;
                }
                else
                {
                    scope = scopenew.ToArray();
                }
            }

            return imgX;
        }
        */
    }
}
