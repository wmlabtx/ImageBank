using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private int _add = 0;

        public string ComputeSim()
        {
            AppVars.SuspendEvent.WaitOne(Timeout.Infinite);

            var dt = DateTime.Now;

            if (_imgList.Count == 0)
            {
                return null;
            }

            Img imgX = null;
            var maxid = _imgList.Max(e => e.Value.Id);
            var scopeid = _imgList
                .Select(e => e.Value)
                .Where(e => e.Id > 0)
                .ToArray();

            if (scopeid.Length == 0)
            {
                imgX = _imgList
                    .OrderBy(e => e.Value.LastChecked)
                    .FirstOrDefault()
                    .Value;
            }
            else
            {
                _add++;
                if (_add == 5)
                {
                    _add = 0;
                    imgX = _imgList
                        .OrderBy(e => e.Value.LastChecked)
                        .FirstOrDefault()
                        .Value;
                }
                else
                {
                    imgX = scopeid
                        .OrderBy(e => e.DoneProgress(maxid))
                        .ThenBy(e => e.LastChecked)
                        .FirstOrDefault();
                }
            }

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
            maxid = _imgList.Max(e => e.Value.Id);
            var count = _imgList.Count();
            var countid = _imgList.Count(e => e.Value.Id > 0);
            var countsim = _imgList.Count(e => 
                e.Value.Id > 0 && 
                !e.Value.Name.Equals(e.Value.NextName) && 
                _imgList.ContainsKey(e.Value.NextName) && 
                e.Value.LastView < e.Value.LastChanged &&
                e.Value.Sim > AppConsts.MinSim);
            sb.Append($"{countsim}/{countid}/{count}: {_avgTimes:F2}s ");

            var progress = imgX.DoneProgress(maxid);
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