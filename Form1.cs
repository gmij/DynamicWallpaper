using DynamicWallpaper.Impl;
using Microsoft.Extensions.Logging;

namespace DynamicWallpaper
{



    internal partial class SettingForm : Form
    {

        private readonly WallpaperManager paperManager;
        private OpsPanel opsPanel;


        public SettingForm(WallpaperManager paperManager, ResourcesHelper rh, IEnumerable<INetworkPaperProvider> paperProviders, ILogger<SettingForm> logger)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            opsPanel = new OpsPanel(paperManager.Monitors);

            


            this.Controls.Add(opsPanel);

            this.paperManager = paperManager;

            EventBus.Subscribe("WallPaperChanged", WhenWallpaperChanged);

            EventBus.Subscribe("WallPaperChanged", args =>
            {
                //  用于版本，下载失败或存在同名资源时，消除Loading图标
                if (args is ResourceExistsEventArgs)
                    Remove_PreviewLoading();
            });

            EventBus.Subscribe("DelWallpaper", e =>
            {
                paperManager.DeleteWallpaper(e.GetData<WallpaperPreviewPanel.WallpaperOpsEventArgs>().FilePath);
            });

            EventBus.Subscribe("SetWallpaper", args =>
            {
                var e = args.GetData<WallpaperPreviewPanel.WallpaperOpsEventArgs>();
                if (e.MonitorId == "all")
                {
                    paperManager.ChangeWallpaper(e.FilePath);
                }
                else
                {
                    paperManager.ChangeWallpaper(e.FilePath, e.MonitorId);
                }
            });


            //this.paperManager.WallpaperChanged += WhenWallpaperChanged;

            InitPreviewImages();
            foreach (var provider in paperProviders)
            {
                flowLayoutPanel2.Controls.Add(new TreasureChestPanel(provider, AddBox_PreviewLoading));
            }
        }

        private void AddBox_PreviewLoading(object? sender, int e)
        {
            for (int i = 0; i < e; i++)
            {
                flowLayoutPanel1.Controls.Add(new WallpaperLoadingPanel());
            }
        }

        private void Remove_PreviewLoading()
        {
            foreach (var control in flowLayoutPanel1.Controls)
            {
                if (control is WallpaperLoadingPanel)
                {
                    flowLayoutPanel1.Controls.Remove(control as WallpaperLoadingPanel);
                    break;
                }
            }
        }

        private void WhenWallpaperChanged(CustomEventArgs args)
        {
            var e = args.GetData<WallpaperChangedEventArgs>();
            if (e == null) 
            { 
                return; 
            }
            switch (e.Mode)
            {
                case WatcherChangeTypes.Created:
                    AddPic(e.Data);
                    Remove_PreviewLoading();
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


        internal void Center()
        {
            this.CenterToScreen();
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