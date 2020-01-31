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
            if (string.IsNullOrEmpty(hashX))
            {
                ///if (_imgList.Count < AppConsts.MaxImages - AppConsts.MaxImportImages) {
                ///    Import(0x1000000, AppVars.BackgroundProgress);
                ///    ProcessDirectory(AppConsts.PathCollection, AppVars.BackgroundProgress);
                ///}

                backgroundworker.ReportProgress(0, string.Empty);
                return;
            }

            if (!_imgList.TryGetValue(hashX, out var imgX))
            {
                backgroundworker.ReportProgress(0, string.Empty);
                return;
            }

            FindNextHash(hashX, out var lastid, out var lastchange, out var nexthash, out var sim);

            if (lastchange != imgX.LastChange)
            {
                imgX.LastChange = lastchange;
            }

            var sb = new StringBuilder();
            var freshcount = GetFreshCount();
            sb.Append($"{freshcount:X}/{count:X}: ");
            if (lastid == imgX.LastId)
            {
                sb.Append($"[{imgX.LastId}] ");
            }
            else
            {
                sb.Append($"[+{lastid - imgX.LastId}] ");
                imgX.LastId = lastid;
            }

            if (Math.Abs(imgX.Sim - sim) > 0.0001)
            {
                sb.Append($"{imgX.Sim:F1} ");
                sb.Append($"{char.ConvertFromUtf32(sim > imgX.Sim? 0x2192 : 0x2193)} ");
                sb.Append($"{sim:F1}");
                imgX.Sim = sim;
                if (!nexthash.Equals(imgX.NextHash))
                {
                    imgX.NextHash = nexthash;
                }
            }
            else
            {
                if (!nexthash.Equals(imgX.NextHash))
                {
                    sb.Append($"{imgX.NextHash} ");
                    sb.Append($"{char.ConvertFromUtf32(0x2192)} ");
                    sb.Append($"{nexthash}");
                    imgX.NextHash = nexthash;
                }
            }

            var message = sb.ToString();
            backgroundworker.ReportProgress(0, message);
        }
    }
}