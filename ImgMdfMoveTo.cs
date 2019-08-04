using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void MoveTo(Img img, string destnode)
        {
            if (img.Node.Equals(destnode))
            {
                return;
            }

            var imgY = GetImgByName(img.NextName);
            if (img != null)
            {
                if (destnode.Equals(imgY.Node))
                {
                    img.Node = destnode;
                    UpdateNode(img);
                    return;
                }
            }

            img.Node = destnode;
            UpdateNode(img);
            ResetNextName(img);
            AppVars.ImgPanel[1] = GetImgPanel(img.NextName);
        }
    }
}
