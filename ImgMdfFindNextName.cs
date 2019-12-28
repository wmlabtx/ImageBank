using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private bool FindNextName(string nameX, out string nameY, out string message)
        {
            message = null;
            nameY = null;
            if (!_imgList.TryGetValue(nameX, out var imgX))
            {
                return false;
            }

            var oldnextname = imgX.NextName;
            var olddistance = imgX.Distance;
            var oldlastid = imgX.LastId;
            var oldlastcheck = imgX.LastCheck;
            var islegacy = imgX.Folder.StartsWith(AppConsts.FolderLegacy, StringComparison.OrdinalIgnoreCase);

            var scopeForComparison = new List<Img>();
            foreach (var imgY in _imgList.Values)
            {
                if (imgY.Name.Equals(imgX.Name))
                {
                    continue;
                }

                if (islegacy || (!islegacy && (imgY.Folder.IndexOf(imgX.Folder, StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    if (imgY.Id > imgX.LastId)
                    {
                        scopeForComparison.Add(imgY);
                    }
                }
            }

            if (scopeForComparison.Count > 1)
            {
                scopeForComparison = scopeForComparison.OrderBy(e => e.Id).ToList();
            }
                
            if (scopeForComparison.Count > 0)
            {
                var orbX = imgX.Orbs;
                var sw = new Stopwatch();
                sw.Start();
                foreach (var imgY in scopeForComparison)
                {
                    imgX.LastId = imgY.Id;
                    var orbY = imgY.Orbs;
                    var distance = HelperOrbs.GetDistance(orbX, orbY);
                    if (distance < imgX.Distance)
                    {
                        imgX.Distance = distance;
                        imgX.NextName = imgY.Name;
                        imgX.LastChange = DateTime.Now;
                    }

                    if (sw.ElapsedMilliseconds > 1000)
                    {
                        break;
                    }
                }
            }

            imgX.LastCheck = DateTime.Now;
            SqlUpdateNameNext(imgX);

            var nextchanged = false;
            if (imgX.Distance < olddistance)
            {
                nextchanged = true;
            }

            nameY = imgX.NextName;

            var sb = new StringBuilder();
            var collectionsize = _imgList.Count;
            var notviewsize = GetCountNotView();
            sb.Append($"{notviewsize}/{collectionsize}: ({_avgtimes:F2}s) ");
            sb.Append($"{olddistance:F2} [{oldlastid}] ");
            if (nextchanged)
            {
                sb.Append($"{char.ConvertFromUtf32(0x2192)} {imgX.Distance:F2} [{imgX.LastId}] ");
            }

            sb.Append("/ ");
            sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastView))} ago");
            sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(oldlastcheck))} ago]");
            message = sb.ToString();
            return true;
        }
    }
}