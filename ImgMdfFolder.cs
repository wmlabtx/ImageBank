using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private Img[] GetImagesFromFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return _imgList.Values.ToArray();
            }
            else
            {
                var images = _imgList
                    .Values
                    .Where(e => !string.IsNullOrEmpty(e.Folder) && e.Folder.IndexOf(folder, StringComparison.OrdinalIgnoreCase) == 0)
                    .ToArray();

                return images;
            }
        }

        public int GetFolderSize(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return _imgList.Count;
            }
            else
            {
                var foldersize = _imgList
                    .Values
                    .Count(e => !string.IsNullOrEmpty(e.Folder) && e.Folder.IndexOf(folder, StringComparison.OrdinalIgnoreCase) == 0);

                return foldersize;
            }
        }
    }
}
