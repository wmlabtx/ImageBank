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

                    var scopeok = _imgList
                        .Where(e =>
                            e.Value.Descriptors.Size.Height > 0 &&
                            _imgList.ContainsKey(e.Value.NextName) &&
                            !e.Value.Name.Equals(e.Value.NextName))
                        .Select(e => e.Value)
                        .ToArray();

                    if (scopeok.Length == 0)
                    {
                        return;
                    }

                    var scopeold = scopeok
                        .Where(e => DateTime.Now.Subtract(e.LastView).TotalDays > 1000)
                        .ToArray();

                    if (scopeold.Length > 0)
                    {
                        scopeok = scopeold;
                    }

                    var scopenew = scopeok
                        .Where(e => e.LastView < e.LastChanged)
                        .ToArray();

                    if (scopenew.Length > 0)
                    {
                        scopeok = scopenew;
                    }

                    var imgX = scopeok
                        .OrderByDescending(e => e.Sim)
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
