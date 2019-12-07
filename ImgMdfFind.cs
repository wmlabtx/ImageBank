using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private bool _flag = false;

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

                    Img imgX;

                    var scope1 = _imgList
                        .Where(e => e.Value.Id > 0 && _imgList.ContainsKey(e.Value.NextName))
                        .Select(e => e.Value)
                        .ToArray();

                    if (scope1.Length == 0)
                    {
                        return;
                    }

                    var scope2 = scope1
                    .Where(e => e.LastView < e.LastChanged)
                    .ToArray();

                    if (scope2.Length > 0 && _flag)
                    {
                        imgX = scope2
                            .OrderBy(e => e.Distance)
                            .FirstOrDefault();

                        if (imgX.Distance >= AppConsts.MinHammingDistance)
                        {
                            imgX = scope2
                                .OrderByDescending(e => e.Sim)
                                .FirstOrDefault();
                        }
                    }
                    else
                    {
                        imgX = scope1
                            .OrderBy(e => e.LastView)
                            .FirstOrDefault();
                    }

                    _flag = !_flag;

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
