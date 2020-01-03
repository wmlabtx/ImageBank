using OpenCvSharp;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void GatherSiftDescriptors(IProgress<string> progress)
        {
            var added = 0;
            var dt = DateTime.Now;
            var directoryInfo = new DirectoryInfo(AppConsts.PathCollection);
            var collecteddescriptorsrows = 0;
            var fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            using (var fs = new FileStream(AppConsts.FileSiftDescriptors, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                foreach (var fileInfo in fileInfos)
                {
                    if (DateTime.Now.Subtract(dt).TotalMilliseconds > AppConsts.TimeLapse)
                    {
                        dt = DateTime.Now;
                        if (progress != null)
                        {
                            var collectedk = collecteddescriptorsrows / 1024;
                            progress.Report($"gathering descriptors ({added}/{collectedk}K)...");
                        }
                    }

                    var filename = fileInfo.FullName;
                    var name = HelperPath.GetName(filename);
                    if (!HelperImages.GetBitmapFromFile(filename, out var jpgdata, out var bitmap, out var needwrite))
                    {
                        continue;
                    }

                    if (!HelperSift.ComputeDescriptors(jpgdata, out var descriptors))
                    {
                        continue;
                    }

                    descriptors.GetArray<float>(out var fbuffer);
                    var buffer = new byte[fbuffer.Length];
                    for (var i = 0; i < fbuffer.Length; i++)
                    {
                        if (fbuffer[i] < 0 || fbuffer[i] > 255)
                        {
                            Debug.Fail("fbuffer[i] < 0 || fbuffer[i] > 255");
                        }

                        buffer[i] = (byte)fbuffer[i];
                    }

                    fs.Write(buffer, 0, buffer.Length);
                    collecteddescriptorsrows += descriptors.Rows;
                    added++;
                    if (collecteddescriptorsrows > AppConsts.MaxDescriptorsForClustering)
                    {
                        break;
                    }
                }
            }

            progress.Report($"Gathering descriptors finished");
        }

        public void CalculateSiftClusters(IProgress<string> progress)
        {
            progress.Report($"Loading descriptors...");
            var buffer = File.ReadAllBytes(AppConsts.FileSiftDescriptors);
            progress.Report($"Calculating clusters...");
            var sw = new Stopwatch();
            sw.Start();
            using (var tw = new StreamWriter(AppConsts.FileSiftClusters))
            {
                var maxnodeid = 0;
                var snstart = new SiftNode(maxnodeid);
                var offset = 0;
                while (offset < buffer.Length)
                {
                    var sp = new SiftPoint(buffer, offset);
                    snstart.N.Add(sp);
                    offset += 128;
                }

                snstart.Update();
                var nodeslist = new List<SiftNode>
                {
                    snstart
                };

                while (nodeslist.Count < AppConsts.MaxClusters)
                {
                    var scopefrt = nodeslist.Where(e => e.Count > 10 && e.R0 == 0 && e.R1 == 0).ToArray();
                    if (scopefrt.Length == 0)
                    {
                        break;
                    }

                    var edgelengthmax = 0f;
                    var indexedgelengthmax = 0;
                    for (var i = 0; i < nodeslist.Count; i++)
                    {
                        if (nodeslist[i].EdgeLength > edgelengthmax)
                        {
                            edgelengthmax = nodeslist[i].EdgeLength;
                            indexedgelengthmax = i;
                        }
                    }

                    var nodeparent = nodeslist[indexedgelengthmax];
                    var node0 = new SiftNode(maxnodeid + 1);
                    nodeslist.Add(node0);
                    nodeparent.R0 = node0.NodeId;
                    var node1 = new SiftNode(maxnodeid + 2);
                    nodeslist.Add(node1);
                    nodeparent.R1 = node1.NodeId;
                    foreach (var sp in nodeparent.N)
                    {
                        if (sp.V[nodeparent.LongestEdge] <= nodeparent.EdgeMedian)
                        {
                            node0.N.Add(sp);
                        }
                        else
                        {
                            node1.N.Add(sp);
                        }
                    }

                    node0.Update();
                    node1.Update();

                    maxnodeid += 2;
                    nodeslist.RemoveAt(indexedgelengthmax);
                    if (nodeparent.NodeId > 0)
                    {
                        tw.WriteLine();
                    }

                    tw.Write($"{nodeparent.NodeId},0,{nodeparent.LongestEdge},{nodeparent.EdgeMedian},{nodeparent.R0},{nodeparent.R1},{nodeparent.Count}");
                    progress.Report($"nodeid:{nodeparent.NodeId} cid:0 le:{nodeparent.LongestEdge} em:{nodeparent.EdgeMedian} r0:{nodeparent.R0} r1:{nodeparent.R1} c:{nodeparent.Count}...");
                }

                var maxcid = 0;
                foreach (var node in nodeslist)
                {
                    if (node.NodeId > 0)
                    {
                        tw.WriteLine();
                    }

                    tw.Write($"{node.NodeId},{maxcid},{node.LongestEdge},{node.EdgeMedian},{node.R0},{node.R1},{node.Count}...");
                    progress.Report($"nodeid:{node.NodeId} cid:{maxcid} le:{node.LongestEdge} em:{node.EdgeMedian} r0:{node.R0} r1:{node.R1} c:{node.Count}...");
                    maxcid++;
                }
            }
            
            sw.Stop();
            progress.Report($"Clustering finished ({HelperConvertors.TimeIntervalToString(sw.Elapsed)})");
        }
    }
}
