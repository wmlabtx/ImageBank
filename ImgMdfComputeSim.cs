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
            var scope = _imgList
                .Where(e => e.Value.LastId < maxid)
                .Select(e => e.Value)
                .ToArray();

            if (scope.Length == 0)
            {
                Thread.Sleep(1000);
                return "idle...";
            }

            var imgX = scope
                .OrderBy(e => e.LastId)
                .ThenBy(e => e.LastChecked)
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

            var scopeok = _imgList
                .Where(e => 
                    e.Value.Descriptors.Size.Height > 0 && 
                    _imgList.ContainsKey(e.Value.NextName) &&
                    !e.Value.Name.Equals(e.Value.NextName) &&
                    e.Value.LastView < e.Value.LastChanged)
                .Select(e => e.Value)
                .ToArray();

            var countok = scopeok.Length;
            if (countok > 0)
            {
                var maxsim = scopeok.Max(e => e.Sim);
                var countsim = scopeok.Count(e => e.Sim == maxsim);

                sb.Append($"{maxsim:F2}:{countsim}/{countok}/{count}: {_avgTimes:F2}s ");

                sb.Append($"{oldsim:F2}");
                if (!imgX.NextName.Equals(oldnextname) || oldsim != imgX.Sim)
                {
                    sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F2}");
                }

                sb.Append(" / ");
                sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastView))} ago");
                sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(oldchecked))} ago]");
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

            return sb.ToString();
        }
    }
}