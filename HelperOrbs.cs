using OpenCvSharp;
using System;
using System.Linq;

namespace ImageBank
{
    public static class HelperOrbs
    {
        private static readonly BFMatcher _bfmatcher = new BFMatcher(NormTypes.Hamming, true);

        public static bool ComputeOrbs(byte[] jpgdata, out Mat orbs)
        {
            orbs = null;
            if (jpgdata == null || jpgdata.Length == 0)
            {
                return false;
            }

            using (var orb = ORB.Create(AppConsts.MaxDescriptorsInImage))
            {
                try
                {
                    using (var matsource = Mat.FromImageData(jpgdata, ImreadModes.Grayscale))
                    {
                        if (matsource.Width == 0 || matsource.Height == 0)
                        {
                            return false;
                        }

                        const double fsample = 800.0 * 600.0;
                        var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                        using (var mat = matsource.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                        {
                            orbs = new Mat();
                            orb.DetectAndCompute(mat, null, out _, orbs);
                            if (orbs.Rows == 0)
                            {
                                throw new Exception();
                            }

                            while (orbs.Rows > AppConsts.MaxDescriptorsInImage)
                            {
                                orbs = orbs.RowRange(0, AppConsts.MaxDescriptorsInImage - 1);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    orbs = null;
                    return false;
                }
            }

            return true;
        }

        public static float GetDistance(Mat x, Mat y)
        {
            if (x.Rows == 0 || y.Rows == 0)
            {
                return 256f;
            }

            var dmatch = _bfmatcher.Match(x, y);
            if (dmatch.Length > 0)
            {
                var distance = (dmatch.Sum(e => e.Distance) + (256f * (x.Rows - dmatch.Length)))/ x.Rows;
                return distance;
            }
            else
            {
                return 256f;
            }
        }
    }
}
