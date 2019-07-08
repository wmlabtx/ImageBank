namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Add(Img img)
        {
            if (!_imgList.TryAdd(img.Name, img))
            {
                return;
            }

            SqlImgAdd(img);
        }
    }
}
