using System.Diagnostics;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private ImgPanel GetImgPanel(string hash)
        {
            Debug.Assert(!string.IsNullOrEmpty(hash), $"hash is null");

            if (!_imgList.TryGetValue(hash, out var img))
            {
                Delete(hash);
                return null;
            }

            if (!HelperImages.GetBitmapFromFile(img.File, out var jpgdata, out var bitmap, out var _))
            {
                Delete(hash);
                return null;
            }

            var imgpanel = new ImgPanel(
                hash:hash, 
                subdirectory: img.Subdirectory,
                lastview: img.LastView,
                generation: img.Generation,
                sim: img.Sim,
                lastchange: img.LastChange,
                bitmap:bitmap, 
                length:jpgdata.Length);

            return imgpanel;
        }
    }
}
