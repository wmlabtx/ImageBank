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

            var maxid = _imgList.Max(e => e.Value.Id);
            var avg = 1f;
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
                var scopeid = _imgList
                    .Values
                    .Where(e => e.Descriptors.Rows > 0)
                    .ToArray();

                avg = (float)scopeid.Sum(e => e.LastId) / (scopeid.Length * maxid);
                if (avg > 0.5)
                {
                    imgX = _imgList
                        .Values
                        .OrderBy(e => e.LastChecked)
                        .FirstOrDefault();
                }
                else
                {
                    imgX = _imgList
                        .Values
                        .Where(e => e.Descriptors.Rows > 0)
                        .OrderBy(e => e.LastId)
                        .FirstOrDefault();
                }
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
            sb.Append($"{count}: {_avgTimes:F2}s {avg:F4}: ");

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