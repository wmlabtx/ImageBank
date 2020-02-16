using System;
using System.Diagnostics.Contracts;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void SetImgPanles(IProgress<string> progress, int idX, int idY)
        {
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

        public void Find(IProgress<string> progress)
        {
            Contract.Requires(progress != null);
            if (!GetPairToCompare(out var idX, out var idY)) {
                progress.Report("No images to view");
                return;
            }

            SetImgPanles(progress, idX, idY);
        }

        public void Find(int idX, IProgress<string> progress)
        {
            Contract.Requires(progress != null);
            if (!_imgList.TryGetValue(idX, out var imgX)) {
                progress.Report($"error getting {idX}");
                return;
            }

            FindNext(idX, out var lastid, out var lastchange, out var nextid, out var match);

            if (lastchange != imgX.LastChange) {
                imgX.LastChange = lastchange;
            }

            if (lastid != imgX.LastId) {
                imgX.LastId = lastid;
            }

            if (nextid != imgX.NextId) {
                imgX.NextId = nextid;
            }

            if (match != imgX.Match) {
                imgX.Match = match;
            }

            var idY = nextid;
            SetImgPanles(progress, idX, idY);
        }
    }
}
