using System;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(string hashX, IProgress<string> progress)
        {
            string hashY;
            progress.Report("Moving forward...");
            if (!GetPairToCompare(ref hashX, out hashY))
            {
                progress.Report("No images to view");
                return;
            }

            progress.Report($"{_imgList.Count}");

            AppVars.ImgPanel[0] = GetImgPanel(hashX);
            if (AppVars.ImgPanel[0] == null)
            {
                Delete(hashX);
                progress.Report($"{hashX} corrupted");
                return;
            }

            AppVars.ImgPanel[1] = GetImgPanel(hashY);
            if (AppVars.ImgPanel[1] == null)
            {
                Delete(hashY);
                progress.Report($"{hashY} corrupted");
                return;
            }

            if (_imgList.TryGetValue(hashX, out var imgX))
            {
                imgX.LastView = DateTime.Now;
            }

            if (_imgList.TryGetValue(hashY, out var imgY))
            {
                imgY.LastView = DateTime.Now;
            }
        }
    }
}
