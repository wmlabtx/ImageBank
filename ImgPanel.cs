using System;
using System.Drawing;

namespace ImageBank
{
    public class ImgPanel : IDisposable
    {
        public Img Img { get; }
        public Bitmap Bitmap { get; }
        public int Size { get; }

        private bool disposedValue = false;

        public ImgPanel(Img img, Bitmap bitmap, int size)
        {
            Img = img;
            Bitmap = bitmap;
            Size = size;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (Bitmap != null)
                {
                    Bitmap.Dispose();
                }

                disposedValue = true;
            }
        }

        ~ImgPanel()
        {
            Dispose(false);
        }
       
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
