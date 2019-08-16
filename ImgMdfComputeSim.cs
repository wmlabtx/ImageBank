﻿using System;
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

            Img imgX = null;
            foreach(var img in _imgList)
            {
                if (img.Value.Name.Equals(img.Value.NextName))
                {
                    imgX = img.Value;
                    break;
                }

                var imgNext = GetImgByName(img.Value.NextName);
                if (imgNext == null)
                {
                    imgX = img.Value;
                    break;
                }
            }

            if (imgX == null)
            {
                imgX = _imgList
                    .OrderBy(e => e.Value.LastChecked)
                    .FirstOrDefault()
                    .Value;
            }

            if (imgX == null)
            {
                return null;
            }

            var oldname = imgX.NextName;
            var oldchecked = imgX.LastChecked;
            var oldsim = imgX.Sim;

            var dt = DateTime.Now;
            var updates = FindNextName(imgX);

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

            var sb = new StringBuilder();
            var count = _imgList.Count();
            var counttoview = _imgList.Count(e => e.Value.LastView < e.Value.LastChanged);
            var countold = _imgList.Count(e => DateTime.Now.Subtract(e.Value.LastChecked).TotalDays > 200);
            sb.Append($"{counttoview}/{countold}/{count}: {_avgTimes:F2}s ");

            if (updates > 0)
            {
                sb.Append($"({updates}) ");
            }

            sb.Append($"{oldsim:F4}");
            if (!imgX.NextName.Equals(oldname) || Math.Abs(oldsim - imgX.Sim) > 0.0001)
            {
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F4}");
            }

            sb.Append(" / ");
            sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastView))} ago");
            sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(oldchecked))} ago]");
            return sb.ToString();
        }
    }
}