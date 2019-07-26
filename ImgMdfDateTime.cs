using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public DateTime GetMinLastView()
        {
            return _imgList.Count > 0 ?
                _imgList.Min(e => e.Value.LastView).AddMinutes(-1) :
                DateTime.Now;
        }

        public DateTime GetMinLastChecked()
        {
            return _imgList.Count > 0 ?
                _imgList.Min(e => e.Value.LastChecked).AddMinutes(-1) :
                DateTime.Now;
        }
    }
}
