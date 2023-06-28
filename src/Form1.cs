using DynamicWallpaper.Tools;
using DynamicWallpaper.TreasureChest;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DynamicWallpaper
{



    internal partial class SettingForm : Form
    {

        private readonly WallpaperManager paperManager;
        //private readonly IEnumerable<ITreasureChest> treasures;
        private OpsPanel opsPanel;


        public SettingForm(WallpaperManager paperManager, IEnumerable<ITreasureChest> treasures, ILogger<SettingForm> logger, ProgressLog log)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            opsPanel = new OpsPanel(paperManager.Monitors);

            InitLocalize();

            EventBus.Subscribe(EventName.SwitchLang, args => InitLocalize());


            this.Controls.Add(opsPanel);

            this.paperManager = paperManager;
            //this.treasures = treasures;
            EventBus.Subscribe(EventName.WallPaperChanged, WhenWallpaperChanged);

            EventBus.Subscribe(EventName.WallPaperChanged, args =>
            {
                //  用于版本，下载失败或存在同名资源时，消除Loading图标
                if (args is ResourceExistsEventArgs)
                {
                    logger.LogInformation("清除无效图标");
                    Remove_PreviewLoading();
                }
            });

            EventBus.Subscribe(EventName.DownFail, args =>
            {
                if (args is ResourceDownloadFailEventArgs arg)
                {
                    Remove_PreviewLoading(arg.GetData<int>());
                }
            });

            EventBus.Subscribe(EventName.DelWallpaper, e =>
            {
                if (e!= null)
                    paperManager.DeleteWallpaper(e.GetData<WallpaperPreviewPanel.WallpaperOpsEventArgs>()?.FilePath);
            });

            EventBus.Subscribe(EventName.SetWallpaper, args =>
            {
                var e = args.GetData<WallpaperPreviewPanel.WallpaperOpsEventArgs>();
                if (e != null)
                {
                    if (e.MonitorId == "all")
                    {
                        paperManager.ChangeWallpaper(e.FilePath);
                    }
                    else
                    {
                        paperManager.ChangeWallpaper(e.FilePath, e.MonitorId);
                    }
                }
            });

            EventBus.Subscribe(EventName.SetLockScreen, args =>
            {
                var e = args.GetData<WallpaperPreviewPanel.WallpaperOpsEventArgs>();
                if (e != null)
                    paperManager.SetLockScreenImage(e.FilePath, true);
            });

            EventBus.Subscribe(EventName.BoxRandom, AddBox_PreviewLoading);


            //this.paperManager.WallpaperChanged += WhenWallpaperChanged;

            InitPreviewImages();
            //foreach (var provider in paperProviders)
            //{
            //    flowLayoutPanel2.Controls.Add(new TreasureChestPanel(provider, AddBox_PreviewLoading));
            //}
            foreach (var provider in treasures)
            {
                flowLayoutPanel2.Controls.Add(provider.Panel);
            }
        }

        private void InitLocalize()
        {
            tabPage1.Text = ResourcesHelper.GetString("PaperPool");
            tabPage2.Text = ResourcesHelper.GetString("TreasureChest");
            Text = ResourcesHelper.GetString("SettingFormTitle");

            Icon = ResourcesHelper.Instance.MainImg;

            tabPage3.Text = ResourcesHelper.GetString("Support");
            linkLabel1.Text = ResourcesHelper.GetString("Issue");
            label2.Text = ResourcesHelper.GetString("Reward");
            label1.Text = ResourcesHelper.GetString("Chat");

            tabPage4.Text = ResourcesHelper.GetString("Log");
        }

        private async void AddBox_PreviewLoading(CustomEventArgs e)
        {
            var data = e.GetData<int>();
            for (int i = 0; i < data; i++)
            {
                await ControlInvokeAsync(() => flowLayoutPanel1.Controls.Add(new WallpaperLoadingPanel()));
                
            }
        }

        private void Remove_PreviewLoading(int i = 1)
        {
            var k = 0;
            lock (flowLayoutPanel1.Controls)
            {

                var count = flowLayoutPanel1.Controls.Count;
                for (var end = count - 1; end > -1; end--)
                {
                    if (flowLayoutPanel1.Controls[end] is WallpaperLoadingPanel loadingPanel)
                    {
                        flowLayoutPanel1.Controls.Remove(loadingPanel);
                        if (++k == i)
                            break;
                    }

                }


                //foreach (var control in flowLayoutPanel1.Controls)
                //{
                //    if (control is WallpaperLoadingPanel loadingPanel)
                //    {
                //        flowLayoutPanel1.Controls.Remove(loadingPanel);
                //        if (++k < i)
                //            break;
                //    }
                //}

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

        private async void DelPic(WallpaperPreview preview)
        {
            var pic = flowLayoutPanel1.Controls.Find(preview.Id, false).FirstOrDefault();

            var clist = flowLayoutPanel1.Controls;
            foreach (WallpaperPreviewPanel control in clist)
            {
                if (control.Id == preview.Id)
                {
                    await ControlInvokeAsync(() => flowLayoutPanel1.Controls.Remove(control));
                }
            }
        }

        //  以下函数由Cursor进行改进，用于解决界面主线程阻塞问题
        private async Task ControlInvokeAsync(Action action)
        {
            if (this.InvokeRequired)
            {
                await Task.Factory.FromAsync(this.BeginInvoke(new Action(() =>
                {
                    action();
                })), this.EndInvoke);
            }
            else
            {
                await Task.Yield();
                action();
            }
        }

        private async void AddPic(WallpaperPreview preview)
        {

            var pic = new WallpaperPreviewPanel(preview);
            pic.MouseEnter += (s, e) =>
            {
                pic.ShowMaskPanel(opsPanel);
            };
            await ControlInvokeAsync(() => flowLayoutPanel1.Controls.Add(pic));

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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/gmij/DynamicWallpaper/issues") { UseShellExecute = true });
        }

        private void toolStripStatusLabel1_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }
    }
}