using OpenCvSharp;
using System;

namespace ImageBank
{
    public static class HelperOrbs
    {
        public static bool ComputeOrbs(byte[] jpgdata, out Mat orbs)
        {
            orbs = null;
            if (jpgdata == null || jpgdata.Length == 0)
            {
                return false;
            }

            using (var orb = ORB.Create(AppConsts.MaxOrbsInImage))
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
                                orbs = null;
                                return false;
                            }

                            while (orbs.Rows > AppConsts.MaxOrbsInImage)
                            {
                                orbs = orbs.RowRange(0, AppConsts.MaxOrbsInImage);
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
    }
}