using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void DeleteNextName(string name)
        {
            var scope = _imgList
                .Select(e => e.Value)
                .Where(e => e.NextName.Equals(name))
                .ToArray();

            foreach (var img in scope)
            {
                ResetNextName(img);
            }
        }

        public void DeleteImg(Img img)
        {
            if (!_imgList.TryRemove(img.Name, out var imgDeleted))
            {
                return;
            }

            HelperSql.DeleteImgAndFile(imgDeleted);
            DeleteNextName(img.Name);
        }
    }
}