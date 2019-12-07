using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Linq;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        private static readonly CudaBFMatcher _bfMatcher = new CudaBFMatcher(DistanceType.Hamming);

        static HelperDescriptors()
        {
            if (!CudaInvoke.HasCuda)
            {
                throw new Exception();
            }
        }

        public static bool ComputeDescriptors(byte[] data, out GpuMat descriptors)
        {
            descriptors = null;
            if (data == null || data.Length == 0)
            {
                return false;
            }

            //try
            //{
                using (var matinput = new Mat())
                {
                    CvInvoke.Imdecode(data, ImreadModes.Grayscale, matinput);
                    if (matinput.Width == 0 || matinput.Height == 0)
                    {
                        return false;
                    }

                    using (var orb = new CudaORBDetector(AppConsts.MaxDescriptorsInImage))
                    {
                        const double fsample = 800.0 * 600.0;
                        var fx = Math.Sqrt(fsample / (matinput.Width * matinput.Height));
                        using (var mat = new Mat())
                        {
                            CvInvoke.Resize(matinput, mat, Size.Empty, fx, fx, Inter.Cubic);
                            descriptors = new GpuMat();
                            using (var keypoints = new VectorOfKeyPoint())
                            using (var gpumat = new GpuMat(mat))
                            {
                                orb.DetectAndCompute(gpumat, null, keypoints, descriptors, false);
                                if (descriptors.Size.Width != 32 || descriptors.Size.Height == 0)
                                {
                                    throw new Exception();
                                }

                                while (descriptors.Size.Height > AppConsts.MaxDescriptorsInImage)
                                {
                                    descriptors = descriptors.RowRange(0, AppConsts.MaxDescriptorsInImage - 1);
                                }
                            }
                        }
                    }
                }
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        public static float GetSim(GpuMat x, GpuMat y)
        {
            if (x.Size.Height == 0)
            {
                return 0f;
            }

            using (var bfmatches = new VectorOfDMatch())
            {
                _bfMatcher.Match(x, y, bfmatches);
                var matches = bfmatches.ToArray();
                var dsum = matches.Where(e => e.Distance < 64).Sum(e => 64 - e.Distance);
                var sim = dsum / x.Size.Height;
                return sim;
            }
        }
    }
}
