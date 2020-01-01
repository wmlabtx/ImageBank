namespace ImageBank
{
    public partial class ImgMdf
    {
        public void MoveTo(string hash, string folder)
        {
            if (_imgList.TryGetValue(hash, out var imgX))
            {
                imgX.Folder = folder;
                if (!string.IsNullOrEmpty(folder))
                {
                    if (_imgList.TryGetValue(imgX.NextHash, out var imgY))
                    {
                        if (imgY.Folder.IndexOf(folder) != 0)
                        {
                            var images = GetImagesFromFolder(folder);
                            var index = HelperRandom.Next(images.Length);
                            imgX.NextHash = images[index].Hash;
                        }
                    }
                }
            }
        }
    }
}
