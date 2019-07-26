using System;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Load(IProgress<string> progress)
        {
            _imgList = HelperSql.Load(progress);
        }
    }
}