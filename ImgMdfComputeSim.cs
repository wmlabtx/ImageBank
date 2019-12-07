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

            var maxid = _imgList.Max(e => e.Value.Id);
            var avgid = _imgList.Where(e => e.Value.Id > 0).Average(e => (float)e.Value.LastId / maxid);

            var imgX = avgid > 0.5f ?
                _imgList
                .Select(e => e.Value)
                .OrderBy(e => e.LastId)
                .ThenBy(e => e.LastChecked)
                .FirstOrDefault() :
                _imgList
                .Select(e => e.Value)
                .Where(e => e.Id > 0)
                .OrderBy(e => e.LastChecked)
                .FirstOrDefault();

            var oldnextname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var oldsim = imgX.Sim;

            FindNextName(imgX);

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            var count = _imgList.Count();
            sb.Append($"{avgid:F4}/{count}: {_avgTimes:F2}s ");

            var progress = imgX.LastId * 100.0 / _imgList.Max(e => e.Value.Id);
            sb.Append($"({progress:F2}%) ");

            sb.Append($"{oldsim:F2}");
            if (!imgX.NextName.Equals(oldnextname) || oldsim != imgX.Sim)
            {
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F2}");
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