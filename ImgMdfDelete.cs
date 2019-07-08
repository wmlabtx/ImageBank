using System;
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
                img.NextName = img.Name;
                img.Sim = 0f;
                img.LastChecked = DateTime.Now.AddYears(-10);
                SqlUpdateLink(img.Name, img.NextName, img.Sim, img.LastChecked);
            }
        }

        private void DeleteImg(Img img)
        {
            if (!_imgList.TryRemove(img.Name, out var imgDeleted))
            {
                return;
            }

            SqlImgDelete(img);
            DeleteNextName(imgDeleted.Name);
        }

        public void DeleteImgAndFile(string name)
        {
            if (!_imgList.TryGetValue(name, out var img))
            {
                return;
            }

            HelperRecycleBin.Delete(img.FileName);
            DeleteImg(img);
        }
    }
}