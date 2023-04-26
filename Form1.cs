using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DynamicWallpaper
{
    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            DoubleBuffered = true;
        }
    }

    public class DoubleBufferFlowPanel : FlowLayoutPanel
    {
        public DoubleBufferFlowPanel()
        {
            DoubleBuffered = true;
        }
    }


    internal partial class SettingForm : Form
    {

        private readonly WallpaperManager _paperManager;
        private readonly ResourcesHelper rh;
        private readonly ILogger logger;
        private DoubleBufferPanel? _deletePanel;

        private Size _imageSize;

        public SettingForm(WallpaperManager paperManager, ResourcesHelper rh, IEnumerable<INetworkPaperProvider> paperProviders, ILogger<SettingForm> logger)
        {
            InitializeComponent();

            this.logger = logger;
            this.rh = rh;
            
            CheckForIllegalCrossThreadCalls = false;

            _imageSize = new Size(220, 180);
            _paperManager = paperManager;
            _paperManager.WallpaperChanged += WhenWallpaperChanged;
            InitPreviewImages();
            //InitializeDeletePanel();
            foreach (var provider in paperProviders)
            {
                flowLayoutPanel2.Controls.Add(new TreasureChestPanel(provider));
            }
        }

        private void WhenWallpaperChanged(object? sender, WallpaperChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case WatcherChangeTypes.Created:
                    AddPic(e.Data);
                    break;
                case WatcherChangeTypes.Deleted:
                    DelPic(e.Data);
                    break;
                case WatcherChangeTypes.Changed:
                case WatcherChangeTypes.Renamed:
                    InitPreviewImages();
                    break;
            }

        }

        private void DownWallpaper(object? sender, EventArgs e)
        {
            _paperManager.GetInternetWallpaper();
        }

        private void DelPic(WallpaperPreview preview)
        {
            var pic = flowLayoutPanel1.Controls.Find(preview.Id, false).FirstOrDefault();

            var clist = flowLayoutPanel1.Controls;
            foreach (WallpaperPreviewPanel control in clist)
            {
                if (control.Id == preview.Id)
                {
                    ControlInvoke(() => flowLayoutPanel1.Controls.Remove(control));
                    break;
                }
            }
        }

        private void ControlInvoke(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() =>
                {
                    action();
                });
            }
            else
            {
                action();
            }
        }

        private void AddPic(WallpaperPreview preview)
        {

            var pic = new WallpaperPreviewPanel(preview);
            pic.DelWallpaperEvent = (s, e) =>
            {
                var path = e.FilePath;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            };
            ControlInvoke(() => flowLayoutPanel1.Controls.Add(pic));
            
        }


        private void InitializeDeletePanel()
        {
            _deletePanel = new DoubleBufferPanel();
            _deletePanel.BackColor = Color.FromArgb(40, Color.Gray);
            PictureBox deleteIcon = new()
            {
                BackColor = Color.Transparent,
                Image = rh.ScreenImg,
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                Margin = new Padding(10, 5, 10, 5)
            };
            deleteIcon.Click += (s, e) =>
            {
                var pic = (s as PictureBox)?.Parent?.Parent as PictureBox;
                var path = (pic?.Tag as WallpaperPreview)?.Path;
                if (File.Exists(path))
                {
                    File.Delete(path);
                    flowLayoutPanel1.Controls.Remove(pic);
                }
            };
            _deletePanel.Controls.Add(deleteIcon);
        }

        private void SetPanelSize(Control parent)
        {
            if (_deletePanel != null)
            {
                _deletePanel.Parent = parent;
                _deletePanel.Size = new Size(parent.Width, parent.Height / 2);
                _deletePanel.Top = (parent.Height - _deletePanel.Height) / 2;
                _deletePanel.Focus();
            }
        }

        private void InitPreviewImages()
        {
            if (_paperManager != null)
            {
                var previews = _paperManager.GetWallpaperPreviews();

                flowLayoutPanel1.Controls.Clear();

                foreach (var preview in previews)
                {
                   AddPic(preview);
                }
            }
        }

        private void Pic_MouseHover(object? sender, EventArgs e)
        {
            if (sender is PictureBox pic)
                SetPanelSize(pic);
        }

    }
}