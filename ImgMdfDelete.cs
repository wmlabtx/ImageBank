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

        public void DeleteImg(string name)
        {
            if (!_imgList.TryRemove(name, out var imgDeleted))
            {
                return;
            }

            HelperSql.DeleteImgAndFile(imgDeleted);
            DeleteNextName(imgDeleted.Name);
        }
    }
}