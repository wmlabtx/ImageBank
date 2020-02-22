using Microsoft.VisualBasic.FileIO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ImgHash;
using OpenCvSharp.Quality;
using OpenCvSharp.XFeatures2D;
using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageBank
{
    public static class Helper
    {
        #region DeleteToRecycleBin

        public static void DeleteToRecycleBin(string filename)
        {
            try {
                if (File.Exists(filename)) {
                    File.SetAttributes(filename, FileAttributes.Normal);
                    FileSystem.DeleteFile(filename, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
            }
            catch (UnauthorizedAccessException) {
            }
            catch (IOException) {
            }
        }

        #endregion

        #region Hash

        public static string ComputeHash3250(byte[] array)
        {
            var digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
            var sb = new StringBuilder(50);
            using (var sha256 = SHA256.Create()) {
                var hash = sha256.ComputeHash(array);
                var index = 0;
                var posbyte = hash.Length - 1;
                var posbit = 0;
                for (var poschar = 0; poschar < 50; poschar++) {
                    index = 0;
                    for (var i = 0; i < 5; i++) {
                        if ((hash[posbyte] & (1 << posbit)) != 0) {
                            index |= 1 << i;
                        }

                        posbit++;
                        if (posbit >= 8) {
                            posbyte--;
                            posbit = 0;
                        }
                    }

                    sb.Append(digits[index]);
                }
            }

            return sb.ToString();
        }

        #endregion

        #region TimeIntervalToString
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

        #endregion

        #region SizeToString

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

        #endregion

        #region Strings

        public static string GetFileName(string name, string path)
        {
            var filename = $"{AppConsts.PathCollection}{path}\\{name}{AppConsts.JpgExtension}";
            return filename;
        }

        public static string GetName(string filename)
        {
            var name = Path
                .GetFileNameWithoutExtension(filename)
                .ToUpperInvariant();

            return name;
        }

        public static string GetPath(string filename)
        {
            var path = Path
                .GetDirectoryName(filename)
                .Substring(AppConsts.PathCollection.Length)
                .ToUpperInvariant();

            return path;
        }

        public static string GetExtension(string filename)
        {
            var name = Path
                .GetExtension(filename)
                .ToUpperInvariant();

            return name;
        }

        #endregion

        #region Image

        public static bool GetImageDataFromFile(
            string filename,
            out byte[] imgdata,
            out Bitmap bitmap,
            out string checksum,
            out bool needwrite)
        {
            imgdata = null;
            bitmap = null;
            checksum = null;
            needwrite = false;
            if (!File.Exists(filename)) {
                return false;
            }

            var extension = GetExtension(filename);
            if (string.IsNullOrEmpty(extension)) {
                return false;
            }

            if (
                !extension.Equals(AppConsts.PngExtension, StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(AppConsts.BmpExtension, StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(AppConsts.WebpExtension, StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(AppConsts.JpgExtension, StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(AppConsts.JpegExtension, StringComparison.OrdinalIgnoreCase)
                ) {
                return false;
            }

            imgdata = File.ReadAllBytes(filename);
            if (imgdata == null || imgdata.Length == 0) {
                return false;
            }

            Mat mat = null;
            try {
                mat = Cv2.ImDecode(imgdata, ImreadModes.AnyColor);

                const float fmax = 6000f * 4000f;
                var fx = (float)Math.Sqrt(fmax / (mat.Width * mat.Height));
                if (fx < 1f) {
                    mat = mat.Resize(OpenCvSharp.Size.Zero, fx, fx, InterpolationFlags.Cubic);
                    var iep = new ImageEncodingParam(ImwriteFlags.JpegQuality, 95);
                    Cv2.ImEncode(AppConsts.JpgExtension, mat, out imgdata, iep);
                    needwrite = true;
                }
                else {
                    if (!extension.Equals(AppConsts.JpgExtension, StringComparison.OrdinalIgnoreCase) &&
                        !extension.Equals(AppConsts.JpegExtension, StringComparison.OrdinalIgnoreCase)) {
                        var iep = new ImageEncodingParam(ImwriteFlags.JpegQuality, 95);
                        Cv2.ImEncode(AppConsts.JpgExtension, mat, out imgdata, iep);
                        needwrite = true;
                    }
                }

                bitmap = mat.ToBitmap();
                checksum = ComputeHash3250(imgdata);
            }
            catch {
                imgdata = null;
                bitmap = null;
                checksum = null;
                needwrite = false;
                return false;
            }
            finally {
                mat?.Dispose();
            }

            return true;
        }

        public static ImageSource ImageSourceFromBitmap(Bitmap bitmap)
        {
            Contract.Requires(bitmap != null);
            var handle = bitmap.GetHbitmap();
            try {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally {
                NativeMethods.DeleteObject(handle);
            }
        }

        #endregion

        #region CleanupDirectories
        public static void CleanupDirectories(string startLocation, IProgress<string> progress)
        {
            Contract.Requires(progress != null);
            foreach (var directory in Directory.GetDirectories(startLocation)) {
                Helper.CleanupDirectories(directory, progress);
                if (Directory.GetFiles(directory).Length != 0 || Directory.GetDirectories(directory).Length != 0) {
                    continue;
                }

                progress.Report($"{directory} deleting...");
                try {
                    Directory.Delete(directory, false);
                }
                catch (IOException) {
                }
            }
        }

        #endregion

        #region EncryptedData

        public static byte[] ReadData(string filename)
        {
            if (!File.Exists(filename)) {
                return null;
            }

            var imgdata = File.ReadAllBytes(filename);
            if (imgdata == null || imgdata.Length == 0) {
                return null;
            }

            return imgdata;
        }

        public static void WriteData(string filename, byte[] imgdata)
        {
            var directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(filename, imgdata);
        }

        #endregion

        #region Descriptors

        public static bool GetPhashOrbv(byte[] imgdata, out ulong phash, out ulong[] orbv)
        {
            phash = 0;
            orbv = null;
            try {
                using (var mat = Mat.FromImageData(imgdata, ImreadModes.Grayscale)) {
                    if (mat.Width == 0 || mat.Height == 0) {
                        return false;
                    }

                    using (var phashmat = new Mat())
                    using (var phashcalc = PHash.Create()) {
                        phashcalc.Compute(mat, phashmat);
                        if (phashmat.Cols != 8 || phashmat.Rows != 1) {
                            return false;
                        }

                        phashmat.GetArray<byte>(out var phasharray);
                        phash = BitConverter.ToUInt64(phasharray, 0);
                    }

                    using (var descriptors = new Mat())
                    using (var orbcalc = ORB.Create()) {
                        orbcalc.DetectAndCompute(mat, null, out _, descriptors);
                        if (descriptors.Cols != 32 || descriptors.Rows == 0) {
                            return false;
                        }

                        descriptors.GetArray<byte>(out var buffer);
                        var counter = 0;
                        var bstat = new int[256];
                        var offset = 0;
                        var descriptor = new byte[32];
                        while (offset < buffer.Length) {
                            Buffer.BlockCopy(buffer, offset, descriptor, 0, 32);
                            var ba = new BitArray(descriptor);
                            for (var i = 0; i < 256; i++) {
                                if (ba[i]) {
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
                        for (var i = 0; i < 256; i++) {
                            if (bstat[i] > mid) {
                                result[ib] |= mask;
                            }

                            if (mask == 0x80) {
                                ib++;
                                mask = 0x01;
                            }
                            else {
                                mask <<= 1;
                            }

                        }

                        orbv = BufferToVector(result);
                    }
                }
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        public static int GetDistance(ulong x, ulong y)
        {
            var distance = Intrinsic.PopCnt(x ^ y);
            return distance;
        }

        public static int GetDistance(ulong[] x, ulong[] y)
        {
            Contract.Requires(x != null);
            Contract.Requires(y != null);
            Contract.Requires(x.Length == 4);
            Contract.Requires(y.Length == 4);
            var distance = 0;
            for (var i = 0; i < 4; i++) {
                distance += Intrinsic.PopCnt(x[i] ^ y[i]);
            }

            return distance;
        }

        public static byte[] VectorToBuffer(ulong[] vector)
        {
            Contract.Requires(vector != null);
            Contract.Requires(vector.Length == 4);
            var buffer = new byte[32];
            Buffer.BlockCopy(vector, 0, buffer, 0, 32);
            return buffer;
        }

        public static ulong[] BufferToVector(byte[] buffer)
        {
            Contract.Requires(buffer != null && buffer.Length == 32);
            var vector = new ulong[4];
            Buffer.BlockCopy(buffer, 0, vector, 0, 32);
            return vector;
        }

        #endregion
    }
}
