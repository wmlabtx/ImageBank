using System;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(IProgress<string> progress)
        {
            progress.Report("Moving forward...");
            if (!GetPairToCompare(out var idX, out var idY)) {
                progress.Report("No images to view");
                return;
            }

            progress.Report(GetPrompt());

            AppVars.ImgPanel[0] = GetImgPanel(idX);
            if (AppVars.ImgPanel[0] == null) {
                Delete(idX);
                progress.Report($"{idX} corrupted, deleted");
                return;
            }

            AppVars.ImgPanel[1] = GetImgPanel(idY);
            if (AppVars.ImgPanel[1] == null) {
                Delete(idY);
                progress.Report($"{idY} corrupted, deleted");
                return;
            }
        }
    }
}
