using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        private static readonly CudaBFMatcher _bfMatcher = new CudaBFMatcher(DistanceType.Hamming);

        public static bool ComputeDescriptors(byte[] data, out GpuMat matdescriptors)
        {
            matdescriptors = null;
            if (data == null || data.Length == 0)
            {
                return false;
            }

            using (var orb = new CudaORBDetector(AppConsts.MaxOrbPointsInImage))
            {
                try
                {
                    using (var matsource = new Mat())                    
                    {
                        CvInvoke.Imdecode(data, ImreadModes.Grayscale, matsource);
                        if (matsource.Width == 0 || matsource.Height == 0)
                        {
                            return false;
                        }

                        using (var gpumat = new GpuMat(matsource))
                        {
                            const double fsample = 1024.0 * 768.0;
                            var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                            using (var mat = new Mat())
                            {
                                CvInvoke.Resize(matsource, mat, Size.Empty, fx, fx, Inter.Cubic);
                                matdescriptors = new GpuMat();
                                using (var keypoints = new VectorOfKeyPoint())
                                {
                                    orb.DetectAndCompute(gpumat, null, keypoints, matdescriptors, false);
                                    if (matdescriptors.Size.Width != 32 || matdescriptors.Size.Height == 0)
                                    {
                                        throw new Exception();
                                    }

                                    while (matdescriptors.Size.Height > AppConsts.MaxOrbPointsInImage)
                                    {
                                        matdescriptors = matdescriptors.RowRange(0, AppConsts.MaxOrbPointsInImage - 1);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    matdescriptors = null;
                    return false;
                }
            }

            return true;
        }

        public static float GetSim(GpuMat x, GpuMat y)
        {
            if (x.Size.Height == 0)
            {

            }

            using (var bfmatches = new VectorOfDMatch())
            {
                _bfMatcher.Match(x, y, bfmatches);
                var matches = bfmatches.ToArray();
                var distances = matches
                    .Where(e => e.Distance < AppConsts.MaxHammingDistance)
                    .Sum(e => AppConsts.MaxHammingDistance - e.Distance);

                var sim = (float)distances / x.Size.Height;
                return sim;
            }
        }

        public static GpuMat ConvertToMatDescriptors(byte[] descriptors)
        {
            var rows = descriptors.Length / 32;
            IntPtr data = Marshal.AllocHGlobal(descriptors.Length);
            Marshal.Copy(descriptors, 0, data, descriptors.Length);
            using (var mat = new Mat(rows, 32, DepthType.Cv8U, 1, data, 32))
            {
                var gpumat = new GpuMat(mat);
                return gpumat;
            }
        }

        public static byte[] ConvertToByteDescriptors(GpuMat gpumat)
        {
            using (var mat = new Mat())
            {
                gpumat.Download(mat);
                var descriptors = new byte[gpumat.Size.Width * gpumat.Size.Height];
                Marshal.Copy(mat.DataPointer, descriptors, 0, descriptors.Length);
                return descriptors;
            }
        }
    }
}
