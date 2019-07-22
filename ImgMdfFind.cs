﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageBank
{
    public partial class ImgMdf
    {
        public void Find(string nameX, IProgress<string> progress)
        {
            while (true)
            {                
                if (string.IsNullOrEmpty(nameX))
                {
                    if (_imgList.Count == 0)
                    {
                        return;
                    }

                    Img imgX = null;

                    var scoperecentlyadded = _imgList
                        .Where(e => e.Value.LastView.Year < 2010 && _imgList.ContainsKey(e.Value.NextName))
                        .Select(e => e.Value)
                        .ToArray();

                    if (scoperecentlyadded.Length > 0)
                    {
                        imgX = scoperecentlyadded.OrderByDescending(e => e.LastChecked).FirstOrDefault();   
                    }
                    else
                    {
                        imgX = _imgList.OrderBy(e => e.Value.LastView).FirstOrDefault().Value;
                        var all = _imgList
                            .Where(e => _imgList.ContainsKey(e.Value.NextName))
                            .Select(e => e.Value)
                            .ToArray();

                        imgX = FindInRotation(all);
                    }

                    if (imgX == null)
                    {
                        nameX = null;
                        continue;
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

                var oldname = AppVars.ImgPanel[0].Img.NextName;
                var oldsim = AppVars.ImgPanel[0].Img.Sim;

                AppVars.ImgPanel[1] = GetImgPanel(AppVars.ImgPanel[0].Img.NextName);
                if (AppVars.ImgPanel[1] == null)
                {
                    var nameY = AppVars.ImgPanel[0].Img.NextName;
                    progress.Report($"{_imgList.Count}: {nameY}: corrupted!");

                    var imgX = AppVars.ImgPanel[0].Img;
                    imgX.NextName = imgX.Name;
                    imgX.Sim = 0f;
                    imgX.LastChecked = DateTime.Now.AddYears(-10);
                    SqlUpdateLink(imgX.Name, imgX.NextName, imgX.Sim, imgX.LastChecked);

                    continue;
                }

                var sb = new StringBuilder();
                sb.Append($"{_imgList.Count}: ");
                sb.Append($"{_avgTimes:F2}s");
                progress.Report(sb.ToString());
                break;
            }
        }

        private Img FindInRotation(Img[] scopeX)
        {
            var scope = scopeX;
            var level = 0;
            Img imgX = null;
            while (true)
            {
                var folders = new SortedDictionary<string, DateTime>();
                foreach (var img in scope)
                {
                    var pars = img.Folder.Split('\\');
                    var folder = pars[level];

                    if (folders.ContainsKey(folder))
                    {
                        if (folders[folder] < img.LastView)
                        {
                            folders[folder] = img.LastView;
                        }
                    }
                    else
                    {
                        folders.Add(folder, img.LastView);
                    }
                }

                var minfolder = AppConsts.FolderLegacy;
                var minlastview = DateTime.Now;
                foreach (var folder in folders.Keys)
                {
                    if (folders[folder] < minlastview)
                    {
                        minfolder = folder;
                        minlastview = folders[folder];
                    }
                }

                var scopenew = new List<Img>();
                foreach (var img in scope)
                {
                    var pars = img.Folder.Split('\\');
                    var folder = pars[level];

                    if (folder.Equals(minfolder))
                    {
                        scopenew.Add(img);
                    }
                }

                level++;
                var nextpars = scopenew[0].Folder.Split('\\');
                if (nextpars.Length <= level)
                {
                    imgX = scopenew.OrderBy(e => e.LastView).FirstOrDefault();
                    break;
                }
                else
                {
                    scope = scopenew.ToArray();
                }
            }

            return imgX;
        }
    }
}
