using System;
using System.Collections.Generic;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void FindNext(int idX, out int lastid, out DateTime lastchange, out int nextid, out int match)
        {
            lock (_imglock) {
                var imgX = _imgList[idX];
                lastchange = imgX.LastChange;
                nextid = imgX.Id;
                match = 0;
                lastid = _id;

                var candidates = new SortedDictionary<int, int>();
                foreach (var descriptor in imgX.GetDescriptors()) {
                    if (_descriptorList.TryGetValue(descriptor, out SortedDictionary<int, int> tree)) {
                        var countX = tree[idX];
                        foreach (var candidate in tree) {
                            if (candidate.Key == idX) {
                                continue;
                            }

                            var count = Math.Min(countX, candidate.Value);
                            if (!candidates.ContainsKey(candidate.Key)) {
                                candidates.Add(candidate.Key, count);
                            }
                            else {
                                candidates[candidate.Key] += count;
                            }

                            if (candidates[candidate.Key] > match) {
                                match = candidates[candidate.Key];
                                nextid = candidate.Key;
                                lastchange = DateTime.Now;
                            }
                        }
                    }
                }
            }
        }
    }
}
