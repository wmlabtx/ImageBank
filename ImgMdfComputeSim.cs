using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        int counterimport = 0;

        public string ComputeSim()
        {
            AppVars.SuspendEvent.WaitOne(Timeout.Infinite);

            if (_imgList.Count == 0)
            {
                return null;
            }

            if (counterimport > 9)
            {
                counterimport = 0;
                Import(10, null);
            }

            var imgX = _imgList.Values.OrderBy(e => e.LastChecked).FirstOrDefault();
            if (imgX == null)
            {
                return null;
            }

            var oldname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var oldsim = imgX.Sim;

            var dt = DateTime.Now;
            var updates = FindNextName(imgX);

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
            sb.Append($"[{counterimport}] ");
            sb.Append($"{oldsim:F1}");
            if (string.IsNullOrEmpty(oldname) || !imgX.NextName.Equals(oldname))
            {
                counterimport = 0;
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F1}");
            }
            else
            {
                counterimport++;
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
