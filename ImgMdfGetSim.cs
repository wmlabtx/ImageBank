using OpenCvSharp;
using System;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private float GetSim(Mat x, Mat y)
        {
            var bfMatches = _bfMatcher.Match(x, y);
            float sum = 0f;
            for (var i = 0; i < bfMatches.Length; i++)
            {
                var distance = bfMatches[i].Distance;
                sum += (float)(1.0 / Math.Pow(Math.Exp(distance / AppConsts.MaxHammingDistance), 2.0));
            }

            return sum / x.Rows;

            /*
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
            */
        }
    }
}