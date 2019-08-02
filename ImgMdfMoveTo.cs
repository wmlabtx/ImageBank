using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void MoveTo(Img img, string destnode)
        {
            img.Node = destnode;
            UpdateNode(img);
            ResetNextName(img);
            AppVars.ImgPanel[1] = GetImgPanel(img.NextName);
        }
    }
}
