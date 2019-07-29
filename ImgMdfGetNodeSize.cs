using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public int GetNodeSize(string node)
        {
            return _imgList.Count(e => e.Value.Node.Equals(node));
        }
    }
}
