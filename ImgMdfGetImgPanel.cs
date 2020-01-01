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
                return null;
            }

            var foldersize = GetFolderSize(img.Folder);
            if (!HelperImages.GetBitmapFromFile(img.File, out var jpgdata, out var bitmap, out var _))
            {
                Delete(hash);
                return null;
            }

            var imgpanel = new ImgPanel(
                hash:hash, 
                folder: img.Folder, 
                foldersize:foldersize, 
                lastview:img.LastView,
                lastcheck:img.LastCheck,
                bitmap:bitmap, 
                length:jpgdata.Length);

            return imgpanel;
        }
    }
}
