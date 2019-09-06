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

                    var scope = _imgList
                        .Where(e => _imgList.ContainsKey(e.Value.NextName) && !e.Value.Name.Equals(e.Value.NextName) && e.Value.LastView < e.Value.LastChanged)
                        .Select(e => e.Value)
                        .OrderByDescending(e => e.Sim)
                        .ToArray();

                    //var imgX = scope.OrderBy(e => e.LastView).FirstOrDefault();

                    
                    var minstars = scope.Min(e => e.Stars);
                    var imgX = scope.Where(e => e.Stars == minstars).FirstOrDefault();
                  

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
