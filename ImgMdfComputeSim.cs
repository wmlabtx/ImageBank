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

            Img imgX;
            int countunfinished;
            var maxid = _imgList.Max(e => e.Value.Id);
            if (maxid == 0)
            {
                countunfinished = 0;
                imgX = _imgList
                    .OrderBy(e => e.Value.LastChecked)
                    .FirstOrDefault()
                    .Value;
            }
            else
            {
                var scope = _imgList
                    .Where(e => e.Value.LastId < maxid)
                    .Select(e => e.Value)
                    .ToArray();

                if (scope.Length == 0)
                {
                    Thread.Sleep(1000);
                    return "idle...";
                }

                countunfinished = scope.Length;
                imgX = scope
                    .OrderBy(e => e.LastChecked)
                    .FirstOrDefault();
            }

            var oldnextname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var olddistance = imgX.Distance;

            FindNextName(imgX);

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            var count = _imgList.Count();
            
            var scopeok = _imgList
                .Where(e =>
                    e.Value.Vector.Length > 0 &&
                    _imgList.ContainsKey(e.Value.NextName) &&
                    !e.Value.Name.Equals(e.Value.NextName) &&
                    e.Value.LastView < e.Value.LastChanged)
                .Select(e => e.Value)
                .ToArray();

            var mindistance = scopeok.Min(e => e.Distance);
            var countdistance = scopeok.Count(e => e.Distance == mindistance);

            sb.Append($"{countunfinished}/{mindistance}:{countdistance}/{count}: {_avgTimes:F2}s ");

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