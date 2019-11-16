using OpenCvSharp;
using OpenCvSharp.ImgHash;
using System;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        private static readonly PHash _phash = PHash.Create();

        public static bool ComputeDescriptors(byte[] data, out ulong phash)
        {
            phash = 0;
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
                        phash = ConvertToPHash(buffer);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static int Distance(ulong x, ulong y)
        {
            var distance = Intrinsic.PopCnt(x ^ y);
            return distance;
        }

        public static ulong ConvertToPHash(byte[] buffer)
        {
            var phash = BitConverter.ToUInt64(buffer, 0);
            return phash;
        }

        public static byte[] ConvertToBuffer(ulong phash)
        {
            var buffer = BitConverter.GetBytes(phash);
            return buffer;
        }
    }
}
