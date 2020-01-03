using System;
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

            AppVars.Progress = new Progress<string>(message => Status.Text = message);

            DisableElements();
            await Task.Run(() => { AppVars.Collection.Load(AppVars.Progress); });            
            await Task.Run(() => { AppVars.Collection.Find(null, AppVars.Progress); });
            DrawCanvas();
            
            EnableElements();

            AppVars.SuspendEvent = new ManualResetEvent(true);

            _backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _backgroundWorker.DoWork += DoComputeSim;
            _backgroundWorker.ProgressChanged += DoComputeSimProgress;
            //_backgroundWorker.RunWorkerAsync();
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
            DisableElements();
            await Task.Run(() => { AppVars.Collection.Import(AppVars.Progress); });
            await Task.Run(() => { AppVars.Collection.Find(null, AppVars.Progress); });
            DrawCanvas();
            EnableElements();
        }

        private async void ExportClick()
        {
            DisableElements();
            await Task.Run(() => { AppVars.Collection.Export(AppVars.Progress); });
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
            AppVars.Collection.UpdateView(AppVars.ImgPanel[0].Hash);

            DisableElements();
            await Task.Run(() => { AppVars.Collection.Find(null, AppVars.Progress); });
            DrawCanvas();
            EnableElements();
        }

        private async void ButtonRightNextMouseClick()
        {
            AppVars.Collection.UpdateView(AppVars.ImgPanel[0].Hash);

            DisableElements();
            await Task.Run(() => { AppVars.Collection.Find(null, AppVars.Progress); });
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
                AppVars.Collection.ComputeSim(_backgroundWorker);
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
                var hash = AppVars.ImgPanel[index].Hash;
                pBoxes[index].Tag = hash;
                pLabels[index].Tag = hash;

                pBoxes[index].Source = HelperImages.ImageSourceFromBitmap(AppVars.ImgPanel[index].Bitmap);

                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(AppVars.ImgPanel[index].Folder))
                {
                    sb.Append($"{AppVars.ImgPanel[index].Folder}\\");
                }
                
                sb.Append($"{AppVars.ImgPanel[index].Hash}");
                if (AppVars.ImgPanel[index].FolderSize > 1)
                {
                    sb.Append($" [{AppVars.ImgPanel[index].FolderSize}]");
                }

                sb.AppendLine();
                
                sb.Append($"{HelperConvertors.SizeToString(AppVars.ImgPanel[index].Length)} ");
                sb.Append($" ({ AppVars.ImgPanel[index].Bitmap.Width}x{AppVars.ImgPanel[index].Bitmap.Height})");
                sb.AppendLine();

                sb.Append($"{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(AppVars.ImgPanel[index].LastView))} ago");
                sb.Append($" [{HelperConvertors.TimeIntervalToString(DateTime.Now.Subtract(AppVars.ImgPanel[index].LastCheck))} ago]");

                pLabels[index].Text = sb.ToString();
                pLabels[index].Background =
                    string.IsNullOrEmpty(AppVars.ImgPanel[index].Folder) ?
                    System.Windows.Media.Brushes.Bisque :
                    System.Windows.Media.Brushes.White;
            }

            var f0 = AppVars.ImgPanel[0].Folder;
            var f1 = AppVars.ImgPanel[1].Folder;
            if (!string.IsNullOrEmpty(f0) && !string.IsNullOrEmpty(f1))
            {
                if (f1.IndexOf(f0) == 0)
                {
                    pLabels[1].Background = System.Windows.Media.Brushes.LightGreen;
                    if (f0.Equals(f1))
                    {
                        pLabels[0].Background = System.Windows.Media.Brushes.LightGreen;
                    }
                    else
                    {
                        pLabels[0].Background = System.Windows.Media.Brushes.Yellow;
                    }
                }
                else
                {
                    if (f0.IndexOf(f1) == 0)
                    {
                        pLabels[0].Background = System.Windows.Media.Brushes.LightGreen;
                        if (f0.Equals(f1))
                        {
                            pLabels[1].Background = System.Windows.Media.Brushes.LightGreen;
                        }
                        else
                        {
                            pLabels[1].Background = System.Windows.Media.Brushes.Yellow;
                        }
                    }
                }
            }

            if (AppVars.ImgPanel[0].Hash.Equals(AppVars.ImgPanel[1].Hash))
            {
                pLabels[0].Background = System.Windows.Media.Brushes.LightGray;
                pLabels[1].Background = System.Windows.Media.Brushes.LightGray;
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
            DisableElements();
            await Task.Run(() => { AppVars.Collection.Delete(AppVars.ImgPanel[index].Hash); });
            await Task.Run(() => { AppVars.Collection.Find(null, AppVars.Progress); });
            DrawCanvas();
            EnableElements();
        }

        private async void MoveToNodeClick(string folder)
        {
            DisableElements();
            await Task.Run(() => { AppVars.Collection.MoveTo(AppVars.ImgPanel[0].Hash, folder); });
            await Task.Run(() => { AppVars.Collection.Find(AppVars.ImgPanel[0].Hash, AppVars.Progress); });
            DrawCanvas();
            EnableElements();
        }

        private async void GatherSiftDescriptorsClick()
        {
            DisableElements();
            await Task.Run(() => { AppVars.Collection.GatherSiftDescriptors(AppVars.Progress); });
            DrawCanvas();
            EnableElements();
        }

        private async void CalculateSiftClustersClick()
        {
            DisableElements();
            await Task.Run(() => { AppVars.Collection.CalculateSiftClusters(AppVars.Progress); });
            DrawCanvas();
            EnableElements();
        }
    }
}
