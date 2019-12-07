using OpenCvSharp;
using System;
using System.Linq;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        private static readonly PHash _phash = PHash.Create();
        private static readonly BFMatcher _bfmatcher = new BFMatcher(NormTypes.Hamming2, true);

        public static bool ComputeDescriptors(byte[] data, out ulong phash, out Mat orbs)
        {
            phash = 0;
            orbs = null;
            if (data == null || data.Length == 0)
            {
                return false;
            }

            try
            {
                using (var matinput = Mat.FromImageData(data, ImreadModes.Grayscale))
                {
                    if (matinput.Width == 0 || matinput.Height == 0)
                    {
                        return false;
                    }

                    using (var matoutput = new Mat())
                    {
                        _phash.Compute(matinput, matoutput);
                        var buffer = new byte[8];
                        matoutput.GetArray(0, 0, buffer);
                        phash = ConvertBufferToPHash(buffer);
                    }

                    using (var orb = ORB.Create(AppConsts.MaxDescriptorsInImage))
                    {
                        const double fsample = 256.0 * 256.0;
                        var fx = Math.Sqrt(fsample / (matinput.Width * matinput.Height));
                        using (var mat = matinput.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                        {
                            orbs = new Mat();
                            orb.DetectAndCompute(mat, null, out _, orbs);
                            if (orbs.Cols != 32 || orbs.Rows == 0)
                            {
                                throw new Exception();
                            }

                            while (orbs.Height > AppConsts.MaxDescriptorsInImage)
                            {
                                orbs = orbs.RowRange(0, AppConsts.MaxDescriptorsInImage);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
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
            var dsum = dmatch.Where(e => e.Distance < AppConsts.MaxHammingDistance).Sum(e => AppConsts.MaxHammingDistance - e.Distance);
            var sim = dsum / x.Rows;
            return sim;
        }

        public static int GetDistance(ulong x, ulong y)
        {
            var distance = Intrinsic.PopCnt(x ^ y);
            return distance;
        }

        public static ulong ConvertBufferToPHash(byte[] buffer)
        {
            var phash = BitConverter.ToUInt64(buffer, 0);
            return phash;
        }

        public static byte[] ConvertPHashToBuffer(ulong phash)
        {
            var buffer = BitConverter.GetBytes(phash);
            return buffer;
        }

        public static byte[] ConvertMatToBuffer(Mat mat)
        {
            var buffer = new byte[mat.Rows * mat.Cols];
            mat.GetArray(0, 0, buffer);
            return buffer;
        }

        public static Mat ConvertBufferToMat(byte[] buffer)
        {
            if (buffer.Length < 32)
            {
                return new Mat();
            }

            var mat = new Mat(buffer.Length / 32, 32, MatType.CV_8U);
            mat.SetArray(0, 0, buffer);
            return mat;
        }
    }
}
