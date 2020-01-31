using System;
using System.Diagnostics;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void FindNextHash(string hashX, out int lastid, out DateTime lastchange, out string nexthash, out float sim)
        {
            var imgX = _imgList[hashX];
            lastid = imgX.LastId;
            lastchange = imgX.LastChange;
            nexthash = imgX.NextHash;
            sim = imgX.Sim;
            if (!_imgList.ContainsKey(nexthash)) {
                lastid = -1;
            }

            if (lastid < 0) {
                nexthash = imgX.Hash;
                sim = 0f;
            }

            var candidates = _imgList
                .Values
                .Where(e => e.Id > imgX.LastId && e.Id <= imgX.Id)
                .OrderBy(e => e.Id)
                .ToArray();

            if (candidates.Length == 0) {
                return;
            }

            var sw = Stopwatch.StartNew();
            foreach (var imgY in candidates) {
                lastid = imgY.Id;
                if (imgY.Id == imgX.Id) {
                    continue;
                }

                var simxy = HelperDescriptors.ComputeSimilarity(imgX.Descriptors, imgY.Descriptors);
                if (simxy > sim) {
                    sim = simxy;
                    nexthash = imgY.Hash;
                    lastchange = DateTime.Now;
                }

                if (sw.ElapsedMilliseconds > 1500) {
                    break;
                }
            }
        }
    }
}
