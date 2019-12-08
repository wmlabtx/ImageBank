using System;
using System.Diagnostics;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            img.NextName = img.Name;
            img.Distance = AppConsts.MaxClustersInImage * 256;
            img.LastChecked = GetMinLastChecked();
            img.LastId = 0;
            UpdateNameNext(img);
        }

        private void FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            var olddistance = imgX.Distance;

            imgX.LastChecked = DateTime.Now;

            if (imgX.Vector.Length == 0)
            {
                var jpgdata = imgX.GetData();
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return;
                }

                if (!HelperDescriptors.ComputeVector(jpgdata, out var vector))
                {
                    DeleteImg(imgX);
                    return;
                }

                imgX.Vector = vector;
                imgX.Id = GetMaxId();
                imgX.LastId = 0;
                imgX.NextName = imgX.Name;
                imgX.Distance = AppConsts.MaxClustersInImage * 256;
            }
            else
            {
                if (imgX.Id == 0)
                {
                    imgX.Id = GetMaxId();
                    imgX.LastId = 0;
                    imgX.NextName = imgX.Name;
                    imgX.Distance = AppConsts.MaxClustersInImage * 256;
                }
                else
                {
                    if (imgX.LastId < imgX.Id)
                    {
                        imgX.LastId = 0;
                        imgX.NextName = imgX.Name;
                        imgX.Distance = AppConsts.MaxClustersInImage * 256;
                    }
                    else
                    {
                        if (imgX.Name.Equals(imgX.NextName))
                        {
                            imgX.Distance = AppConsts.MaxClustersInImage * 256;
                            imgX.LastId = 0;
                        }
                        else
                        {
                            var imgY = GetImgByName(imgX.NextName);
                            if (imgY == null)
                            {
                                imgX.NextName = imgX.Name;
                                imgX.Distance = AppConsts.MaxClustersInImage * 256;
                                imgX.LastId = 0;
                            }
                        }
                    }
                }
            }

            var scope = _imgList
                .Where(e => e.Value.Id > imgX.LastId)
                .OrderBy(e => e.Value.Id)
                .Select(e => e.Value)
                .ToArray();

            if (scope.Length == 0)
            {
                UpdateNameNext(imgX);
                return;
            }

            var sw = new Stopwatch();
            sw.Start();
            foreach (var imgY in scope)
            {
                imgX.LastId = imgY.Id;
                if (imgX.LastId != imgX.Id)
                {
                    var distance = HelperDescriptors.GetDistance(imgX.Vector, imgY.Vector, imgX.Distance);
                    if (distance < imgX.Distance)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Distance = distance;
                        imgX.LastChecked = DateTime.Now;
                    }
                }

                if (sw.ElapsedMilliseconds > 1000)
                {
                    break;
                }
            }

            if (!imgX.NextName.Equals(oldnextname) || olddistance != imgX.Distance)
            {
                imgX.LastChanged = DateTime.Now;
            }

            UpdateNameNext(imgX);
            return;
        }
    }
}