using System;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(string hashX, IProgress<string> progress)
        {
            progress.Report("Moving forward...");
            while (true)
            {
                if (string.IsNullOrEmpty(hashX))
                {
                    hashX = GetNextToView();
                    if (string.IsNullOrEmpty(hashX))
                    {
                        progress.Report("No available images to view");
                        return;
                    }
                }

                AppVars.ImgPanel[0] = GetImgPanel(hashX);
                if (AppVars.ImgPanel[0] == null)
                {
                    Delete(hashX);
                    progress.Report("{hashX} corrupted");
                    hashX = null;
                    continue;
                }

                var hashY = GetNextHash(hashX);
                if (string.IsNullOrEmpty(hashY))
                {
                    hashX = null;
                    continue;
                }

                AppVars.ImgPanel[1] = GetImgPanel(hashY);
                if (AppVars.ImgPanel[1] == null)
                {
                    Delete(hashY);
                    progress.Report("{hashY} corrupted");
                    hashX = null;
                    continue;
                }

                progress.Report(string.Empty);
                return;
            }
        }
    }
}
