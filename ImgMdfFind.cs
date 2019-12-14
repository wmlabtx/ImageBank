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

                    var scopevalid = _imgList
                        .Values
                        .Where(e => 
                            e.Descriptors.Rows > 0 && 
                            _imgList.ContainsKey(e.NextName) && 
                            !e.NextName.Equals(e.Name))
                        .ToArray();

                    if (scopevalid.Length == 0)
                    {
                        return;
                    }

                    var scoperobust = scopevalid
                        .OrderByDescending(e => e.LastId)
                        .Take(scopevalid.Length / 10)
                        .ToArray();

                    var imgX = scoperobust
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
