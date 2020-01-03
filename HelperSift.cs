using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;

namespace ImageBank
{
    public static class HelperSift
    {
        public static byte[] ConvertToBuffer(Mat mat)
        {
            mat.GetArray<float>(out var fbuffer);
            var buffer = new byte[fbuffer.Length * sizeof(float)];
            Buffer.BlockCopy(fbuffer, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        public static byte[] ConvertToBuffer(float[] fbuffer)
        {
            var buffer = new byte[fbuffer.Length * sizeof(float)];
            Buffer.BlockCopy(fbuffer, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        public static Mat ConvertToMat(byte[] buffer)
        {
            var fbuffer = new float[buffer.Length / sizeof(float)];
            Buffer.BlockCopy(buffer, 0, fbuffer, 0, buffer.Length);
            var mat = new Mat(fbuffer.Length / 128, 128, MatType.CV_32FC1);
            mat.SetArray(fbuffer);
            return mat;
        }

        public static float[] ConvertToFloat(byte[] buffer)
        {
            var fbuffer = new float[buffer.Length / sizeof(float)];
            Buffer.BlockCopy(buffer, 0, fbuffer, 0, buffer.Length);
            return fbuffer;
        }

        public static bool ComputeDescriptors(byte[] jpgdata, out Mat descriptors)
        {
            descriptors = null;
            if (jpgdata == null || jpgdata.Length == 0)
            {
                return false;
            }

            using (var sift = SIFT.Create(AppConsts.MaxDescriptorsInImage))
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
                            descriptors = new Mat();
                            sift.DetectAndCompute(mat, null, out _, descriptors);
                            if (descriptors.Rows == 0)
                            {
                                descriptors = null;
                                return false;
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
    }
}