using Microsoft.VisualBasic.FileIO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Quality;
using OpenCvSharp.XFeatures2D;
using System;
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
        private static readonly QualityBRISQUE brisque = 
            QualityBRISQUE.Create(AppConsts.BrisqueModel, AppConsts.BrisqueRange);

        #region Encryption

        private const string PasswordSole = "{mzx}";
        private static readonly byte[] AES_IV = { 
            0xE1, 0xD9, 0x94, 0xE6, 0xE6, 0x43, 0x39, 0x34,
            0x33, 0x0A, 0xCC, 0x9E, 0x7D, 0x66, 0x97, 0x16
        };

        private static Aes CreateAes(string password)
        {
            using (var hash256 = SHA256.Create()) {
                var passwordwithsole = string.Concat(password, PasswordSole);
                var passwordbuffer = Encoding.ASCII.GetBytes(passwordwithsole);
                var passwordkey256 = hash256.ComputeHash(passwordbuffer);
                var aes = Aes.Create();
                aes.KeySize = 256;
                aes.Key = passwordkey256;
                aes.BlockSize = 128;
                aes.IV = AES_IV;
                aes.Mode = CipherMode.CBC;
                return aes;
            }
        }

        public static byte[] Encrypt(byte[] array, string password)
        {
            Contract.Requires(array != null);
            Contract.Requires(password != null);
            var aes = CreateAes(password);
            try {
                using (var ms = new MemoryStream()) {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                        cs.Write(array, 0, array.Length);
                    }

                    return ms.ToArray();
                }
            }
            finally {
                aes.Dispose();
            }
        }

        public static byte[] Decrypt(byte[] array, string password)
        {
            Contract.Requires(array != null);
            Contract.Requires(password != null);
            var aes = CreateAes(password);
            try {
                
                try {
                    using (var ms = new MemoryStream(array)) {
                        byte[] decoded;
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read)) {
                            var count = cs.Read(array, 0, array.Length);
                            decoded = new byte[count];
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.Read(decoded, 0, count);
                        }

                        return decoded;
                    }
                }
                catch (CryptographicException) {
                    return null;
                }
            }
            finally {
                aes.Dispose();
            }
        }

        #endregion

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
            var filename = $"{AppConsts.PathCollection}{path}\\{name}{AppConsts.MzxExtension}";
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
            out float quality,
            out Bitmap bitmap,
            out string checksum,
            out bool needwrite)
        {
            imgdata = null;
            quality = 0;
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
                !extension.Equals(AppConsts.JpegExtension, StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(AppConsts.MzxExtension, StringComparison.OrdinalIgnoreCase)
                ) {
                return false;
            }

            imgdata = File.ReadAllBytes(filename);
            if (imgdata == null || imgdata.Length == 0) {
                return false;
            }

            if (extension.Equals(AppConsts.MzxExtension, StringComparison.OrdinalIgnoreCase)) {
                var password = GetName(filename);
                imgdata = Decrypt(imgdata, password);
                if (imgdata == null || imgdata.Length == 0) {
                    return false;
                }

                extension = AppConsts.JpgExtension;
            }
            else {
                needwrite = true;
            }

            Mat mat = null;
            Mat matgray = null;
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

                matgray = Cv2.ImDecode(imgdata, ImreadModes.Grayscale);
                var scalar = brisque.Compute(matgray);
                quality = scalar.Val0 < 0.1f ? 0f : (float)(100f / scalar.Val0);
            }
            catch {
                imgdata = null;
                quality = 0;
                bitmap = null;
                checksum = null;
                needwrite = false;
                return false;
            }
            finally {
                mat?.Dispose();
                matgray?.Dispose();
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

        public static byte[] ReadEncryptedData(string filename)
        {
            if (!File.Exists(filename)) {
                return null;
            }

            var imgdata = File.ReadAllBytes(filename);
            if (imgdata == null || imgdata.Length == 0) {
                return null;
            }

            var password = GetName(filename);
            imgdata = Decrypt(imgdata, password);
            return imgdata;
        }

        public static void WriteEncryptedData(string filename, byte[] imgdata)
        {
            var directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            var password = GetName(filename);
            imgdata = Encrypt(imgdata, password);
            File.WriteAllBytes(filename, imgdata);
        }

        #endregion

        #region Descriptors

        public static bool GetImageDescriptors(byte[] imgdata, out uint[] descriptors)
        {
            descriptors = null;
            try {
                using (var mat = Mat.FromImageData(imgdata, ImreadModes.Grayscale)) {
                    if (mat.Width == 0 || mat.Height == 0) {
                        return false;
                    }

                    using (var sift = SIFT.Create(AppConsts.MaxDescriptorsInImage)) {
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
                            var length = Math.Min(data.Length, AppConsts.MaxDescriptorsInImage * 128);
                            descriptors = new uint[length / 128];
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
                                    descriptors[offhsh] = Crc32C.Crc32CAlgorithm.Compute(descriptor);
                                    offhsh++;
                                }
                            }

                            Array.Sort(descriptors);
                        }
                    }
                }
            }
            catch (Exception) {
                descriptors = null;
                return false;
            }

            return true;
        }

        public static byte[] DescriptorsToBuffer(uint[] array)
        {
            Contract.Requires(array != null);
            var buffer = new byte[array.Length * sizeof(uint)];
            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        public static uint[] BufferToDescriptors(byte[] buffer)
        {
            Contract.Requires(buffer != null);
            var array = new uint[buffer.Length / sizeof(uint)];
            Buffer.BlockCopy(buffer, 0, array, 0, buffer.Length);
            return array;
        }

        #endregion

        #region Quality
        public static float ComputeQuality(string jpgfilename)
        {
            var jpgdata = File.ReadAllBytes(jpgfilename);
            var mat = Mat.FromImageData(jpgdata, ImreadModes.Grayscale);
            var scalar = brisque.Compute(mat);
            return (float)(100f / scalar.Val0);
        }

        #endregion
    }
}
