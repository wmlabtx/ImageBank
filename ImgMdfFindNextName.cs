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
            img.Sim = 0f;
            img.LastChecked = GetMinLastChecked();
            img.LastId = img.Id;
            UpdateNameNext(img);
        }

        private void FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            var oldsim = imgX.Sim;

            imgX.LastChecked = DateTime.Now;

            if (imgX.Orbs.Rows == 0)
            {
                var jpgdata = imgX.GetData();
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return;
                }

                if (!HelperDescriptors.ComputeDescriptors(jpgdata, out var orbs))
                {
                    DeleteImg(imgX);
                    return;
                }

                imgX.Orbs = orbs;
                imgX.Id = GetMaxId();
                imgX.LastId = imgX.Id;
                imgX.NextName = imgX.Name;
                imgX.Sim = 0f;
            }
            else
            {
                if (imgX.Id == 0)
                {
                    imgX.Id = GetMaxId();
                    imgX.LastId = imgX.Id;
                    imgX.NextName = imgX.Name;
                    imgX.Sim = 0f;
                }
                else
                {
                    if (imgX.LastId < imgX.Id)
                    {
                        imgX.LastId = imgX.Id;
                        imgX.NextName = imgX.Name;
                        imgX.Sim = 0f;
                    }
                    else
                    {
                        if (imgX.Name.Equals(imgX.NextName))
                        {
                            imgX.Sim = 0f;
                            imgX.LastId = imgX.Id;
                        }
                        else
                        {
                            var imgY = GetImgByName(imgX.NextName);
                            if (imgY == null)
                            {
                                imgX.NextName = imgX.Name;
                                imgX.Sim = 0f;
                                imgX.LastId = imgX.Id;
                            }
                        }
                    }
                }
            }

            var scope = _imgList
                .Where(e => e.Value.Id > imgX.LastId && e.Value.Id > imgX.Id)
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
                    var sim = HelperDescriptors.GetSim(imgX.Orbs, imgY.Orbs);
                    if (sim > imgX.Sim)
                    {
                        imgX.NextName = imgY.Name;
                        imgX.Sim = sim;
                        imgX.LastChecked = DateTime.Now;
                    }
                }

                if (sw.ElapsedMilliseconds > 1000)
                {
                    break;
                }
            }

            if (!imgX.NextName.Equals(oldnextname) || oldsim != imgX.Sim)
            {
                imgX.LastChanged = DateTime.Now;
            }

            UpdateNameNext(imgX);
            return;
        }
    }
}