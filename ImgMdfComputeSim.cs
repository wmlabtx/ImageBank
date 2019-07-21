using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public string ComputeSim()
        {
            AppVars.SuspendEvent.WaitOne(Timeout.Infinite);

            if (_imgList.Count == 0)
            {
                return null;
            }

            var scopewrong = _imgList
                .Where(e => !_imgList.ContainsKey(e.Value.NextName))
                .Select(e => e.Value)
                .ToArray();

            var imgX = scopewrong.Length > 0 ?
                scopewrong.First() :
                _imgList.Values.OrderBy(e => e.LastChecked).FirstOrDefault();

            if (imgX == null)
            {
                return null;
            }

            if (!File.Exists(imgX.FileName))
            {
                DeleteImg(imgX);
                return null;
            }

            var oldname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var oldsim = imgX.Sim;

            var dt = DateTime.Now;
            var updates = FindNextName(imgX);

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null)
            {
                return null;
            }

            if (!File.Exists(imgY.FileName))
            {
                DeleteImg(imgY);
                return null;
            }

            _findTimes.Enqueue(DateTime.Now.Subtract(dt).TotalSeconds);
            if (_findTimes.Count > 100)
            {
                _findTimes.Dequeue();
            }

            if (_findTimes.Count > 0)
            {
                _avgTimes = _findTimes.Average();
            }

            var sb = new StringBuilder();
            var countnotvievedyet = _imgList.Count(e => e.Value.LastView < e.Value.LastUpdated);
            sb.Append($"{countnotvievedyet}/{_imgList.Count}: ");
            sb.Append($" {_avgTimes:F2}s");
            ((IProgress<string>)AppVars.Progress).Report(sb.ToString());

            sb.Length = 0;          
            sb.Append($"{oldsim:F1}");
            if (string.IsNullOrEmpty(oldname) || !imgX.NextName.Equals(oldname))
            {
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F1}");
            }

            if (updates > 0)
            {
                sb.Append($" ({updates})");
            }

            sb.Append(" / ");
            sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastView))} ago");
            sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(oldchecked))} ago]");
            return sb.ToString();
        }
    }
}
