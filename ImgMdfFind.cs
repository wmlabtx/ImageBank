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
                        .Where(e => e.Value.Id > 0 && !e.Value.Name.Equals(e.Value.NextName) && _imgList.ContainsKey(e.Value.NextName))
                        .Select(e => e.Value)
                        .ToArray();

                    if (scope.Length == 0)
                    {
                        return;
                    }

                    var scopesim = scope
                        .Where(e => e.LastView < e.LastChanged && e.Sim > AppConsts.MinSim)
                        .ToArray();

                    var imgX = scopesim.Length > 0 ?
                        scopesim
                        .OrderByDescending(e => e.Sim)
                        .FirstOrDefault() :
                        scope
                        .OrderBy(e => e.LastView)
                        .FirstOrDefault();

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
    }
}
