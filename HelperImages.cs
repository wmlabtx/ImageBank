﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
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
        public static bool GetJpgFromFile(string filename, out byte[] jpgdata)
        {
            jpgdata = null;
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
            }
            else
            {
                if (
                    extension.Equals(AppConsts.PngExtension, StringComparison.InvariantCultureIgnoreCase) ||
                    extension.Equals(AppConsts.BmpExtension, StringComparison.InvariantCultureIgnoreCase) ||
                    extension.Equals(AppConsts.WebpExtension, StringComparison.InvariantCultureIgnoreCase)
                   )
                {
                    try
                    {
                        var mat = new Mat();
                        CvInvoke.Imdecode(jpgdata, ImreadModes.AnyColor, mat);
                        var bitmap = mat.Bitmap;
                        if (!extension.Equals(AppConsts.JpgExtension, StringComparison.InvariantCultureIgnoreCase) &&
                            !extension.Equals(AppConsts.JpegExtension, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var iep = new KeyValuePair<ImwriteFlags, int>[] { new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, 90) };
                            var data = new VectorOfByte();
                            CvInvoke.Imencode(AppConsts.JpgExtension, mat, data, iep);
                            jpgdata = data.ToArray();
                        }
                    }
                    catch (ArgumentException)
                    {
                        jpgdata = null;
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool GetBitmap(byte[] jpgdata, out Bitmap bitmap)
        {
            try
            {
                using (var ms = new MemoryStream(jpgdata))
                {
                    bitmap = (Bitmap)Image.FromStream(ms);
                }
            }
            catch (ArgumentException)
            {
                bitmap = null;
                return false;
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
