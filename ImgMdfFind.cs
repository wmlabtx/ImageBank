using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(string nameX, IProgress<string> progress)
        {
            progress.Report("Moving forward...");
            string message;
            while (true)
            {
                if (string.IsNullOrEmpty(nameX))
                {
                    nameX = GetNextToView();
                    if (string.IsNullOrEmpty(nameX))
                    {
                        progress.Report("No available images to view");
                        return;
                    }
                }

                AppVars.ImgPanel[0] = GetImgPanel(nameX);
                if (AppVars.ImgPanel[0] == null)
                {
                    nameX = null;
                    continue;
                }

                if (!FindNextName(nameX, out var nameY, out message))
                {
                    Delete(nameX);
                    nameX = null;
                    continue;
                }

                AppVars.ImgPanel[1] = GetImgPanel(nameY);
                if (AppVars.ImgPanel[1] == null)
                {
                    Delete(nameY);
                    nameX = null;
                    continue;
                }

                break;
            }

            progress.Report(message);
        }
    }
}
