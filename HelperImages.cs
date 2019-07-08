using OpenCvSharp;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageBank
{
    public static class HelperImages
    {
        public static Bitmap GetBitmap(string filename)
        {
            Bitmap bitmap;
            using (var mat = new Mat(filename))
            {
                try
                {
                    bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
                }
                catch (System.ArgumentException)
                {
                    return null;
                }
            }

            return bitmap;
        }

        public static bool SaveBitmap(string filename, Bitmap bitmap)
        {
            var iep = new ImageEncodingParam(ImwriteFlags.WebPQuality, 100);
            using (var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap))
            {
                if (!mat.SaveImage(filename, iep))
                {
                    return false;
                }
            }

            return true;
        }

        public static byte[] ConvertToWebp(Bitmap bitmap)
        {
            var iep = new ImageEncodingParam(ImwriteFlags.WebPQuality, 100);
            using (var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap))
            {
                Cv2.ImEncode(AppConsts.WebpExtension, mat, out var data, iep);
                if (data != null && data.Length > 0)
                {
                    return data;
                }
            }

            return null;
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
