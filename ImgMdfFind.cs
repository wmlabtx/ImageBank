using System;
using System.Linq;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(string nameX, IProgress<string> progress)
        {
            progress.Report(string.Empty);
            while (true)
            {                
                if (string.IsNullOrEmpty(nameX))
                {
                    if (_imgList.Count == 0)
                    {
                        return;
                    }

                    Img imgX;

                    var scope1 = _imgList
                        .Where(e => _imgList.ContainsKey(e.Value.NextName)) 
                        .Select(e => e.Value)
                        .ToArray();

                    var scope2 = scope1
                        .Where(e => e.LastView < e.LastChanged)
                        .ToArray();

                    if (scope2.Length > 0)
                    {
                        imgX = scope2
                            .OrderByDescending(e => e.Sim)
                            .FirstOrDefault();
                    }
                    else
                    {
                        imgX = scope1
                            .OrderBy(e => e.LastView)
                            .FirstOrDefault();
                    }

                    nameX = imgX.Name;                    

                    AppVars.ImgPanel[0] = GetImgPanel(nameX);
                    if (AppVars.ImgPanel[0] == null)
                    {
                        progress.Report($"{_imgList.Count}: {nameX}: corrupted!");
                        nameX = null;
                        continue;
                    }
                }

                AppVars.ImgPanel[1] = GetImgPanel(AppVars.ImgPanel[0].Img.NextName);
                if (AppVars.ImgPanel[1] == null)
                {
                    var nameY = AppVars.ImgPanel[0].Img.NextName;
                    progress.Report($"{_imgList.Count}: {nameY}: corrupted!");

                    var imgX = AppVars.ImgPanel[0].Img;
                    ResetNextName(imgX);

                    continue;
                }
                
                break;
            }
        }

        /*
        private Img FindInRotation(Img[] scope)
        {
            var nodes = new SortedDictionary<string, DateTime>();
            foreach (var img in scope)
            {
                if (nodes.ContainsKey(img.Node))
                {
                    if (nodes[img.Node] < img.LastView)
                    {
                        nodes[img.Node] = img.LastView;
                    }
                }
                else
                {
                    nodes.Add(img.Node, img.LastView);
                }
            }

            var minnode = string.Empty;
            var minlastview = DateTime.Now;
            foreach (var node in nodes.Keys)
            {
                if (DateTime.Now.Subtract(nodes[node]).TotalHours < 1.0)
                {
                    continue;
                }

                if (nodes[node] < minlastview)
                {
                    minnode = node;
                    minlastview = nodes[node];
                }
            }

            var imgX = scope
                .Where(e => e.Node.Equals(minnode))
                .OrderBy(e => e.LastView)
                .FirstOrDefault();

            return imgX;
        }
        */
    }
}
