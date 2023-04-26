using Microsoft.Extensions.Logging;

namespace DynamicWallpaper
{



    internal partial class SettingForm : Form
    {

        private readonly WallpaperManager paperManager;
        private readonly ILogger logger;
        private OpsPanel opsPanel;


        public SettingForm(WallpaperManager paperManager, ResourcesHelper rh, IEnumerable<INetworkPaperProvider> paperProviders, ILogger<SettingForm> logger)
        {
            InitializeComponent();

            this.logger = logger;


            CheckForIllegalCrossThreadCalls = false;

            opsPanel = new OpsPanel(paperManager.Monitors);
            opsPanel.DelWallpaperEvent += (s, e) =>
            {
                paperManager.DeleteWallpaper(e.FilePath);
            };
            opsPanel.SetWallpaperEvent += (s, e) =>
            {
                if (e.MonitorId == "all")
                {
                    paperManager.ChangeWallpaper(e.FilePath);
                }
                else
                {
                    paperManager.ChangeWallpaper(e.FilePath, e.MonitorId);
                }
            };
            this.Controls.Add(opsPanel);

            this.paperManager = paperManager;
            this.paperManager.WallpaperChanged += WhenWallpaperChanged;
            InitPreviewImages();
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
            paperManager.GetInternetWallpaper();
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
            pic.MouseEnter += (s, e) =>
            {
                pic.ShowMaskPanel(opsPanel);
            };
            ControlInvoke(() => flowLayoutPanel1.Controls.Add(pic));

        }


        private void InitPreviewImages()
        {
            if (paperManager != null)
            {
                var previews = paperManager.GetWallpaperPreviews();

                flowLayoutPanel1.Controls.Clear();

                foreach (var preview in previews)
                {
                    AddPic(preview);
                }
            }
        }


    }
}