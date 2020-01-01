using OpenCvSharp;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageBank
{
    public class Flann
    {
        private static readonly object _flannLock = new object();
        private static readonly BFMatcher _bfMatcher = new BFMatcher(NormTypes.Hamming, true);
        private readonly FlannBasedMatcher _flannBasedMatcher;
        private readonly LshIndexParams _indexParams;
        private string[] _flannHashes;

        public Flann(Img[] images)
        {
            _indexParams = new LshIndexParams(10, 20, 0);
            _flannBasedMatcher = new FlannBasedMatcher(_indexParams);
            _flannHashes = new string[0];
            Update(images);
        }

        public void Update(Img[] images)
        {
            lock (_flannLock)
            {
                _flannBasedMatcher.Clear();
                if (images.Length >= AppConsts.MaxFlann)
                {
                    var descriptors = images
                        .Select(e => e.Orbs)
                        .ToArray();

                    if (descriptors.Length > 0)
                    {
                        _flannBasedMatcher.Clear();
                        _flannBasedMatcher.Add(descriptors);
                        _flannBasedMatcher.Train();
                        _flannHashes = images
                            .Select(e => e.Hash)
                            .ToArray();
                    }
                }
            }
        }

        private string FindByFlann(Img imgX)
        {
            var votes = new SortedDictionary<string, float>();
            var dmatcharray = _flannBasedMatcher.KnnMatch(imgX.Orbs, 16);
            foreach (var dmatch in dmatcharray)
            {
                foreach (var d in dmatch)
                {
                    if (d.Distance < 64)
                    {
                        var vote = 64 - d.Distance;
                        var hash = _flannHashes[d.ImgIdx];
                        if (!hash.Equals(imgX.Hash))
                        {
                            if (votes.ContainsKey(hash))
                            {
                                votes[hash] += vote;
                            }
                            else
                            {
                                votes.Add(hash, vote);
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
                    if (AppVars.Collection.ContainsKey(candidate))
                    {
                        return candidate;
                    }
                }

                return null;
            }
            else
            {
                return imgX.Hash;
            }
        }

        private string FindByBf(Img imgX, Img[] images)
        {
            var nexthash = imgX.Hash;
            var maxvotes = 0f; 

            foreach (var imgY in images)
            {
                if (!imgY.Hash.Equals(imgX.Hash))
                {
                    var votes = ComputeVotes(imgX.Orbs, imgY.Orbs);
                    if (votes > maxvotes)
                    {
                        maxvotes = votes;
                        nexthash = imgY.Hash;
                    }
                }
            }

            return nexthash;
        }

        private float ComputeVotes(Mat x, Mat y)
        {
            var bfMatches = _bfMatcher.Match(x, y);
            var distances = bfMatches.ToArray();

            var votes = 0f;
            foreach (var d in distances)
            {
                if (d.Distance < 64)
                {
                    var vote = 64 - d.Distance;
                    votes += vote;
                }
            }

            return votes;
        }

        public string Find(Img imgX, Img[] images)
        {
            lock (_flannLock)
            {
                if (images.Length >= AppConsts.MaxFlann)
                {
                    if (
                        _flannHashes.Length == 0 || 
                        (((float)Math.Abs(images.Length - _flannHashes.Length)) / _flannHashes.Length) > 0.1
                        )
                    {
                        Update(images);
                    }

                    var nextname = FindByFlann(imgX);
                    if (string.IsNullOrEmpty(nextname))
                    {
                        Update(images);
                        nextname = FindByFlann(imgX);
                    }

                    return nextname;
                }
                else
                {
                    var nextname = FindByBf(imgX, images);
                    return nextname;
                }
            }
        }
    }
}
