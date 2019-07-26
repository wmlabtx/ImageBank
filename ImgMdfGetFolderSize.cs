using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public int GetFolderSize(string folder)
        {
            return _imgList.Count(e => e.Value.Folder.Equals(folder));
        }
    }
}
