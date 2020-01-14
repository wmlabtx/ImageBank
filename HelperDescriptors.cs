using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;

namespace ImageBank
{
    public class HelperDescriptors
    {
        public static bool ComputeDescriptors(byte[] jpgdata, out float[] descriptors)
        {
            descriptors = null;
            if (jpgdata == null || jpgdata.Length == 0)
            {
                return false; 
            }

            try
            {
                using (var matsource = Mat.FromImageData(jpgdata, ImreadModes.Grayscale))
                {
                    if (matsource.Width == 0 || matsource.Height == 0)
                    {
                        return false;
                    }

                    const double fsample = 512.0 * 512.0;
                    var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                    using (var mat = matsource.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                    {
                        using (var sift = SIFT.Create())
                        {
                            var keypoints = sift.Detect(mat);
                            if (keypoints.Length == 0)
                            {
                                return false;
                            }

                            using (var matdescriptors = new Mat())
                            {
                                sift.Compute(mat, ref keypoints, matdescriptors);
                                if (matdescriptors.Rows == 0)
                                {
                                    return false;
                                }

                                using (var bestlabels = new Mat())
                                using (var clusters = new Mat())
                                {
                                    Cv2.Kmeans(
                                        matdescriptors,
                                        1,
                                        bestlabels,
                                        new TermCriteria(CriteriaType.Eps, 100, 0.1),
                                        100,
                                        KMeansFlags.PpCenters,
                                        clusters);

                                    clusters.GetArray(out descriptors);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                descriptors = null;
                return false;
            }

            return true;
        }

        public static float GetDistance(IReadOnlyList<float> x, int xoffset, IReadOnlyList<float> y, int yoffset)
        {
            var sum = 0f;
            for (var i = 0; i < 128; i++)
            {
                sum += (x[xoffset + i] - y[yoffset + i]) * (x[xoffset + i] - y[yoffset + i]);
            }

            var distance = (float)Math.Sqrt(sum);
            return distance;
        }
    }
}
