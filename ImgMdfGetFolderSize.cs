using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public int GetFolderSize(string folder)
        {
            if (HelperPath.IsLegacy(folder))
            {
                return _imgList.Count(e => HelperPath.IsLegacy(e.Value.Folder));
            }
            else
            {
                return _imgList.Count(e => e.Value.Folder.Equals(folder));
            }            
        }
    }
}
