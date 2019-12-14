using System;
using System.Diagnostics;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        private void ResetNextName(Img img)
        {
            img.LastId = 0;
            img.NextName = img.Name;
            img.Sim = 0f;
            img.LastChecked = GetMinLastChecked();
            UpdateNameNext(img);
        }

        private void FindNextName(Img imgX)
        {
            var oldnextname = imgX.NextName;
            var oldsim = imgX.Sim;

            imgX.LastChecked = DateTime.Now;

            if (imgX.Id == 0)
            {
                imgX.Id = _imgList.Max(e => e.Value.Id) + 1;
                UpdateProperty(imgX, AppConsts.AttrId, imgX.Id);
            }

            if (imgX.Descriptors.Rows == 0)
            {
                var jpgdata = imgX.GetData();
                if (jpgdata == null || jpgdata.Length == 0)
                {
                    DeleteImg(imgX);
                    return;
                }

                if (!HelperDescriptors.ComputeDescriptors(jpgdata, out var descriptors))
                {
                    DeleteImg(imgX);
                    return;
                }

                imgX.Descriptors = descriptors;
            }

            if (!_imgList.ContainsKey(imgX.NextName) || imgX.Name.Equals(imgX.NextName))
            {
                ResetNextName(imgX);
            }

            var scope = _imgList
                .Where(e => e.Value.Descriptors.Rows > 0 && e.Value.Id > imgX.LastId)
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
                if (imgX.Name.Equals(imgY.Name))
                {
                    continue;
                }

                var sim = HelperDescriptors.GetSim(imgX.Descriptors, imgY.Descriptors);
                if (sim > imgX.Sim)
                {
                    imgX.NextName = imgY.Name;
                    imgX.Sim = sim;
                    imgX.LastChecked = DateTime.Now;
                    break;
                }

                if (sw.ElapsedMilliseconds > 1000)
                {
                    sw.Stop();
                    break;
                }
            }

            if (!imgX.NextName.Equals(oldnextname) || oldsim != imgX.Sim)
            {
                imgX.LastChanged = DateTime.Now;
            }

            UpdateNameNext(imgX);
        }
    }
}