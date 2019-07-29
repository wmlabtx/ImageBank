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

            var jpgdata = GetJpgData(img);
            if (jpgdata == null)
            {
                return null;
            }

            if (!HelperImages.GetBitmap(jpgdata, out var bitmap))
            {
                DeleteImg(img);
                return null;
            }

            var imgpanel = new ImgPanel(img, bitmap, jpgdata.Length);
            return imgpanel;
        }
    }
}
