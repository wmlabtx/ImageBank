using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public DateTime GetMinLastView()
        {
            return _imgList.Count > 0 ?
                _imgList.Min(e => e.Value.LastView).AddSeconds(-1) :
                DateTime.Now;
        }

        public DateTime GetMinLastChecked()
        {
            return _imgList.Count > 0 ?
                _imgList.Min(e => e.Value.LastChecked).AddSeconds(-1) :
                DateTime.Now;
        }

        public int GetMaxId()
        {
            return _imgList.Count > 0 ?
                _imgList.Max(e => e.Value.Id) + 1 :
                1;
        }
    }
}
