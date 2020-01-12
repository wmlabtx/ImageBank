using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.ImgHash;
using OpenCvSharp.XFeatures2D;

namespace ImageBank
{
    public class HelperDescriptors
    {
        public static bool ComputeDescriptors(byte[] jpgdata, out int[] descriptors)
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

                    const double fsample = 256.0 * 256.0;
                    var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                    using (var mat = matsource.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                    {
                        using (var sift = SIFT.Create(AppConsts.MaxDescriptorsInImage))
                        {
                            var keypoints = sift.Detect(mat);
                            if (keypoints.Length == 0)
                            {
                                return false;
                            }

                            if (keypoints.Length > AppConsts.MaxDescriptorsInImage)
                            {
                                keypoints = keypoints
                                    .ToList()
                                    .Take(AppConsts.MaxDescriptorsInImage)
                                    .ToArray();
                            }

                            var matdescriptors = new Mat();
                            sift.Compute(mat, ref keypoints, matdescriptors);
                            if (matdescriptors.Rows == 0)
                            {
                                return false;
                            }

                            while (matdescriptors.Rows > AppConsts.MaxDescriptorsInImage)
                            {
                                matdescriptors = matdescriptors.RowRange(0, AppConsts.MaxDescriptorsInImage);
                            }

                            matdescriptors.GetArray(out float[] fdescriptors);
                            descriptors = Array.ConvertAll(fdescriptors, e => (int)e);
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

        public static float GetDistance(IReadOnlyList<int> x, int xoffset, IReadOnlyList<int> y, int yoffset)
        {
            var sum = 0;
            for (var i = 0; i < 128; i++)
            {
                sum += (x[xoffset + i] - y[yoffset + i]) * (x[xoffset + i] - y[yoffset + i]);
            }

            var distance = (float)Math.Sqrt(sum);
            return distance;
        }

        private struct DistanceTwo
        {
            public int X;
            public int Y;
            public float D;
        }

        public static float GetDistance(int[] x, int[] y)
        {
            float distance;
            var list = new List<DistanceTwo>();
            var xoffset = 0;
            while (xoffset < x.Length)
            {
                var yoffset = 0;
                while (yoffset < y.Length)
                {
                    distance = GetDistance(x, xoffset, y, yoffset);
                    list.Add(new DistanceTwo() { X = xoffset, Y = yoffset, D = distance });
                    yoffset += 128;
                }

                xoffset += 128;
            }

            list = list.OrderBy(e => e.D).ToList();
            var sum = 0f;
            var cnt = 0;
            while (list.Count > 0)
            {
                var mind = list[0].D;
                var minx = list[0].X;
                var miny = list[0].Y;
                sum += mind;
                cnt++;
                list.RemoveAll(e => e.X == minx || e.Y == miny);
            }

            distance = sum / cnt;
            return distance;
        }
    }
}
