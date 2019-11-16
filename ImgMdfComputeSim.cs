using System;
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

            var dt = DateTime.Now;

            if (_imgList.Count == 0)
            {
                return null;
            }

            var imgX = _imgList
                .Select(e => e.Value)
                .OrderBy(e => e.LastChecked)
                .FirstOrDefault();

            var oldnextname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var olddistance = imgX.Distance;

            var updates = FindNextName(imgX);

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            var count = _imgList.Count();
            var countzero = _imgList.Count(e => e.Value.PHash > 0 && e.Value.LastView < e.Value.LastChanged);
            sb.Append($"{countzero}/{count}: {_avgTimes:F2}s ");

            if (updates > 0)
            {
                sb.Append($"({updates}) ");
            }

            sb.Append($"{olddistance}");
            if (!imgX.NextName.Equals(oldnextname) || olddistance != imgX.Distance)
            {
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Distance}");
            }

            sb.Append(" / ");
            sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastView))} ago");
            sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(oldchecked))} ago]");

            _findTimes.Enqueue(DateTime.Now.Subtract(dt).TotalSeconds);
            if (_findTimes.Count > 100)
            {
                _findTimes.Dequeue();
            }

            if (_findTimes.Count > 0)
            {
                _avgTimes = _findTimes.Average();
            }

            return sb.ToString();
        }
    }
}