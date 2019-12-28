using System;
using System.Diagnostics;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private int GetFolderSize(string folderX)
        {
            Debug.Assert(!string.IsNullOrEmpty(folderX), $"folderX is null");

            if (folderX.StartsWith(AppConsts.FolderLegacy, StringComparison.OrdinalIgnoreCase))
            {
                return _imgList.Count;
            }
            else
            {
                var foldersize = _imgList.Count(e => e.Value.Folder.IndexOf(folderX, StringComparison.OrdinalIgnoreCase) == 0);
                return foldersize;
            }
        }

        private ImgPanel GetImgPanel(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), $"name is null");

            if (!_imgList.TryGetValue(name, out var img))
            {
                return null;
            }

            var foldersize = GetFolderSize(img.Folder);
            if (!HelperImages.GetBitmapFromFile(img.DataFile, out var jpgdata, out var bitmap, out var _))
            {
                Delete(name);
                return null;
            }

            var imgpanel = new ImgPanel(
                name:name, 
                folder: img.Folder, 
                foldersize:foldersize, 
                lastview:img.LastView,
                lastchange:img.LastChange,
                distance:img.Distance,
                bitmap:bitmap, 
                length:jpgdata.Length);

            return imgpanel;
        }
    }
}
