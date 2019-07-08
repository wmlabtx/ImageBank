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

                    var folders = new SortedDictionary<string, DateTime>();
                    foreach (var img in _imgList)
                    {
                        if 
                    }



                    var imgX = _imgList.Values.OrderBy(e => e.LastView).FirstOrDefault();
                    if (imgX == null)
                    {
                        return;
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
    }
}
