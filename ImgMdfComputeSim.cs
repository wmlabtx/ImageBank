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
            if (count == 0) {
                backgroundworker.ReportProgress(0, "no images");
                return;
            }

            Img imgX;
            var idX = GetNextToComputeHashes();
            if (idX >= 0) {
                if (!_imgList.TryGetValue(idX, out imgX)) {
                    backgroundworker.ReportProgress(0, $"error getting {idX}");
                    return;
                }

                var jpgdata = ReadJpgData(imgX.Name, imgX.Path);
                if (!HelperDescriptors.ComputeHashes(jpgdata, out var descriptors)) {
                    backgroundworker.ReportProgress(0, $"Unable to compute hashes: {imgX.Path}\\{imgX.Name}{AppConsts.JpgExtension}");
                    return;
                }

                imgX.SetDescriptors(descriptors);
                imgX.LastId = -1;
            }
            else {
                idX = GetNextToCheck();
                if (idX < 0) {
                    backgroundworker.ReportProgress(0, "idle");
                    return;
                }

                if (!_imgList.TryGetValue(idX, out imgX)) {
                    backgroundworker.ReportProgress(0, $"error getting {idX}");
                    return;
                }
            }

            FindNext(idX, out var lastid, out var lastchange, out var nextid, out var match, out var updated);

            if (lastchange != imgX.LastChange) {
                imgX.LastChange = lastchange;
            }

            var sb = new StringBuilder();
            sb.Append(GetPrompt());
            if (lastid == imgX.LastId) {
                sb.Append($"[{imgX.LastId}] ");
            }
            else {
                sb.Append($"[+{lastid - imgX.LastId}] ");
                imgX.LastId = lastid;
            }

            if (updated > 0) {
                sb.Append($"U+{updated} ");
            }

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
                    sb.Append($"{imgX.NextId} ");
                    sb.Append($"{char.ConvertFromUtf32(0x2192)} ");
                    sb.Append($"{nextid}");
                    imgX.NextId = nextid;
                }
            }

            var message = sb.ToString();
            backgroundworker.ReportProgress(0, message);
        }
    }
}