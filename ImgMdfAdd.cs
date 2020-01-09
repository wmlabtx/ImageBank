namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Add(Img img)
        {
            _imgList.TryAdd(img.Hash, img);
            SqlAdd(img);
        }
    }
}
