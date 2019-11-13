using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void SetNode(string node)
        {
            var imgX = AppVars.ImgPanel[0].Img;
            imgX.Node = node;
            UpdateNode(imgX);

            Img[] scope;
            if (string.IsNullOrEmpty(node))
            {
                scope = _imgList
                    .Where(e => !e.Value.Name.Equals(imgX.Name))
                    .Select(e => e.Value)
                    .ToArray();
            }
            else
            {
                scope = _imgList
                    .Where(e => e.Value.Node.Equals(node))
                    .Select(e => e.Value)
                    .ToArray();
            }

            var imgY = scope[scope.Length / 2];
            imgX.NextName = imgY.Name;
            imgX.Sim = 0f;
            imgX.LastChecked = GetMinLastChecked();
            UpdateNameNext(imgX);

            AppVars.ImgPanel[1] = GetImgPanel(imgY.Name);
        }

        public int NodeSize(string node)
        {
            if (string.IsNullOrEmpty(node))
            {
                return 0;
            }

            var nodesize = _imgList.Count(e => e.Value.Node.Equals(node) && e.Value.Descriptors.Size.Height > 0);
            return nodesize;
        }
    }
}
