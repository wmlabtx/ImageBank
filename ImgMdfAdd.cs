namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Add(Img img, byte[] data)
        {
            if (!_imgList.TryAdd(img.Name, img))
            {
                return;
            }

            HelperSql.AddImg(img, data);
        }
    }
}
