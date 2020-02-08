using System.Drawing;
using System.IO;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private ImgPanel GetImgPanel(int id)
        {
            if (!_imgList.TryGetValue(id, out var img)) {
                Delete(id);
                return null;
            }

            Bitmap bitmap;
            long length;
            try {
                using (var fs = new FileStream(img.File, FileMode.Open, FileAccess.Read)) {
                    length = fs.Length;
                    bitmap = (Bitmap)Image.FromStream(fs);
                }
            }
            catch {
                length = 0;
                bitmap = null;
            }

            if (bitmap == null)
            {
                Delete(id);
                return null;
            }

            var imgpanel = new ImgPanel(
                id: id,
                name: img.Name,
                path: img.Path,
                lastview: img.LastView,
                generation: img.Generation,
                match: img.Match,
                lastchange: img.LastChange,
                bitmap: bitmap, 
                length: length);

            return imgpanel;
        }
    }
}
