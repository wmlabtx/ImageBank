using OpenCvSharp;
using System;
using System.Collections;
using System.Linq;

namespace ImageBank
{
    public static class HelperDescriptors
    {
        private static readonly BFMatcher _bfmatcher = new BFMatcher(NormTypes.Hamming, true);

        public static bool ComputeDescriptors(byte[] data, out Mat descriptors)
        {
            descriptors = null;
            if (data == null || data.Length == 0)
            {
                return false;
            }

            using (var orb = ORB.Create(10000))
            {
                try
                {
                    using (var matsource = Mat.FromImageData(data, ImreadModes.Grayscale))
                    {
                        if (matsource.Width == 0 || matsource.Height == 0)
                        {
                            return false;
                        }

                        const double fsample = 800.0 * 600.0;
                        var fx = Math.Sqrt(fsample / (matsource.Width * matsource.Height));
                        using (var mat = matsource.Resize(Size.Zero, fx, fx, InterpolationFlags.Cubic))
                        using (var matdescriptors = new Mat())
                        {
                            orb.DetectAndCompute(mat, null, out _, matdescriptors);
                            if (matdescriptors.Rows == 0)
                            {
                                throw new Exception();
                            }

                            var buffer = HelperConvertors.ConvertMatToBuffer(matdescriptors);
                            using (var fdescriptors = new Mat(matdescriptors.Rows, 256, MatType.CV_32FC1))
                            {
                                fdescriptors.SetTo(0f);
                                var off = 0;
                                var row = 0;
                                var col = 0;
                                var bit = 7;
                                while (off < buffer.Length)
                                {
                                    do
                                    {
                                        do
                                        {
                                            if ((buffer[off] & (1 << bit)) > 0)
                                            {
                                                fdescriptors.Set(row, col, 1f);
                                            }

                                            bit--;
                                            col++;
                                        }
                                        while (bit >= 0);
                                        bit = 7;
                                        off++;
                                    }
                                    while (col < 256);
                                    col = 0;
                                    row++;
                                }

                                using (var bestlabels = new Mat())
                                using (var clusters = new Mat())
                                {
                                    Cv2.Kmeans(
                                        data: fdescriptors,
                                        k: AppConsts.MaxClustersInImage,
                                        bestLabels: bestlabels,
                                        criteria:
                                            new TermCriteria(type: CriteriaType.Eps | CriteriaType.MaxIter, maxCount: 1000, epsilon: 0.01),
                                        attempts: 10,
                                        flags: KMeansFlags.PpCenters,
                                        centers: clusters);

                                    descriptors = new Mat(clusters.Rows, 32, MatType.CV_8UC1);
                                    row = 0;
                                    bit = 7;
                                    var col256 = 0;
                                    var col32 = 0;                                        
                                    byte bcc = 0x00;
                                    do
                                    {
                                        do
                                        {
                                            do
                                            {
                                                var v = clusters.At<float>(row, col256);
                                                if (v > 0.5f)
                                                {
                                                    bcc |= (byte)(1 << bit);
                                                }

                                                bit--;
                                                col256++;
                                            }
                                            while (bit >= 0);
                                            bit = 7;
                                            descriptors.Set(row, col32, bcc);
                                            bcc = 0x00;
                                            col32++;
                                        }
                                        while (col32 < 32);
                                        col32 = 0;
                                        col256 = 0;
                                        row++;
                                    }
                                    while (row < clusters.Rows);
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
            if (dmatch.Length > 0)
            {
                var ds = dmatch
                    .Where(e => e.Distance < 64)
                    .OrderBy(e => e.Distance)
                    .Select(e => e.Distance)
                    .ToArray();

                var sd = 0f;
                var k = 1f;
                foreach (var d in ds)
                {
                    sd += (64 - d) * k;
                    k /= 2f;
                }

                var sim = sd / 2f;
                return sim;
            }
            else
            {
                return 0f;
            }
        }
    }
}
