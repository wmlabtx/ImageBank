using OpenCvSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void CalculateClusters(IProgress<string> progress)
        {
            AppVars.SuspendEvent.Reset();

            var points = new Mat();

            const int maximages = 1024 * 4;
            const int maxdescriptors = 100;
            var counter = 0;

            if (File.Exists(AppConsts.FileDescriptors))
            {
                using (var fs = new FileStorage(AppConsts.FileDescriptors, FileStorage.Mode.Read | FileStorage.Mode.FormatXml))
                {
                    points = fs[AppConsts.DescriptorsName].ReadMat();
                    counter = points.Height / maxdescriptors;
                }
            }

            var dt = DateTime.Now;            
            var imgs = _imgList.Values.ToArray();
            for (; counter < maximages; counter++)
            {
                if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse)
                {
                    dt = DateTime.Now;
                    if (progress != null)
                    {
                        progress.Report($"analysing files ({counter}/{maximages}/{points.Height / 1024}K)...");
                    }
                }

                var jpgdata = imgs[counter].GetData();
                if (!HelperDescriptors.ComputeSiftDescriptors(jpgdata, maxdescriptors, out var descriptors))
                {
                    continue;
                }

                points.PushBack(descriptors);
            }

            progress.Report($"writing descriptors ({points.Height / 1024}K)...");
            using (var fs = new FileStorage(AppConsts.FileDescriptors, FileStorage.Mode.Write | FileStorage.Mode.FormatXml))
            {
                fs.Write(AppConsts.DescriptorsName, points);
            }

            progress.Report($"clustering ({points.Height / 1024}K)...");
            var sw = new Stopwatch();
            sw.Start();
            using (var bestlabels = new Mat())
            using (var clusters = new Mat())
            {
                Cv2.Kmeans(
                    points,
                    256,
                    bestlabels,
                    new TermCriteria(CriteriaType.Eps, 10, 0.1),
                    10,
                    KMeansFlags.PpCenters,
                    clusters);

                using (var fs = new FileStorage(AppConsts.FileClusters, FileStorage.Mode.Write | FileStorage.Mode.FormatXml))
                {
                    fs.Write(AppConsts.ClustersName, clusters);
                }
            }

            var elapsed = sw.Elapsed;
            sw.Stop();
            progress.Report($"Clustering finished ({points.Height / 1024}K) - {elapsed}");

            AppVars.SuspendEvent.Set();
        }
    }
}
