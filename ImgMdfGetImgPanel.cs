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
                var imgdata = Helper.ReadEncryptedData(img.File);
                using (var ms = new MemoryStream(imgdata)) {
                    length = imgdata.Length;
                    bitmap = (Bitmap)Image.FromStream(ms);
                }
            }
            catch {
                length = 0;
                bitmap = null;
            }

            if (bitmap == null) {
                Delete(id);
                return null;
            }

            var imgpanel = new ImgPanel(
                id: id,
                name: img.Name,
                path: img.Path,
                lastview: img.LastView,
                generation: img.Generation,
                quality: img.Quality,
                lastchange: img.LastChange,
                bitmap: bitmap, 
                length: length,
                descriptors: img.GetDescriptors().Length);

            return imgpanel;
        }
    }
}
