using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
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

        public static byte[] ConvertMatToBuffer(GpuMat gpumat)
        {
            using (var mat = new Mat())
            {
                gpumat.Download(mat);
                var descriptors = new byte[gpumat.Size.Width * gpumat.Size.Height];
                Marshal.Copy(mat.DataPointer, descriptors, 0, descriptors.Length);
                return descriptors;
            }
        }

        public static GpuMat ConvertBufferToMat(byte[] buffer)
        {
            if (buffer.Length < 32)
            {
                return new GpuMat();
            }

            var rows = buffer.Length / 32;
            IntPtr data = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, data, buffer.Length);
            using (var mat = new Mat(rows, 32, DepthType.Cv8U, 1, data, 32))
            {
                var gpumat = new GpuMat(mat);
                return gpumat;
            }
        }
    }
}
