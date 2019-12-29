using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void FlannUpdate()
        {
            lock (_flannLock)
            {
                _flannNames = FlannUpdate(_flannBasedMatcher);
            }
        }

        private string[] FlannUpdate(FlannBasedMatcher _flann)
        {
            lock (_flannLock)
            {
                _flann.Clear();                
                var descriptors = _imgList
                    .Select(e => e.Value.Orbs)
                    .ToArray();

                if (descriptors.Length > 0)
                {
                    _flann.Add(descriptors);
                    _flann.Train();
                }

                var names = _imgList.Keys.ToArray();
                return names;
            }
        }

        private string FlannFindNextName(FlannBasedMatcher flann, string[] names, Img imgX)
        {
            lock (_flannLock)
            {
                var votes = new SortedDictionary<string, float>();
                var dmatcharray = flann.KnnMatch(imgX.Orbs, 16);
                foreach (var dmatch in dmatcharray)
                {
                    foreach (var d in dmatch)
                    {
                        if (d.Distance < 64)
                        {
                            var vote = 64 - d.Distance;
                            var name = names[d.ImgIdx];
                            if (!name.Equals(imgX.Name))
                            {
                                if (votes.ContainsKey(name))
                                {
                                    votes[name] += vote;
                                }
                                else
                                {
                                    votes.Add(name, vote);
                                }
                            }
                        }
                    }
                }

                if (votes.Count > 0)
                {
                    var candidates = votes.OrderByDescending(e => e.Value).Select(e => e.Key).ToArray();
                    foreach (var candidate in candidates)
                    {
                        if (_imgList.ContainsKey(candidate))
                        {
                            return candidate;
                        }
                    }

                    _flannAvailable = false;
                    return null;
                }
                else
                {
                    return imgX.Name;
                }
            }
        }

        private string FlannFindNextName(Img imgX)
        {
            lock (_flannLock)
            {


                if (HelperPath.IsLegacy(imgX.Folder))
                {
                    var group = _imgList
                        .Values
                        .Where(e => HelperPath.IsLegacy(e.Folder) && !e.Name.Equals(imgX.Name))
                        .ToArray();

                    if (group.Length > 0)
                    {
                        var names = group
                            .Select(e => e.Name)
                            .ToArray();

                        var index = HelperRandom.Next(group.Length);
                        return names[index];
                    }
                    else
                    {
                        return imgX.Name;
                    }
                }
                else
                {
                    _flannBasedMatcherFolder.Clear();
                    var group = _imgList
                        .Values
                        .Where(e => HelperPath.IsSubfolder(imgX.Folder, e.Folder))
                        .ToArray();

                    if (group.Length > 0)
                    {
                        var names = group
                            .Select(e => e.Name)
                            .ToArray();

                        var descriptors = group
                            .Select(e => e.Orbs)
                            .ToArray();

                        _flannBasedMatcherFolder.Add(descriptors);
                        _flannBasedMatcherFolder.Train();

                        return FlannFindNextName(_flannBasedMatcherFolder, names, imgX, progress);
                    }
                    else
                    {
                        return imgX.Name;
                    }
                }
            }
        }
    }
}
