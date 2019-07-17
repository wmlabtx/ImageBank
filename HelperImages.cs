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
        public static bool GetDataAndBitmap(string filename, out byte[] data, out Bitmap bitmap)
        {
            if (!File.Exists(filename))
            {
                data = null;
                bitmap = null;
                return false;
            }

            data = File.ReadAllBytes(filename);
            if (data == null || data.Length == 0)
            {
                bitmap = null;
                return false;
            }

            /*
            var extension = Path.GetExtension(filename);
            if (extension.Equals(AppConsts.DatExtension))
            {
                var name = HelperPath.GetName(filename);
                data = HelperEncrypting.Decrypt(data, name);
                if (data == null || data.Length == 0)
                {
                    bitmap = null;
                    return false;
                }
            }
            */

            using (var mat = Cv2.ImDecode(data, ImreadModes.AnyColor))
            {
                try
                {
                    bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
                    var extension = Path.GetExtension(filename);
                    if (!extension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var iep = new ImageEncodingParam(ImwriteFlags.JpegQuality, 90);
                        Cv2.ImEncode(AppConsts.JpgExtension, mat, out data, iep);
                    }
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
