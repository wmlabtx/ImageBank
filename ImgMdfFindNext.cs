using System;
using System.Diagnostics;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void FindNext(int idX, out int lastid, out DateTime lastchange, out int nextid, out int match, out int updated)
        {
            var imgX = _imgList[idX];
            lastid = imgX.LastId;
            lastchange = imgX.LastChange;
            nextid = imgX.NextId;
            match = imgX.Match;
            updated = 0;
            if (!_imgList.ContainsKey(nextid)) {
                lastid = -1;
            }

            if (lastid < 0) {
                nextid = imgX.Id;
                match = 0;
            }

            var candidates = _imgList
                .Values
                .Where(e => e.Id > imgX.LastId && e.Id <= imgX.Id)
                .OrderBy(e => e.Id)
                .ToArray();

            if (candidates.Length == 0) {
                return;
            }

            //var sw = Stopwatch.StartNew();
            foreach (var imgY in candidates) {
                lastid = imgY.Id;
                if (imgY.Id == imgX.Id) {
                    continue;
                }

                var matchxy = HelperDescriptors.ComputeMatch(imgX.GetDescriptors(), imgY.GetDescriptors());
                if (matchxy > match) {
                    match = matchxy;
                    nextid = imgY.Id;
                    lastchange = DateTime.Now;
                }

                if (matchxy > imgY.Match) {
                    imgY.Match = matchxy;
                    imgY.NextId = imgX.Id;
                    imgY.LastChange = DateTime.Now;
                    updated++;
                }


                //if (sw.ElapsedMilliseconds > 1500) {
                //    break;
                //}
            }
        }
    }
}
