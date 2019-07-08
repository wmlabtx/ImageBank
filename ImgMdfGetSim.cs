using OpenCvSharp;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private float GetSim(Mat x, Mat y)
        {
            var bfMatches = _bfMatcher.Match(x, y);
            var sum = 0;
            for (var i = 0; i < bfMatches.Length; i++)
            {
                var distance = (int)bfMatches[i].Distance;
                if (distance < AppConsts.MaxHammingDistance)
                {
                    sum += AppConsts.MaxHammingDistance - distance;
                }
            }

            return (float)sum / x.Rows;
        }
    }
}