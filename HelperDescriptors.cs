using OpenCvSharp;
using System;
using System.Linq;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        private static readonly BFMatcher _bfmatcher = new BFMatcher(NormTypes.Hamming, true);

        public static bool ComputeDescriptors(byte[] data, out Mat descriptors)
        {
            descriptors = null;
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
                        {
                            descriptors = new Mat();
                            orb.DetectAndCompute(mat, null, out _, descriptors);
                            if (descriptors.Rows == 0)
                            {
                                throw new Exception();
                            }

                            while (descriptors.Rows > AppConsts.MaxDescriptorsInImage)
                            {
                                descriptors = descriptors.RowRange(0, AppConsts.MaxDescriptorsInImage);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    descriptors = null;
                    return false;
                }
            }

            return true;
        }

        public static float GetSim(Mat x, Mat y)
        {
            if (x.Rows == 0 || y.Rows == 0)
            {
                return 0f;
            }

            var dmatch = _bfmatcher.Match(x, y);
            if (dmatch.Length > 0)
            {
                var ds = dmatch
                    .Where(e => e.Distance < 64)
                    .OrderBy(e => e.Distance)
                    .Select(e => e.Distance)
                    .ToArray();

                var sd = 0f;
                var k = 1f;
                foreach (var d in ds)
                {
                    sd += (64 - d) * k;
                    k /= 2f;
                }

                var sim = sd / 2f;
                return sim;
            }
            else
            {
                return 0f;
            }
        }
    }
}
