using DynamicWallpaper.Tools;
using static DynamicWallpaper.WallpaperPreviewPanel;

namespace DynamicWallpaper
{
    /// <summary>
    /// 图片预览的操作面板
    /// </summary>
    internal class OpsPanel: DoubleBufferPanel
    {
        private readonly IDictionary<string, string> monitors;
        
        private PictureBox moreBtn;
        private DoubleBufferPictureBox loveIcon;

        public OpsPanel(IDictionary<string, string> monitors) { 
            
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Transparent;

            this.Size = WallpaperSetting.PreviewImgSize;

            this.monitors = monitors;

            loveIcon = new DoubleBufferPictureBox();
            moreBtn = new DoubleBufferPictureBox();

            InitComponent();

            BuildMoreOptsMenu(moreBtn);


        }

        private static void RegisterEvent()
        {
            EventBus.Register("DelWallpaper");
            EventBus.Register("SetWallpaper");
            EventBus.Register("SetLockScreen");

            EventBus.Register("LovePaper");
            EventBus.Register("NoLovePaper");
            EventBus.Register("BrokenPaper");
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            try
            {
                var dataItem = DataItem;
                if (dataItem == null || string.IsNullOrEmpty(dataItem.Path)) return;
                if (FavoriteWallpaperStorage.Contains(dataItem.Path))
                {
                    loveIcon.Image = ResourcesHelper.Instance.LovedImg;
                    loveIcon.Click -= Love_Click;
                    loveIcon.Click += Loved_Click;
                }
                else
                {
                    loveIcon.Image = ResourcesHelper.Instance.LoveImg;
                    loveIcon.Click -= Loved_Click;
                    loveIcon.Click += Love_Click;
                }
            }
            catch { }
        }

        private void InitComponent()
        {
            // 加入中间的半透明遮罩面板
            var flowPanel = new DoubleBufferPanel()
            {
                Height = 80,
                Width = this.Width,
                BackColor = Color.FromArgb(100, Color.Black),
                AutoSize = true,
            };

            flowPanel.Top = (this.Height - flowPanel.Height) / 2;

            //  加入一个居中面板，用于计算父层的顶部和左部的偏移量
            var centerPanel = new DoubleBufferFlowPanel()
            {
                Height = 40,
                Width = 100,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
            };


            loveIcon.Image = ResourcesHelper.Instance.LoveImg;
            loveIcon.BackColor = Color.Transparent;

            loveIcon.Click += Love_Click;
            var broken = new DoubleBufferPictureBox
            {
                Image = ResourcesHelper.Instance.BrokenImg,
                BackColor = Color.Transparent,
            };
            broken.Click += Broken_Click;

            loveIcon.Size = broken.Size = new Size(40, 40);

            centerPanel.Controls.Add(loveIcon);
            centerPanel.Controls.Add(broken);
            centerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            flowPanel.Controls.Add(centerPanel);

            //  计算居中偏移量
            centerPanel.Top = (flowPanel.Height - centerPanel.Height) / 2;
            centerPanel.Left = (flowPanel.Width - centerPanel.Width) / 2;


            this.Controls.Add(flowPanel);

            // 在控件右上角，显示一个图标
            moreBtn.Image = ResourcesHelper.Instance.MoreImg;
            moreBtn.SizeMode = PictureBoxSizeMode.Zoom;
            moreBtn.Width = 15;
            moreBtn.Height = 15;

            //  把图标定位在右上角
            moreBtn.Location = new Point(this.Width - moreBtn.Width - 5, 5);

            this.Controls.Add(moreBtn);

            //  鼠标离开时，把操作层隐藏
            this.MouseLeave += (sender, e) =>
            {
                this.Parent = null;
            };
        }

        private void Loved_Click(object? sender, EventArgs e)
        {
            loveIcon.Image = ResourcesHelper.Instance.LoveImg;
            loveIcon.Click -= Loved_Click;
            loveIcon.Click += Love_Click;
            EventBus.Publish("NoLovePaper", new CustomEventArgs(DataItem));
        }

        private void Broken_Click(object? sender, EventArgs e)
        {
            EventBus.Publish("BrokenPaper", new CustomEventArgs(DataItem));
        }

        private void Love_Click(object? sender, EventArgs e)
        {
            loveIcon.Image = ResourcesHelper.Instance.LovedImg;
            loveIcon.Click -= Love_Click;
            loveIcon.Click += Loved_Click;
            EventBus.Publish("LovePaper", new CustomEventArgs(DataItem));
        }

        private WallpaperPreview? DataItem => (Parent?.Parent as WallpaperPreviewPanel)?.Wallpaper;

        private void BuildMoreOptsMenu(Control icon)
        {
            var delTxt = ResourcesHelper.GetString("Ops.Delete");
            var applyTxt = ResourcesHelper.GetString("Ops.Apply");

            var screenTxt = ResourcesHelper.GetString("Ops.LockScreen");

            // 创建一个菜单项
            var deleteItem = new ToolStripMenuItem(delTxt);

            // 为菜单项添加点击事件
            deleteItem.Click += (sender, args) =>
            {
                var wallpaper = DataItem;
                if (wallpaper == null || string.IsNullOrEmpty(wallpaper.Path))
                {
                    return;
                }
                // 在此处添加删除逻辑
                EventBus.Publish("DelWallpaper", new CustomEventArgs(new WallpaperOpsEventArgs(wallpaper.Path)));
            };

            // 创建一个菜单
            var menu = new ContextMenuStrip();

            // 把菜单项添加到菜单中
            menu.Items.Add(deleteItem);

            // 创建一个锁屏菜单项
            var lockScreenItem = new ToolStripMenuItem($"{applyTxt}{screenTxt}");
            menu.Items.Add(lockScreenItem);

            // 为菜单项添加点击事件
            lockScreenItem.Click += (sender, args) =>
            {
                var wallpaper = DataItem;
                if (wallpaper == null || string.IsNullOrEmpty(wallpaper.Path))
                {
                    return;
                }
                EventBus.Publish("SetLockScreen", new CustomEventArgs(new WallpaperOpsEventArgs(wallpaper.Path)));
            };

            foreach (var monitor in monitors)
            {
                var setItem = new ToolStripMenuItem($"{applyTxt}{monitor.Key}");
                setItem.Click += (sender, args) =>
                {
                    var wallpaper = DataItem;
                    if (wallpaper == null || string.IsNullOrEmpty(wallpaper.Path))
                    {
                        return;
                    }
                    EventBus.Publish("SetWallpaper", new CustomEventArgs(new WallpaperOpsEventArgs(wallpaper.Path, monitor.Value)));
                };
                menu.Items.Add(setItem);
            }


            // 为图标添加点击事件
            icon.Click += (sender, args) =>
            {
                // 在图标位置弹出菜单
                menu.Show(icon, new Point(0, icon.Height));
            };
        }

    }
}
