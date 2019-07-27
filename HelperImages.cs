using OpenCvSharp;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageBank
{
    public static class HelperImages
    {
        public static bool GetJpgFromFile(string filename, out byte[] data)
        {
            data = null;
            if (!File.Exists(filename))
            {
                return false;
            }

            var extension = Path.GetExtension(filename);
            if (
                !extension.Equals(AppConsts.PngExtension, StringComparison.InvariantCultureIgnoreCase) &&
                !extension.Equals(AppConsts.BmpExtension, StringComparison.InvariantCultureIgnoreCase) &&
                !extension.Equals(AppConsts.WebpExtension, StringComparison.InvariantCultureIgnoreCase) &&
                !extension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase) &&
                !extension.Equals(AppConsts.JpegExtension, StringComparison.InvariantCultureIgnoreCase) &&
                !extension.Equals(AppConsts.DatExtension, StringComparison.InvariantCultureIgnoreCase)
               )
            {
                return false;
            }

            data = File.ReadAllBytes(filename);
            if (data == null || data.Length == 0)
            {
                return false;
            }
                
            if (extension.Equals(AppConsts.DatExtension))
            {
                var password = HelperPath.GetPassword(filename);
                data = HelperEncrypting.Decrypt(data, password);
                if (data == null || data.Length == 0)
                {
                    return false;
                }
            }
            else
            {
                if (
                    extension.Equals(AppConsts.PngExtension, StringComparison.InvariantCultureIgnoreCase) ||
                    extension.Equals(AppConsts.BmpExtension, StringComparison.InvariantCultureIgnoreCase) ||
                    extension.Equals(AppConsts.WebpExtension, StringComparison.InvariantCultureIgnoreCase)
                   )
                {
                    using (var mat = Cv2.ImDecode(data, ImreadModes.AnyColor))
                    {
                        try
                        {
                            var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
                            if (!extension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase) &&
                                !extension.Equals(AppConsts.JpegExtension, StringComparison.InvariantCultureIgnoreCase))
                            {
                                var iep = new ImageEncodingParam(ImwriteFlags.JpegQuality, 90);
                                Cv2.ImEncode(AppConsts.JpgExtension, mat, out data, iep);
                            }
                        }
                        catch (ArgumentException)
                        {
                            data = null;
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool GetJpgAndBitmapFromDatabase(Img img, out byte[] data, out Bitmap bitmap)
        {
            data = HelperSql.GetData(img);
            if (data == null || data.Length == 0)
            {
                bitmap = null;
                return false;
            }

            using (var mat = Cv2.ImDecode(data, ImreadModes.AnyColor))
            {
                try
                {
                    bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
                }
                catch (ArgumentException)
                {
                    data = null;
                    bitmap = null;
                    return false;
                }
            }

            return true;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ImageSourceFromBitmap(Bitmap bitmap)
        {
            var handle = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle);
            }
        }
    }
}
