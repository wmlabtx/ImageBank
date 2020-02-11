using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void FindNext(int idX, out DateTime lastcheck, out DateTime lastchange, out int nextid, out int match)
        {
            var imgX = _imgList[idX];
            lastcheck = DateTime.Now;
            lastchange = imgX.LastChange;
            nextid = imgX.Id;
            match = 0;

            var candidates = 
                (imgX.Path.StartsWith(AppConsts.PathLegacy) ?
                _imgList.Values.Where(e => e.GetDescriptors().Length > 0) :
                _imgList.Values.Where(e => e.GetDescriptors().Length > 0 && imgX.Path.StartsWith(e.Path)))
                .ToArray();

            if (candidates.Length == 0) {
                return;
            }

            foreach (var imgY in candidates) {
                if (imgY.Id == imgX.Id) {
                    continue;
                }

                var matchxy = HelperDescriptors.ComputeMatch(imgX.GetDescriptors(), imgY.GetDescriptors());
                if (matchxy > match) {
                    match = matchxy;
                    nextid = imgY.Id;
                    lastchange = DateTime.Now;
                }
            }
        }
    }
}
