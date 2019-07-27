using System.Drawing;

namespace ImageBank
{
    public class ImgPanel
    {
        public Img Img { get; }
        public Bitmap Bitmap { get; }
        public int Size { get; }

        public ImgPanel(Img img, Bitmap bitmap, int size)
        {
            Img = img;
            Bitmap = bitmap;
            Size = size;
        }
    }
}
