﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ImageBank
{
    public partial class MainWindow
    {
        private double _picsMaxWidth;
        private double _picsMaxHeight;
        private double _labelMaxHeight;
        private BackgroundWorker _backgroundWorker;
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();

        private async void WindowLoaded()
        {
            BoxLeft.MouseDown += PictureLeftBoxMouseClick;
            BoxRight.MouseDown += PictureRightBoxMouseClick;

            LabelLeft.MouseDown += ButtonLeftNextMouseClick;

            Left = SystemParameters.WorkArea.Left + AppConsts.WindowMargin;
            Top = SystemParameters.WorkArea.Top + AppConsts.WindowMargin;
            Width = SystemParameters.WorkArea.Width - AppConsts.WindowMargin * 2;
            Height = SystemParameters.WorkArea.Height - AppConsts.WindowMargin * 2;
            Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - AppConsts.WindowMargin - Width) / 2;
            Top = SystemParameters.WorkArea.Top + (SystemParameters.WorkArea.Height - AppConsts.WindowMargin - Height) / 2;

            _picsMaxWidth = Grid.ActualWidth;
            _labelMaxHeight = LabelLeft.ActualHeight;
            _picsMaxHeight = Grid.ActualHeight - _labelMaxHeight;

            _notifyIcon.Icon = new Icon(@"app.ico");
            _notifyIcon.Visible = false;
            _notifyIcon.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                    _notifyIcon.Visible = false;
                    RedrawCanvas();
                };

            var progress = new Progress<string>(message => Status.Text = message);

            DisableElements();
            await Task.Run(() => { AppVars.Collection.Load(progress); });
            
            await Task.Run(() => { AppVars.Collection.Find(null, progress); });
            DrawCanvas();
            
            EnableElements();

            AppVars.SuspendEvent = new ManualResetEvent(true);

            _backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _backgroundWorker.DoWork += DoComputeSim;
            _backgroundWorker.ProgressChanged += DoComputeSimProgress;
            _backgroundWorker.RunWorkerAsync();
        }

        private void WindowClosing()
        {
            DisableElements();
            _backgroundWorker?.CancelAsync();
            EnableElements();
        }

        private void OnStateChanged()
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _notifyIcon.Visible = true;
            }
        }

        private void WindowSizeChanged()
        {
            RedrawCanvas();
        }

        private async void ImportClick()
        {
            var progress = new Progress<string>(message => Status.Text = message);
            DisableElements();
            await Task.Run(() => { AppVars.Collection.Import(progress); });
            await Task.Run(() => { AppVars.Collection.Find(null, progress); });
            DrawCanvas();
            EnableElements();
        }

        private void RotateLeftClick()
        {
            /*
            Transform(RotateFlipType.Rotate270FlipNone);
            */
        }

        private void RotateRightClick()
        {
            /*
            Transform(RotateFlipType.Rotate90FlipNone);
            */
        }

        private void MirrorHorizontalClick()
        {
            /*
            Transform(RotateFlipType.RotateNoneFlipX);
            */
        }

        private void MoveToRightClick()
        {
            var folder = AppVars.ImgPanel[1].Img.Folder;
            MoveToFolderClick(folder);
        }

        private async void MoveToFolderClick(string folder)
        {
            var progress = new Progress<string>(message => Status.Text = message);
            DisableElements();
            await Task.Run(() => { AppVars.Collection.MoveTo(AppVars.ImgPanel[0].Img.Name, folder); });
            await Task.Run(() => { AppVars.Collection.Find(null, progress); });
            DrawCanvas();
            EnableElements();
        }

        private void PictureLeftBoxMouseClick()
        {
            ImgPanelDelete(0);
        }

        private void PictureRightBoxMouseClick()
        {
            ImgPanelDelete(1);
        }

        private async void ButtonLeftNextMouseClick()
        {
            AppVars.ImgPanel[0].Img.LastView = DateTime.Now;
            AppVars.Collection.SqlUpdateLastView(AppVars.ImgPanel[0].Img.Name, AppVars.ImgPanel[0].Img.LastView);

            var progress = new Progress<string>(message => Status.Text = message);
            DisableElements();
            await Task.Run(() => { AppVars.Collection.Find(null, progress); });
            DrawCanvas();
            EnableElements();
        }

        private void DoComputeSimProgress(object sender, ProgressChangedEventArgs e)
        {
            BackgroundStatus.Text = (string)e.UserState;
        }

        private void DoComputeSim(object s, DoWorkEventArgs args)
        {
            while (!_backgroundWorker.CancellationPending)
            {
                var message = AppVars.Collection.ComputeSim();
                if (!string.IsNullOrEmpty(message))
                {
                    _backgroundWorker.ReportProgress(0, message);
                    _notifyIcon.BalloonTipText = message;
                }

                Thread.Sleep(100);
            }

            args.Cancel = true;
        }

        private void DisableElements()
        {
            ElementsEnable(false);
        }

        private void EnableElements()
        {
            ElementsEnable(true);
        }

        private void ElementsEnable(bool enabled)
        {
            foreach (System.Windows.Controls.MenuItem item in Menu.Items)
            {
                item.IsEnabled = enabled;
            }

            Status.IsEnabled = enabled;
            BoxLeft.IsEnabled = enabled;
            BoxRight.IsEnabled = enabled;
            LabelLeft.IsEnabled = enabled;
            LabelRight.IsEnabled = enabled;
        }

        private void DrawCanvas()
        {
            if (AppVars.ImgPanel[0] == null || AppVars.ImgPanel[1] == null)
            {
                return;
            }

            var pBoxes = new[] { BoxLeft, BoxRight };
            var pLabels = new[] { LabelLeft, LabelRight };

            for (var index = 0; index < 2; index++)
            {
                var name = AppVars.ImgPanel[index].Img.Name;
                pBoxes[index].Tag = name;
                pLabels[index].Tag = name;

                pBoxes[index].Source = HelperImages.ImageSourceFromBitmap(AppVars.ImgPanel[index].Bitmap);

                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(AppVars.ImgPanel[index].Img.Folder))
                {
                    sb.Append($"{AppVars.ImgPanel[index].Img.Folder}\\");
                }

                sb.Append($"{AppVars.ImgPanel[index].Img.Name}");

                var foldersize = AppVars.Collection.GetFolderSize(AppVars.ImgPanel[index].Img.Folder);
                if (foldersize > 0)
                {
                    sb.Append($" [{foldersize}]");
                }

                sb.Append($" {AppVars.ImgPanel[index].Img.Sim:F1}");

                sb.AppendLine();
                sb.Append($"{HelperConvertors.SizeToString(AppVars.ImgPanel[index].Size)} ({AppVars.ImgPanel[index].Bitmap.Width:F0}x{AppVars.ImgPanel[index].Bitmap.Height:F0})");

                var points = AppVars.ImgPanel[index].Img.Descriptors != null ? AppVars.ImgPanel[index].Img.Descriptors.Rows : 0;
                if (points > 0)
                {
                    sb.Append($" P{points}");
                }

                sb.AppendLine();
                sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(AppVars.ImgPanel[index].Img.LastView))} ago");
                sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(AppVars.ImgPanel[index].Img.LastUpdated))} ago]");

                pLabels[index].Text = sb.ToString();
                pLabels[index].Background =
                    HelperPath.IsLegacy(AppVars.ImgPanel[index].Img.Folder) ?
                    System.Windows.Media.Brushes.White :
                    System.Windows.Media.Brushes.Bisque;
            }

            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            var ws = new double[2];
            var hs = new double[2];
            for (var index = 0; index < 2; index++)
            {
                ws[index] = _picsMaxWidth / 2;
                hs[index] = _picsMaxHeight;
                if (AppVars.ImgPanel[index] != null && AppVars.ImgPanel[index].Bitmap != null)
                {
                    ws[index] = AppVars.ImgPanel[index].Bitmap.Width;
                    hs[index] = AppVars.ImgPanel[index].Bitmap.Height;
                }
            }

            var aW = _picsMaxWidth / (ws[0] + ws[1]);
            var aH = _picsMaxHeight / Math.Max(hs[0], hs[1]);
            var a = Math.Min(aW, aH);
            if (a > 1.0)
            {
                a = 1.0;
            }

            SizeToContent = SizeToContent.Manual;
            Grid.ColumnDefinitions[0].Width = new GridLength(ws[0] * a, GridUnitType.Pixel);
            Grid.ColumnDefinitions[1].Width = new GridLength(ws[1] * a, GridUnitType.Pixel);
            Grid.RowDefinitions[0].Height = new GridLength(Math.Max(hs[0], hs[1]) * a, GridUnitType.Pixel);
            Grid.Width = (ws[0] + ws[1]) * a;
            Grid.Height = Math.Max(hs[0], hs[1]) * a + _labelMaxHeight;
            SizeToContent = SizeToContent.WidthAndHeight;
            Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - AppConsts.WindowMargin - Width) / 2;
            Top = SystemParameters.WorkArea.Top + (SystemParameters.WorkArea.Height - AppConsts.WindowMargin - Height) / 2;
        }

        private async void ImgPanelDelete(int index)
        {
            var progress = new Progress<string>(message => Status.Text = message);
            DisableElements();
            await Task.Run(() => { AppVars.Collection.DeleteImgAndFile(AppVars.ImgPanel[index].Img.Name); });
            await Task.Run(() => { AppVars.Collection.Find(null, progress); });
            DrawCanvas();
            EnableElements();
        }
    }
}
