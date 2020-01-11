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

                            while (matdescriptors.Rows > AppConsts.MaxDescriptorsInImage)
                            {
                                matdescriptors = matdescriptors.RowRange(0, AppConsts.MaxDescriptorsInImage);
                            }

                            matdescriptors.GetArray(out byte[] array);
                            descriptors = To64(array);
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
    }
}
