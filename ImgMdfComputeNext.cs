using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Compute(BackgroundWorker backgroundworker)
        {
            Contract.Requires(backgroundworker != null);

            AppVars.SuspendEvent.WaitOne(Timeout.Infinite);

            lock (_imglock) {
                if (_imgList.Count == 0) {
                    backgroundworker.ReportProgress(0, "no images");
                    return;
                }
            }

            var idX = GetNextToCheck();
            if (idX < 0) {
                backgroundworker.ReportProgress(0, "idle");
                return;
            }

            Img imgX;
            lock (_imglock) {
                if (!_imgList.TryGetValue(idX, out imgX)) {
                    backgroundworker.ReportProgress(0, $"error getting {idX}");
                    return;
                }
            }

            FindNext(idX, out var lastid, out var lastchange, out var nextid, out var match);

            var sb = new StringBuilder();
            sb.Append($"i{imgX.Id}: ");
            if (match != imgX.Match) {
                sb.Append($"{imgX.Match} ");
                sb.Append($"{char.ConvertFromUtf32(match > imgX.Match ? 0x2192 : 0x2193)} ");
                sb.Append($"{match}");
                imgX.Match = match;
                if (nextid != imgX.NextId) {
                    imgX.NextId = nextid;
                }
            }
            else {
                if (nextid != imgX.NextId) {
                    sb.Append($"i{imgX.NextId} ");
                    sb.Append($"{char.ConvertFromUtf32(0x2192)} ");
                    sb.Append($"i{nextid}");
                    imgX.NextId = nextid;
                }
                else {

                }
            }

            if (lastchange != imgX.LastChange) {
                imgX.LastChange = lastchange;
            }
            
            if (lastid != imgX.LastId) {
                imgX.LastId = lastid;
            }

            var message = sb.ToString();
            backgroundworker.ReportProgress(0, message);
        }
    }
}