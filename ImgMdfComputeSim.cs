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
                //.OrderBy(e => e.Value.LastView)
                //.Take(1000)
                .OrderBy(e => e.Value.LastChecked)
                .FirstOrDefault()
                .Value;

            if (imgX.Descriptors.Size.Height == 0)
            {
                var jpgdata = GetJpgData(imgX);
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return null;
                }

                var crcname = HelperCrc.GetCrc(jpgdata);
                if (!crcname.Equals(imgX.Name))
                {
                    DeleteImg(imgX);
                    return null;
                }

                if (HelperDescriptors.ComputeDescriptors(jpgdata, out var descriptors))
                {
                    imgX.Descriptors = descriptors;
                    UpdateDescriptors(imgX);
                }
            }

            var oldnextname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var oldsim = imgX.Sim;
            
            var updates = FindNextName(imgX);

            var imgY = GetImgByName(imgX.NextName);
            if (imgY == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            var count = _imgList.Count();
            var minlvdate = _imgList.Min(e => e.Value.LastView.Date);
            var countzero = _imgList.Count(e => e.Value.Descriptors.Size.Height > 0);
            sb.Append($"{countzero}/{count}: {_avgTimes:F2}s ");

            if (updates > 0)
            {
                sb.Append($"({updates}) ");
            }

            sb.Append($"{oldsim:F4}");
            if (!imgX.NextName.Equals(oldnextname) || Math.Abs(oldsim - imgX.Sim) > 0.0001)
            {
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F4}");
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