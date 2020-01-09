using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void FastFind()
        {
            var count = _imgList.Count;
            if (count == 0)
            {
                return;
            }

            var hashX = GetNextToCheck();
            if (string.IsNullOrEmpty(hashX))
            {
                return;
            }

            if (!_imgList.TryGetValue(hashX, out var imgX))
            {
                return;
            }

            FindNextHash(hashX, out var lastid, out var lastchange, out var nexthash, out var sim);
            if (lastid != imgX.LastId)
            {
                imgX.LastId = lastid;
            }

            if (lastchange != imgX.LastChange)
            {
                imgX.LastChange = lastchange;
            }

            if (!nexthash.Equals(imgX.NextHash) || Math.Abs(imgX.Sim - sim) > 0.0001)
            {
                imgX.NextHash = nexthash;
                imgX.Sim = sim;
            }
        }

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

            var toview = _imgList.Values.Count(e => e.LastView < e.LastChange);

            var sb = new StringBuilder();
            if (toview > 0)
            {
                sb.Append($"{toview}/");
            }

            sb.Append($"{count}");

            var maxid = _imgList.Max(e => e.Value.Id) + 1;
            var avgid = _imgList.Values.Sum(e => e.LastId) * 100f / (maxid * _imgList.Count);
            sb.Append($" ({avgid:F2}%)");
            
            sb.Append($": ");

            if (lastid == imgX.LastId)
            {
                sb.Append($"[{imgX.LastId}] ");
            }
            else
            {
                sb.Append($"[{imgX.LastId}+{lastid - imgX.LastId}] ");
                imgX.LastId = lastid;
            }
            
            sb.Append($"{imgX.Sim:F4} ");
            if (!nexthash.Equals(imgX.NextHash) || Math.Abs(imgX.Sim - sim) > 0.0001)
            {
                imgX.NextHash = nexthash;
                imgX.Sim = sim;
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F4}");
            }

            var message = sb.ToString();
            backgroundworker.ReportProgress(0, message);

            if (avgid > 50f)
            {
                Import(10, null);
            }
        }
    }
}