namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Add(Img img)
        {
            _imgList.TryAdd(img.Hash, img);
            SqlAdd(img);
        }
    }
}
