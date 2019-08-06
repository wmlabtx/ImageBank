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

            Img imgX = null;
            var scopeWrongNew = _imgList
                .Where(e => e.Value.Gen == Img.GenNew && !_imgList.ContainsKey(e.Value.NextName))
                .ToArray();

            if (scopeWrongNew.Length > 0)
            {
                imgX = scopeWrongNew.FirstOrDefault().Value;
            }
            else
            {
                var scopeWrong = _imgList
                    .Where(e => !_imgList.ContainsKey(e.Value.NextName))
                    .ToArray();

                if (scopeWrong.Length > 0)
                {
                    imgX = scopeWrong.FirstOrDefault().Value;
                }
                else
                {
                    foreach(var img in _imgList)
                    {
                        var imgNext = GetImgByName(img.Value.NextName);
                        if (imgNext == null)
                        {
                            imgX = img.Value;
                            break;
                        }

                        if (!HelperPath.NodesComparable(img.Value.Node, imgNext.Node))
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

                }
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
            var genew = _imgList.Count(e => e.Value.Gen == Img.GenNew);
            var genmodified = _imgList.Count(e => e.Value.Gen == Img.GenModified);
            var genviewed= _imgList.Count(e => e.Value.Gen == Img.GenViewed);
            sb.Append($"{genew}/{genmodified}/{genviewed}/{count}: {_avgTimes:F2}s ");

            if (updates > 0)
            {
                sb.Append($"({updates}) ");
            }

            sb.Append($"{oldsim:F2}");
            if (!imgX.Name.Equals(oldname) && !imgX.NextName.Equals(oldname))
            {
                sb.Append($" {char.ConvertFromUtf32(0x2192)} {imgX.Sim:F2}");
            }

            sb.Append(" / ");
            sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastView))} ago");
            sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(oldchecked))} ago]");
            return sb.ToString();
        }
    }
}
