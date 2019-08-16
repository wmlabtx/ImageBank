using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        public static bool ComputeDescriptors(byte[] data, out ulong[] udescriptors)
        {
            udescriptors = null;
            if (data == null || data.Length == 0)
            {
                return false;
            }

            var orb = ORB.Create(AppConsts.MaxOrbPointsInImage);
            try
            {
                using (var matsource = Mat.FromImageData(data, ImreadModes.Grayscale))
                {
                    if (matsource.Width == 0 || matsource.Height == 0)
                    {
                        return false;
                    }

                    const double fsample = 1024.0 * 768.0;
                    var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                    using (var mat = matsource.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                    {
                        var descriptors = new Mat();
                        orb.DetectAndCompute(mat, null, out _, descriptors);
                        if (descriptors.Cols != 32 || descriptors.Rows == 0)
                        {
                            throw new Exception();
                        }

                        while (descriptors.Rows > AppConsts.MaxOrbPointsInImage)
                        {
                            descriptors = descriptors.RowRange(0, AppConsts.MaxOrbPointsInImage);
                        }

                        var buffer = new byte[descriptors.Rows * descriptors.Cols];
                        descriptors.GetArray(0, 0, buffer);
                        udescriptors = ConvertToDescriptors(buffer);
                    }
                }
            }
            catch (Exception)
            {
                udescriptors = null;
                return false;
            }

            return true;
        }

        private struct DistanceTwo
        {
            public int X;
            public int Y;
            public int D;
        }

        private static int Dcomparer(DistanceTwo x, DistanceTwo y)
        {
            return x.D.CompareTo(y.D);
        }

        private static int HammingDistance(IReadOnlyList<ulong> x, int xoffset, IReadOnlyList<ulong> y, int yoffset)
        {
            var distance = 0;
            for (var i = 0; i < 4; i++)
            {
                distance += Intrinsic.PopCnt(x[xoffset + i] ^ y[yoffset + i]);
                if (distance >= AppConsts.MaxHammingDistance)
                {
                    break;
                }
            }

            return distance;
        }

        public static float GetSim(ulong[] x, ulong[] y)
        {
            var list = new List<DistanceTwo>();
            var xoffset = 0;
            while (xoffset < x.Length)
            {
                var yoffset = 0;
                while (yoffset < y.Length)
                {
                    var distance = HammingDistance(x, xoffset, y, yoffset);
                    if (distance < AppConsts.MaxHammingDistance)
                    {
                        list.Add(new DistanceTwo() { X = xoffset, Y = yoffset, D = distance });
                    }

                    yoffset += 4;
                }

                xoffset += 4;
            }

            list.Sort(Dcomparer);
            float sum = 0f;
            while (list.Count > 0)
            {
                var mind = list[0].D;
                var minx = list[0].X;
                var miny = list[0].Y;
                var k = (float)Math.Pow(((AppConsts.MaxHammingDistance - mind) / (double)AppConsts.MaxHammingDistance), 2.0);
                sum += k;
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

            var sim = sum * 4f / x.Length;
            return sim;
        }

        public static ulong[] ConvertToDescriptors(byte[] buffer)
        {
            var udescriptors = new ulong[buffer.Length / sizeof(ulong)];
            Buffer.BlockCopy(buffer, 0, udescriptors, 0, buffer.Length);
            return udescriptors;
        }

        public static byte[] ConvertToByteArray(ulong[] udescriptors)
        {
            var buffer = new byte[udescriptors.Length * sizeof(ulong)];
            Buffer.BlockCopy(udescriptors, 0, buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
