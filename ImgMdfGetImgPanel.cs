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

            if (!HelperImages.GetJpgAndBitmapFromDatabase(img, out var data, out var bitmap))
            {
                DeleteImg(img);
                return null;
            }

            var imgpanel = new ImgPanel(img, bitmap, data.Length);
            return imgpanel;
        }
    }
}
