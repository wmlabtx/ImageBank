using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        public static bool ComputeVector(byte[] data, out ulong[] vector)
        {
            vector = null;
            if (data == null || data.Length == 0)
            {
                return false;
            }

            using (var orb = ORB.Create(AppConsts.MaxDescriptorsInImage))
            {
                try
                {
                    using (var matsource = Mat.FromImageData(data, ImreadModes.Grayscale))
                    {
                        if (matsource.Width == 0 || matsource.Height == 0)
                        {
                            return false;
                        }

                        const double fsample = 800.0 * 600.0;
                        var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                        using (var mat = matsource.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                        using (var descriptors = new Mat())
                        {
                            orb.DetectAndCompute(mat, null, out _, descriptors);
                            if (descriptors.Cols != 32 || descriptors.Rows == 0)
                            {
                                throw new Exception();
                            }

                            var buffer = HelperConvertors.ConvertMatToBuffer(descriptors);
                            var orbcluster = new OrbCluster();
                            var offset = 0;
                            while (offset < buffer.Length)
                            {
                                var orbpoint = new OrbPoint(buffer, offset);
                                orbcluster.OrbPoints.Add(orbpoint);
                                offset += 32;
                            }

                            vector = orbcluster.GetCenter();

                            /*
                            var orbclusters = new List<OrbCluster>();
                            orbclusters.Add(orbcluster);
                            while (orbclusters.Count < AppConsts.MaxClustersInImage)
                            {
                                orbclusters = orbclusters.OrderByDescending(e => e.OrbPoints.Count).ToList();
                                var medianbit = orbclusters[0].GetMedianBit();
                                var oc0 = new OrbCluster();
                                var oc1 = new OrbCluster();
                                foreach (var oc in orbclusters[0].OrbPoints)
                                {
                                    if (oc.IsBit(medianbit))
                                    {
                                        oc1.OrbPoints.Add(oc);
                                    }
                                    else
                                    {
                                        oc0.OrbPoints.Add(oc);
                                    }
                                }

                                orbclusters.RemoveAt(0);
                                orbclusters.Add(oc0);
                                orbclusters.Add(oc1);
                            }
                            
                            vector = new ulong[orbclusters.Count * 4];
                            offset = 0;
                            while (offset < vector.Length)
                            {
                                var center = orbclusters[offset / 4].GetCenter();
                                Buffer.BlockCopy(center, 0, vector, offset * sizeof(ulong), 32);
                                offset += 4;
                            }
                            */
                        }
                    }
                }
                catch (Exception)
                {
                    vector = null;
                    return false;
                }
            }

            return true;
        }

        private struct DistanceTwo
        {
            public int X;
            public int Y;
            public int D;
        }

        private static int HammingDistance(IReadOnlyList<ulong> x, int xoffset, IReadOnlyList<ulong> y, int yoffset)
        {
            var distance = 0;
            for (var i = 0; i < 4; i++)
            {
                distance += Intrinsic.PopCnt(x[xoffset + i] ^ y[yoffset + i]);
            }

            return distance;
        }

        public static int GetDistance(ulong[] x, ulong[] y, int min)
        {
            var list = new List<DistanceTwo>();
            var xoffset = 0;
            while (xoffset < x.Length)
            {
                var yoffset = 0;
                while (yoffset < y.Length)
                {
                    var d = HammingDistance(x, xoffset, y, yoffset);
                    list.Add(new DistanceTwo() { X = xoffset, Y = yoffset, D = d });
                    yoffset += 4;
                }

                xoffset += 4;
            }

            list = list.OrderBy(e => e.D).ToList();
            var distance = 0;
            while (list.Count > 0)
            {
                var mind = list[0].D;
                var minx = list[0].X;
                var miny = list[0].Y;
                distance += mind;
                if (distance > min)
                {
                    return distance;
                }

                var pos = list.Count - 1;
                while (pos >= 0)
                {
                    if (list[pos].X == minx || list[pos].Y == miny)
                    {
                        list.RemoveAt(pos);
                    }

                    pos--;
                }
            }

            return distance;
        }
    }
}
