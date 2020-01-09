using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace ImageBank
{
    public class HelperDescriptors
    {
        public static bool ComputeDescriptors(byte[] jpgdata, out ulong[] descriptors)
        {
            descriptors = null;
            if (jpgdata == null || jpgdata.Length == 0)
            {
                return false; 
            }

            try
            {
                using (var matsource = Mat.FromImageData(jpgdata, ImreadModes.Grayscale))
                {
                    if (matsource.Width == 0 || matsource.Height == 0)
                    {
                        return false;
                    }

                    const double fsample = 1024.0 * 768.0;
                    var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                    using (var mat = matsource.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                    {
                        using (var orb = ORB.Create(AppConsts.MaxDescriptorsInImage))
                        {
                            var keypoints = orb.Detect(mat);
                            var matdescriptors = new Mat();
                            orb.Compute(mat, ref keypoints, matdescriptors);
                            if (matdescriptors.Rows == 0)
                            {
                                return false;
                            }

                            matdescriptors.GetArray(out byte[] array);
                            descriptors = To64(array);

                            /*
                            var bstat = new int[256];
                            var offset = 0;
                            var offset256 = 0;
                            var offset8 = 0;
                            var mask = (byte)0x80;
                            while (offset < descriptors.Length)
                            {
                                if ((descriptors[offset] & mask) != 0)
                                {
                                    bstat[offset256]++;
                                }

                                offset256++;
                                offset8++;
                                mask >>= 1;

                                if (offset8 == 8)
                                {
                                    offset8 = 0;
                                    mask = 0x80;
                                    offset++;
                                }

                                if (offset256 == 256)
                                {
                                    offset256 = 0;
                                }
                            }

                            var mid = matdescriptors.Rows / 2;
                            var result = new byte[32];
                            offset = 0;
                            offset256 = 0;
                            offset8 = 0;
                            mask = 0x80;
                            while (offset256 < bstat.Length)
                            {
                                if (bstat[offset256] > mid)
                                {
                                    result[offset] |= mask;
                                }

                                offset256++;
                                offset8++;
                                mask >>= 1;

                                if (offset8 == 8)
                                {
                                    offset8 = 0;
                                    mask = 0x80;
                                    offset++;
                                }
                            }

                            fingerprint = new ulong[4];
                            Buffer.BlockCopy(result, 0, fingerprint, 0, 32);
                            */
                        }
                    }
                }
            }
            catch (Exception)
            {
                descriptors = null;
                return false;
            }

            return true;
        }

        public static ulong[] To64(byte[] buffer)
        {
            var ulongs = new ulong[buffer.Length / 8];
            Buffer.BlockCopy(buffer, 0, ulongs, 0, buffer.Length);
            return ulongs;
        }

        public static byte[] From64(ulong[] array)
        {
            var buffer = new byte[array.Length * 8];
            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        public static int GetHammingDistance(IReadOnlyList<ulong> x, int xoffset, IReadOnlyList<ulong> y, int yoffset)
        {
            var distance = 0;
            for (var i = 0; i < 4; i++)
            {
                distance += Intrinsic.PopCnt(x[xoffset + i] ^ y[yoffset + i]);
                if (distance >= 64)
                {
                    break;
                }
            }

            return distance;
        }

        private struct DistanceTwo
        {
            public int X;
            public int Y;
            public int D;
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
                    var distance = GetHammingDistance(x, xoffset, y, yoffset);
                    if (distance < 64)
                    {
                        list.Add(new DistanceTwo() { X = xoffset, Y = yoffset, D = distance });
                    }

                    yoffset += 32;
                }

                xoffset += 32;
            }

            list = list.OrderBy(e => e.D).ToList();
            var sum = 0;
            while (list.Count > 0)
            {
                var mind = list[0].D;
                var minx = list[0].X;
                var miny = list[0].Y;
                sum += 64 - mind;
                list.RemoveAll(e => e.X == minx || e.Y == miny);
            }

            return sum * 4f / x.Length;
        }


        /*
        public static float[] ToFloat(byte[] buffer)
        {
            var floats = new float[buffer.Length / 4];
            Buffer.BlockCopy(buffer, 0, floats, 0, buffer.Length);
            return floats;
        }
        */

        /*
    public static byte[] ToBuffer(float[] array)
    {
        var buffer = new byte[array.Length * 4];
        Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
        return buffer;
    }

    public static byte[] ToBuffer(float[,] array)
    {
        var buffer = new byte[array.Length * 4];
        Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
        return buffer;
    }

    public static float[,] ToF2D(byte[] buffer)
    {
        var f2d = new float[256, 32];
        Buffer.BlockCopy(buffer, 0, f2d, 0, buffer.Length);
        return f2d;
    }

    public static Mat ToMat(byte[] buffer)
    {
        var mat = new Mat(buffer.Length / 32, 32, MatType.CV_8U);
        mat.SetArray(buffer);
        return mat;
    }

    public static float GetSim(Mat x, Mat y)
    {
        var dmatch = bFMatcher.Match(x, y);
        var sum = dmatch
            .Where(e => e.Distance < 64)
            .Sum(e => 64 - e.Distance);

        var sim = sum / x.Rows;
        return sim;
    }
    */

        /*
        private struct Rod
        {
            public int I;
            public int J;
            public int H;
        }

        public static float Distance(ulong[] x, ulong[] y)
        {
            var list = new List<Rod>();
            for (var i = 0; i < x.Length; i++)
            {
                for (var j = 0; j < y.Length; j++)
                {
                    list.Add(new Rod() { I = i, J = j, H = Intrinsic.PopCnt(x[i] ^ y[j]) });
                }
            }

            list = list.OrderBy(e => e.H).ToList();
            var sum = 0;
            while (list.Count() > 0)
            {
                sum += list[0].H;
                var ix = list[0].I;
                var jx = list[0].J;
                list.RemoveAll(e => e.I == ix || e.J == jx);
            }

            var distance = (float)sum / x.Length;
            return distance;
        }
        */

        /*
        public static Mat ConvertToMat(byte[] buffer)
        {
            var mat = new Mat(buffer.Length / 8, 8, MatType.CV_8U);
            mat.SetArray(buffer);
            return mat;
        }

        public static float Distance(Mat x, Mat y)
        {
            var dmatch = bFMatcher.Match(x, y);
            var sum = dmatch.Sum(e => e.Distance) + 256 * (x.Rows - dmatch.Length);
            var distance = sum / x.Rows;
            return distance;
        }
        */

        /*
    public static float GetDelta(float[] x, float[] y)
    {
        var delta = 0f;
        for (var i = 0; i < x.Length; i++)
        {
            delta += Math.Abs(x[i] - y[i]);
        }

        return delta;
    }

    public static float GetDelta(float[,] x, float[,] y)
    {
        var delta = 0f;
        for (var i = 0; i < 256; i++)
        {
            for (var j = 0; j < 32; j++)
            {
                delta += Math.Abs(x[i, j] - y[i, j]);
            }
        }

        return delta;
    }
    */
    }
}
