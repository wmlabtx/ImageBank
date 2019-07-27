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

            if (_imgList.Count == 0)
            {
                return null;
            }

            var scopewrong = _imgList
                .Where(e => !_imgList.ContainsKey(e.Value.NextName))
                .ToArray();

            var imgX = scopewrong.Length > 0 ?
                scopewrong
                    .FirstOrDefault().Value :
                _imgList
                    .OrderBy(e => e.Value.LastChecked)
                    .FirstOrDefault().Value;

            if (imgX == null)
            {
                return null;
            }

            var oldname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var oldsim = imgX.Sim;

            var dt = DateTime.Now;
            FindNextName(imgX);

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null)
            {
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

            /*
            imgX.LastView = DateTime.Now;
            SqlUpdateLastView(imgX.Name, imgX.LastView);
            */

            var sb = new StringBuilder();
            var g0 = _imgList.Count(e => e.Value.Generation == 0);
            var g1 = _imgList.Count(e => e.Value.Generation == 1);
            var g2 = _imgList.Count(e => e.Value.Generation == 2);
            sb.Append($"{g0}/{g1}/{g2}: ");
            sb.Append($"{_avgTimes:F2}s ");

            sb.Append($"{oldsim:F2}");
            if (string.IsNullOrEmpty(oldname) || !imgX.NextName.Equals(oldname))
            {
                if (!imgX.Name.Equals(oldname) && imgX.Generation == 2)
                {
                    imgX.Generation = 1;
                    HelperSql.UpdateGeneration(imgX);
                }

                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F2}");
            }

            sb.Append(" / ");
            sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastView))} ago");
            sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(oldchecked))} ago]");
            return sb.ToString();
        }
    }
}
