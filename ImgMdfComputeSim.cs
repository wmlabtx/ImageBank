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
                .Where(e => e.Value.Name.Equals(e.Value.NextName) || !_imgList.ContainsKey(e.Value.NextName))
                .Select(e => e.Value)
                .ToArray();

            var imgX = scopewrong.Length > 0 ?
                scopewrong.First() :
                _imgList.Values.OrderBy(e => e.LastChecked).FirstOrDefault();

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
            sb.Append($"{_imgList.Count}: ");
            sb.Append($"{_avgTimes:F2}s ");

            sb.Append($"{oldsim:F1}");
            if (string.IsNullOrEmpty(oldname) || !imgX.NextName.Equals(oldname))
            {
                if (HelperPath.IsLegacy(imgX.Folder))
                {
                    imgX.LastView = GetMinLastView();
                    HelperSql.UpdateLastView(imgX.Name, imgX.LastView);
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
