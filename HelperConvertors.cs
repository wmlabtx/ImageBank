using OpenCvSharp;
using System;

namespace ImageBank
{
    public static class HelperConvertors
    {
        public static string TimeIntervalToString(TimeSpan ts)
        {
            if (ts.TotalDays >= 2.0)
                return $"{ts.TotalDays:F0} days";

            if (ts.TotalDays >= 1.0)
                return $"{ts.TotalDays:F0} day";

            if (ts.TotalHours >= 2.0)
                return $"{ts.TotalHours:F0} hours";

            if (ts.TotalHours >= 1.0)
                return $"{ts.TotalHours:F0} hour";

            if (ts.TotalMinutes >= 2.0)
                return $"{ts.TotalMinutes:F0} minutes";

            if (ts.TotalMinutes >= 1.0)
                return $"{ts.TotalMinutes:F0} minute";

            return $"{ts.TotalSeconds:F0} seconds";
        }

        public static string SizeToString(long size)
        {
            var str = $"{size} b";
            if (size < 1024)
                return str;

            var ksize = (double)size / 1024;
            str = $"{ksize:F1} Kb";
            if (ksize < 1024)
                return str;

            ksize /= 1024;
            str = $"{ksize:F2} Mb";
            return str;
        }

        public static byte[] ConvertMatToBuffer(Mat mat)
        {
            var buffer = new byte[mat.Rows * mat.Cols];
            mat.GetArray(0, 0, buffer);
            return buffer;
        }

        public static Mat ConvertBufferToMat(byte[] buffer)
        {
            if (buffer.Length < 32)
            {
                return new Mat();
            }

            var mat = new Mat(buffer.Length / 32, 32, MatType.CV_8U);
            mat.SetArray(0, 0, buffer);
            return mat;
        }
    }
}
