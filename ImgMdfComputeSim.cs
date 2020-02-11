﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void ComputeSim(BackgroundWorker backgroundworker)
        {
            AppVars.SuspendEvent.WaitOne(Timeout.Infinite);

            var count = _imgList.Count();
            if (count == 0) {
                backgroundworker.ReportProgress(0, "no images");
                return;
            }

            var idX = GetNextToCheck();
            if (idX < 0) {
                backgroundworker.ReportProgress(0, "idle");
                return;
            }

            if (!_imgList.TryGetValue(idX, out var imgX)) {
                backgroundworker.ReportProgress(0, $"error getting {idX}");
                return;
            }

            if (imgX.GetDescriptors().Length == 0) {
                var jpgdata = ReadJpgData(imgX.Name, imgX.Path);
                if (!HelperDescriptors.ComputeHashes(jpgdata, out var descriptors)) {
                    backgroundworker.ReportProgress(0, $"Unable to compute hashes: {imgX.Path}\\{imgX.Name}{AppConsts.JpgExtension}");
                    return;
                }

                imgX.SetDescriptors(descriptors);
            }

            FindNext(idX, out var lastcheck, out var lastchange, out var nextid, out var match);

            var sb = new StringBuilder();
            sb.Append($"i{imgX.Id}: ");
            if (match != imgX.Match) {
                sb.Append($"{imgX.Match} ");
                sb.Append($"{char.ConvertFromUtf32(match > imgX.Match ? 0x2192 : 0x2193)} ");
                sb.Append($"{match}");
                imgX.Match = match;
                if (nextid != imgX.NextId) {
                    imgX.NextId = nextid;
                }
            }
            else {
                if (nextid != imgX.NextId) {
                    sb.Append($"i{imgX.NextId} ");
                    sb.Append($"{char.ConvertFromUtf32(0x2192)} ");
                    sb.Append($"i{nextid}");
                    imgX.NextId = nextid;
                }
            }

            if (lastchange != imgX.LastChange) {
                imgX.LastChange = lastchange;
            }

            sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(imgX.LastCheck))} ago]");
            if (lastcheck != imgX.LastCheck) {
                imgX.LastCheck = lastcheck;
            }

            var message = sb.ToString();
            backgroundworker.ReportProgress(0, message);
        }
    }
}