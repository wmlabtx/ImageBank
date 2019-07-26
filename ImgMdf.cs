using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private ConcurrentDictionary<string, Img> _imgList = new ConcurrentDictionary<string, Img>();       
        private readonly Queue<double> _findTimes = new Queue<double>();
        private double _avgTimes;
    }
}