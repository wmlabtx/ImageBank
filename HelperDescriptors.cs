using OpenCvSharp;
using System;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        public static bool ComputeDescriptors(byte[] data, out Mat matdescriptors)
        {
            matdescriptors = null;
            if (data == null || data.Length == 0)
            {
                return false;
            }

            var orb = ORB.Create(AppConsts.MaxOrbPointsInImage);
            try
            {
                using (var mat = Mat.FromImageData(data, ImreadModes.Grayscale))
                {
                    if (mat.Width == 0 || mat.Height == 0)
                    {
                        return false;
                    }

                    matdescriptors = new Mat();
                    orb.DetectAndCompute(mat, null, out _, matdescriptors);
                    if (matdescriptors.Cols != 32 || matdescriptors.Rows == 0)
                    {
                        matdescriptors.Dispose();
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                matdescriptors = null;
                return false;
            }

            return true;
        }

        public static Mat ConvertToMatDescriptors(byte[] descriptors)
        {
            var rows = descriptors.Length / 32;
            var mat = new Mat(rows, 32, MatType.CV_8UC1, descriptors);
            return mat;
        }

        public static byte[] ConvertToByteDescriptors(Mat mat)
        {
            var descriptors = new byte[mat.Rows * mat.Cols];
            mat.GetArray(0, 0, descriptors);
            return descriptors;
        }
    }
}
