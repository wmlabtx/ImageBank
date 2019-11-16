using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            img.NextName = img.Name;
            img.Distance = 64;
            img.LastChecked = GetMinLastChecked();
            UpdateNameNext(img);
        }

        private int FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            var olddistance = imgX.Distance;

            imgX.LastChecked = DateTime.Now;

            if (imgX.PHash == 0)
            {
                var jpgdata = GetJpgData(imgX);
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return 0;
                }

                var crcname = HelperCrc.GetCrc(jpgdata);
                if (!crcname.Equals(imgX.Name))
                {
                    DeleteImg(imgX);
                    return 0;
                }

                if (!HelperDescriptors.ComputeDescriptors(jpgdata, out var phash))
                {
                    DeleteImg(imgX);
                    return 0;
                }

                imgX.PHash = phash;
                UpdatePHash(imgX);

                imgX.NextName = imgX.Name;
                imgX.Distance = 64;
            }
            else
            {
                if (imgX.Name.Equals(imgX.NextName))
                {
                    imgX.Distance = 64;
                }
                else
                {
                    var imgY = GetImgByName(imgX.NextName);
                    if (imgY == null)
                    {
                        imgX.NextName = imgX.Name;
                        imgX.Distance = 64;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(imgX.Person) && !imgX.Person.Equals(imgY.Person))
                        {
                            imgX.NextName = imgX.Name;
                            imgX.Distance = 64;
                        }
                    }
                }
            }

            var scope = _imgList
                .Where(e => !e.Value.Name.Equals(imgX.Name) && e.Value.PHash > 0)
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
                if (string.IsNullOrEmpty(imgX.Person) || imgX.Person.Equals(imgY.Person))
                {
                    var distance = HelperDescriptors.Distance(imgX.PHash, imgY.PHash);
                    if (distance < imgX.Distance)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Distance = distance;
                        imgX.LastChecked = DateTime.Now;
                    }

                    if (string.IsNullOrEmpty(imgY.Person) || imgY.Person.Equals(imgX.Person))
                    {
                        if (distance < imgY.Distance)
                        {
                            imgY.NextName = imgX.Name;
                            imgY.Distance = distance;
                            imgY.LastChanged = DateTime.Now;
                            imgY.LastChecked = DateTime.Now;
                            UpdateNameNext(imgY);
                            updates++;
                        }
                    }
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