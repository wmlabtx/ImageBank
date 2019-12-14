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

            var count = _imgList.Count();
            if (count == 0)
            {
                return null;
            }

            var scopevalid = _imgList
                .Values
                .Where(e =>
                    e.Descriptors.Rows > 0 &&
                    _imgList.ContainsKey(e.NextName) &&
                    !e.Name.Equals(e.NextName))
                .ToArray();

            var scopechanged = scopevalid
                .Where(e => e.LastView < e.LastChanged)
                .ToArray();

            var countchanged = scopechanged.Length;
            var maxid = _imgList.Max(e => e.Value.Id);

            Img imgX;
            if (maxid == 0)
            {
                imgX = _imgList
                    .Values
                    .OrderBy(e => e.LastChecked)
                    .FirstOrDefault();
            }
            else
            {
                var scope = _imgList
                    .Values
                    .Where(e => e.LastId < maxid)
                    .ToArray();

                if (scope.Length == 0)
                {
                    Thread.Sleep(1000);
                    return "idle...";
                }

                imgX = scope
                    .OrderBy(e => e.LastChecked)
                    .FirstOrDefault();
            }

            var oldnextname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var oldsim = imgX.Sim;
            var oldlastid = imgX.LastId;

            FindNextName(imgX);

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null)
            {
                return null;
            }

            var sb = new StringBuilder();            
            if (countchanged > 0)
            {
                sb.Append($"{countchanged}/");
            }
                
            sb.Append($"{count}: {_avgTimes:F2}s ");

            sb.Append($"{oldsim:F2} [{oldlastid}]");
            if (!imgX.NextName.Equals(oldnextname) || oldsim != imgX.Sim)
            {
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F2} [{imgX.LastId}]");
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