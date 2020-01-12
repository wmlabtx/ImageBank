using System;
using System.Diagnostics;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void FindNextHash(string hashX, out int lastid, out DateTime lastchange, out string nexthash, out float sim)
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
            else
            {
                if (lastid == 0 && sim > 0f)
                {
                    sim = 0f;
                }
            }

            var candidates = _imgList
                .Values
                .Where(e => e.Id >= imgX.LastId)
                .OrderBy(e => e.Id)
                .ToArray();

            if (candidates.Length == 0)
            {
                return;
            }

            var virgin = false;
            if (lastid == 0)
            {
                virgin = true;
            }

            var sw = new Stopwatch();
            sw.Start();
            foreach (var imgY in candidates)
            {
                lastid = imgY.Id + 1;
                if (imgY.Id == imgX.Id)
                {
                    continue;
                }
                
                var simxy = HelperDescriptors.GetDistance(imgX.Descriptors, imgY.Descriptors);
                if (simxy < sim || virgin)
                {
                    virgin = false;
                    sim = simxy;
                    nexthash = imgY.Hash;
                    lastchange = DateTime.Now;
                }

                if (sw.ElapsedMilliseconds > 1000)
                {
                    break;
                }
            }
        }
    }
}
