using OpenCvSharp;
using System;
using System.Collections;
using System.Linq;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        public static bool ComputeVector(byte[] data, out ulong[] vector)
        {
            vector = null;
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

                    using (var orb = ORB.Create(AppConsts.MaxDescriptorsInImage))
                    {

                        var orbs = new Mat();
                        orb.DetectAndCompute(matinput, null, out _, orbs);
                        if (orbs.Cols != 32|| orbs.Rows == 0)
                        {
                            throw new Exception();
                        }

                        while (orbs.Height > AppConsts.MaxDescriptorsInImage)
                        {
                            orbs = orbs.RowRange(0, AppConsts.MaxDescriptorsInImage);
                        }

                        var counter = 0;
                        var bstat = new int[256];
                        var buffer = ConvertMatToBuffer(orbs);
                        var offset = 0;
                        var descriptor = new byte[32];
                        while (offset < buffer.Length)
                        {
                            Buffer.BlockCopy(buffer, offset, descriptor, 0, 32);
                            var ba = new BitArray(descriptor);
                            for (var i = 0; i < 256; i++)
                            {
                                if (ba[i])
                                {
                                    bstat[i]++;
                                }
                            }

                            counter++;
                            offset += 32;
                        }

                        var mid = counter / 2;
                        var result = new byte[32];
                        var ib = 0;
                        byte mask = 0x01;
                        for (var i = 0; i < 256; i++)
                        {
                            if (bstat[i] > mid)
                            {
                                result[ib] |= mask;
                            }

                            if (mask == 0x80)
                            {
                                ib++;
                                mask = 0x01;
                            }
                            else
                            {
                                mask <<= 1;
                            }
                            
                        }

                        vector = ConvertBufferToVector(result);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static int GetDistance(ulong[] x, ulong[] y)
        {
            if (x.Length == 0 || y.Length == 0)
            {
                return 256;
            }

            var distance = 0;
            for (var i = 0; i < 4; i++)
            {
                distance += Intrinsic.PopCnt(x[i] ^ y[i]);
            }

            return distance;
        }

        public static byte[] ConvertVectorToBuffer(ulong[] vector)
        {
            if (vector == null || vector.Length == 0)
            {
                return new byte[0];
            }


            var buffer = new byte[32];
            Buffer.BlockCopy(vector, 0, buffer, 0, 32);
            return buffer;
        }

        public static ulong[] ConvertBufferToVector(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return new ulong[0];
            }

            var vector = new ulong[4];
            Buffer.BlockCopy(buffer, 0, vector, 0, 32);
            return vector;
        }

        public static byte[] ConvertMatToBuffer(Mat mat)
        {
            var buffer = new byte[mat.Rows * mat.Cols];
            mat.GetArray(0, 0, buffer);
            return buffer;
        }
    }
}
