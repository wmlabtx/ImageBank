namespace ImageBank
{
    public partial class ImgMdf
    {
        private void Add(Img img)
        {
            _imgList.TryAdd(img.Id, img);
            _nameList.TryAdd(img.Name, img);
            _checksumList.TryAdd(img.Checksum, img);
            SqlAdd(img);
        }
    }
}
