using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            img.LastId = 0;
            img.NextName = img.Name;
            img.Distance = 256;
            img.LastChecked = GetMinLastChecked();
            UpdateNameNext(img);
        }

        private int FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            var olddistance = imgX.Distance;

            imgX.LastChecked = DateTime.Now;

            if (imgX.Vector.Length < 4)
            {
                var jpgdata = imgX.GetData();
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return 0;
                }

                if (!HelperDescriptors.ComputeVector(jpgdata, out var vector))
                {
                    DeleteImg(imgX);
                    return 0;
                }

                imgX.Vector = vector;
            }

            if (!_imgList.ContainsKey(imgX.NextName) || imgX.Name.Equals(imgX.NextName))
            {
                ResetNextName(imgX);
            }

            var scope = _imgList
                .Where(e => e.Value.Vector.Length >= 4 && e.Value.Id > imgX.LastId)
                .OrderBy(e => e.Value.Id)
                .Select(e => e.Value)
                .ToArray();

            if (scope.Length == 0)
            {
                UpdateNameNext(imgX);
                return 0;
            }
           
            var updates = 0;
            foreach (var imgY in scope)
            {
                imgX.LastId = imgY.Id;
                if (imgX.Name.Equals(imgY.Name))
                {
                    continue;
                }

                var distance = HelperDescriptors.GetDistance(imgX.Vector, imgY.Vector);
                if (distance < imgX.Distance)
                {
                    imgX.NextName = imgY.Name;
                    imgX.Distance = distance;
                    imgX.LastChecked = DateTime.Now;
                }

                if (distance < imgY.Distance)
                {
                    imgY.NextName = imgX.Name;
                    imgY.Distance = distance;
                    imgY.LastChanged = DateTime.Now;
                    UpdateNameNext(imgY);
                    updates++;
                }
            }

            if (!imgX.NextName.Equals(oldnextname) || olddistance != imgX.Distance)
            {
                imgX.LastChanged = DateTime.Now;
            }

            UpdateNameNext(imgX);
            return updates;
        }
    }
}