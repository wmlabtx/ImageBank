using System;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void FindNext(int idX, out int lastid, out DateTime lastchange, out int nextid, out int distance)
        {
            lock (_imglock) {
                var imgX = _imgList[idX];
                lastchange = imgX.LastChange;
                nextid = imgX.Id;
                distance = 256;
                lastid = _id;

                foreach (var imgY in _imgList.Values) {
                    if (imgY.Id == idX) {
                        continue;
                    }

                    var distancephash = Helper.GetDistance(imgX.Phash, imgY.Phash);
                    if (distancephash < 8 && distancephash < distance) {
                        distance = distancephash;
                        nextid = imgY.Id;
                        lastchange = DateTime.Now;
                    }
                    else {
                        var distanceorbv = Helper.GetDistance(imgX.Orbv(), imgY.Orbv());
                        if (distanceorbv < distance) {
                            distance = distanceorbv;
                            nextid = imgY.Id;
                            lastchange = DateTime.Now;
                        }
                    }
                }
            }
        }
    }
}
