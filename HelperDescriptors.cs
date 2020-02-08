using System;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        public static bool ComputeHashes(byte[] jpgdata, out uint[] hashes)
        {
            hashes = null;
            try {
                using (var mat = Mat.FromImageData(jpgdata, ImreadModes.Grayscale)) {
                    if (mat.Width == 0 || mat.Height == 0) {
                        return false;
                    }

                    using (var sift = SIFT.Create(2000)) {
                        var keypoints = sift.Detect(mat);
                        if (keypoints.Length == 0) {
                            return false;
                        }

                        using (var matdescriptors = new Mat()) {
                            sift.Compute(mat, ref keypoints, matdescriptors);
                            if (matdescriptors.Rows == 0) {
                                return false;
                            }

                            matdescriptors.GetArray(out float[] data);
                            var length = Math.Min(data.Length, 2000 * 128);
                            hashes = new uint[length / 128];
                            var descriptor = new byte[128];
                            var offsrc = 0;
                            var offdst = 0;
                            var offhsh = 0;
                            while (offsrc < length) {
                                descriptor[offdst] = (byte)((int)data[offsrc] >> 6);
                                offsrc++;
                                offdst++;
                                if (offdst == 128) {
                                    offdst = 0;
                                    hashes[offhsh] = Crc32C.Crc32CAlgorithm.Compute(descriptor);
                                    offhsh++;
                                }
                            }

                            Array.Sort(hashes);
                        }
                    }
                }
            }
            catch (Exception) {
                hashes = null;
                return false;
            }

            return true;
        }

        public static int ComputeMatch(uint[] x, uint[] y) {
            var match = 0;
            var i = 0;
            var j = 0;
            while (i < x.Length && j < y.Length) {
                if (x[i] == y[j]) {
                    match++;
                    i++;
                    j++;
                }
                else {
                    if (x[i] < y[j]) {
                        i++;
                    }
                    else {
                        j++;
                    }
                }
            }

            return match;
        }
    }
}
