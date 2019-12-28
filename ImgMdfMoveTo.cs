using System.IO;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void MoveTo(string name, string node)
        {
            if (_imgList.TryGetValue(name, out var img))
            {
                var oldfilename = img.DataFile;
                img.Folder = node;
                if (!img.DataFile.Equals(oldfilename))
                {
                    File.Move(oldfilename, img.DataFile);
                    img.Id = GetNextId();
                    ResetNextName(img);
                    ResetRefers(name);
                }
            }
        }
    }
}
