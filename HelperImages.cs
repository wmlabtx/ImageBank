using OpenCvSharp;
using OpenCvSharp.Extensions;
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
        public static bool GetBitmapFromFile(string filename, out byte[] jpgdata, out Bitmap bitmap, out bool needwrite)
        {
            jpgdata = null;
            bitmap = null;
            needwrite = false;
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

            jpgdata = File.ReadAllBytes(filename);
            if (jpgdata == null || jpgdata.Length == 0)
            {
                return false;
            }

            if (extension.Equals(AppConsts.DatExtension))
            {
                var password = HelperPath.GetPassword(filename);
                jpgdata = HelperEncrypting.Decrypt(jpgdata, password);
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    return false;
                }
                
                extension = AppConsts.JpgExtension;
                needwrite = true;
            }

            Mat mat = null;
            try
            {
                mat = Cv2.ImDecode(jpgdata, ImreadModes.AnyColor);
                const float fmax = 6000f * 4000f;
                var fx = (float)Math.Sqrt(fmax / (mat.Width * mat.Height));
                if (fx < 1f)
                {
                    mat = mat.Resize(OpenCvSharp.Size.Zero, fx, fx, InterpolationFlags.Cubic);
                    var iep = new ImageEncodingParam(ImwriteFlags.WebPQuality, 64);
                    Cv2.ImEncode(AppConsts.WebpExtension, mat, out jpgdata, iep);
                    needwrite = true;
                }
                else
                {
                    if (!extension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase) &&
                        !extension.Equals(AppConsts.JpegExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var iep = new ImageEncodingParam(ImwriteFlags.JpegQuality, 95);
                        Cv2.ImEncode(AppConsts.JpgExtension, mat, out jpgdata, iep);
                        needwrite = true;
                    }
                }

                bitmap = BitmapConverter.ToBitmap(mat);
            }
            catch (ArgumentException)
            {
                jpgdata = null;
                bitmap = null;
                return false;
            }
            finally
            {
                if (mat != null)
                {
                    mat.Dispose();
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
