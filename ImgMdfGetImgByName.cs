namespace ImageBank
{
    public partial class ImgMdf
    {
        public Img GetImgByName(string name)
        {
            return !_imgList.TryGetValue(name, out var img) ? null : img;
        }
    }
}
