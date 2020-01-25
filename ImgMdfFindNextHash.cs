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
            if (!_imgList.ContainsKey(nexthash))
            {
                lastid = 0;
                nexthash = imgX.Hash;
                sim = 0f;
            }

            var candidates = _imgList
                .Values
                .Where(e => e.Id >= imgX.LastId && e.Id <= imgX.Id)
                .OrderBy(e => e.Id)
                .ToArray();

            if (candidates.Length == 0)
            {
                return;
            }

            var sw = Stopwatch.StartNew();
            foreach (var imgY in candidates)
            {
                lastid = imgY.Id + 1;
                if (imgY.Id == imgX.Id)
                {
                    continue;
                }

                var simxy = HelperDescriptors.ComputeSimilarity(imgX.Descriptors, imgY.Descriptors);
                if (simxy > sim)
                {
                    sim = simxy;
                    nexthash = imgY.Hash;
                    lastchange = DateTime.Now;
                }

                if (imgX.Descriptors.Length != imgY.Descriptors.Length) {
                    simxy = HelperDescriptors.ComputeSimilarity(imgY.Descriptors, imgX.Descriptors);
                }
                
                if (simxy > imgY.Sim) {
                    imgY.Sim = simxy;
                    imgY.NextHash = imgX.Hash;
                    imgY.LastChange = DateTime.Now;
                }

                if (sw.ElapsedMilliseconds > 1000)
                {
                    break;
                }
            }
        }
    }
}
