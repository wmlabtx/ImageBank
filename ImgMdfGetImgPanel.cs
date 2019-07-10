using System.IO;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private ImgPanel GetImgPanel(string name)
        {
            var img = GetImgByName(name);
            if (img == null)
            {
                return null;
            }

            if (!File.Exists(img.FileName))
            {
                DeleteImg(img);
                return null;
            }

            if (!HelperImages.GetDataAndBitmap(img.FileName, out var data, out var bitmap))
            {
                DeleteImg(img);
                return null;
            }

            var realname = HelperCrc.GetCrc(data);
            if (!realname.Equals(name))
            {
                DeleteImg(img);
                return null;
            }

            var imgpanel = new ImgPanel(img, bitmap, data.Length);
            return imgpanel;
        }
    }
}
