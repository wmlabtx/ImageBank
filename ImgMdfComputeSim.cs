using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void ComputeSim(BackgroundWorker backgroundworker)
        {
            AppVars.SuspendEvent.WaitOne(Timeout.Infinite);

            var count = _imgList.Count();
            if (count == 0)
            {
                backgroundworker.ReportProgress(0, string.Empty);
                return;
            }

            var hashX = GetNextToCheck();
            if (!_imgList.TryGetValue(hashX, out var imgX))
            {
                backgroundworker.ReportProgress(0, string.Empty);
                return;
            }

            imgX.LastCheck = DateTime.Now;

            //backgroundworker.ReportProgress(0, "Flanning...");
            var images = GetImagesFromFolder(imgX.Folder);
            if (!_flannList.ContainsKey(imgX.Folder))
            {
                _flannList.TryAdd(imgX.Folder, new Flann(images));
            }

            var nexthash = _flannList[imgX.Folder].Find(imgX, images);
            if (string.IsNullOrEmpty(nexthash))
            {
                backgroundworker.ReportProgress(0, string.Empty);
                return;
            }

            var sb = new StringBuilder();
            sb.Append($"{count}: ");
            sb.Append($"{imgX.Hash} ");
            if (!nexthash.Equals(imgX.NextHash))
            {
                imgX.NextHash = nexthash;
            }

            var message = sb.ToString();
            backgroundworker.ReportProgress(0, message);
        }
    }
}