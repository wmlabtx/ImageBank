using OpenCvSharp;
using System;
using System.Runtime.InteropServices;

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

        public static ulong[] ConvertToUlongs(byte[] buffer)
        {
            var ulongs = new ulong[buffer.Length / sizeof(ulong)];
            Buffer.BlockCopy(buffer, 0, ulongs, 0, buffer.Length);
            return ulongs;
        }

        public static byte[] ConvertToBytes(ulong[] ulongs)
        {
            var buffer = new byte[ulongs.Length * sizeof(ulong)];
            Buffer.BlockCopy(ulongs, 0, buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
