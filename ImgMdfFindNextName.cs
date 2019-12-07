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
            img.Distance = 64;
            img.Sim = 0f;
            img.LastChecked = GetMinLastChecked();
            img.LastId = 0;
            UpdateNameNext(img);
        }

        private void FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            var olddistance = imgX.Distance;
            var oldsim = imgX.Sim;

            imgX.LastChecked = DateTime.Now;

            if (imgX.PHash == 0 || imgX.Orbs.Rows == 0)
            {
                var jpgdata = GetJpgData(imgX);
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return;
                }

                var crcname = HelperCrc.GetCrc(jpgdata);
                if (!crcname.Equals(imgX.Name))
                {
                    DeleteImg(imgX);
                    return;
                }

                if (!HelperDescriptors.ComputeDescriptors(jpgdata, out var phash, out var orbs))
                {
                    DeleteImg(imgX);
                    return;
                }

                imgX.PHash = phash;
                imgX.Orbs = orbs;
                imgX.Id = GetMaxId();

                imgX.NextName = imgX.Name;
                imgX.Distance = 64;
                imgX.Sim = 0f;
            }
            else
            {
                if (imgX.Name.Equals(imgX.NextName))
                {
                    imgX.Distance = 64;
                    imgX.Sim = 0f;
                    imgX.LastId = 0;
                }
                else
                {
                    var imgY = GetImgByName(imgX.NextName);
                    if (imgY == null)
                    {
                        imgX.NextName = imgX.Name;
                        imgX.Distance = 64;
                        imgX.Sim = 0f;
                        imgX.LastId = 0;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(imgX.Person) && !imgX.Person.Equals(imgY.Person))
                        {
                            imgX.NextName = imgX.Name;
                            imgX.Distance = 64;
                            imgX.Sim = 0f;
                            imgX.LastId = 0;
                        }
                    }
                }
            }

            var scope = _imgList
                .Where(e => e.Value.Id != imgX.Id && e.Value.Id > imgX.LastId)
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
                if (string.IsNullOrEmpty(imgX.Person) || imgX.Person.Equals(imgY.Person))
                {
                    var distance = HelperDescriptors.GetDistance(imgX.PHash, imgY.PHash);
                    if (distance < AppConsts.MinHammingDistance && distance < imgX.Distance)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Distance = distance;
                        imgX.Sim = HelperDescriptors.GetSim(imgX.Orbs, imgY.Orbs);
                        imgX.LastChecked = DateTime.Now;
                    }
                    else
                    {
                        var sim = HelperDescriptors.GetSim(imgX.Orbs, imgY.Orbs);
                        if (sim > imgX.Sim)
                        {
                            imgX.NextName = imgY.Name;
                            imgX.Sim = sim;
                            imgX.Distance = HelperDescriptors.GetDistance(imgX.PHash, imgY.PHash);
                            imgX.LastChecked = DateTime.Now;
                        }
                    }
                }

                if (sw.ElapsedMilliseconds > 1000)
                {
                    break;
                }
            }

            if (!imgX.NextName.Equals(oldnextname) || olddistance != imgX.Distance || oldsim != imgX.Sim)
            {
                imgX.LastChanged = DateTime.Now;
            }

            UpdateNameNext(imgX);
            return;
        }
    }
}