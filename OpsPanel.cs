using static DynamicWallpaper.WallpaperPreviewPanel;

namespace DynamicWallpaper
{
    internal class OpsPanel: DoubleBufferPanel
    {
        private readonly IDictionary<string, string> monitors;
        public EventHandler<WallpaperOpsEventArgs> SetWallpaperEvent { get; set; }

        public EventHandler<WallpaperOpsEventArgs> DelWallpaperEvent { get; set; }

        private PictureBox moreBtn;

        public OpsPanel(IDictionary<string, string> monitors) { 
            
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Transparent;

            this.Size = WallpaperSetting.PreviewImgSize;

            this.monitors = monitors;

            InitComponent();

            BuildMoreOptsMenu(moreBtn);

        }

        private void InitComponent()
        {
            var flowPanel = new DoubleBufferFlowPanel()
            {
                Height = 80,
                Width = this.Width,
                BackColor = Color.FromArgb(100, Color.Black),
                FlowDirection = FlowDirection.LeftToRight,
                //WrapContents = false,
                //Dock = DockStyle.Bottom,
                //Padding = new Padding(0, 0, 0, 0),
                //Margin = new Padding(0, 0, 0, 0),
                AutoSize = true,
                //Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            };

            flowPanel.Top = (this.Height - flowPanel.Height) / 2;

            var love = new PictureBox
            {
                Image = ResourcesHelper.Instance.LoveImg,
                BackColor = Color.Transparent,
            };
            var broken = new PictureBox
            {
                Image = ResourcesHelper.Instance.BrokenImg,
                BackColor = Color.Transparent,
            };

            this.MouseLeave += (sender, e) =>
            {
                this.Parent = null;
            };

            flowPanel.Controls.Add(love);
            flowPanel.Controls.Add(broken);

            this.Controls.Add(flowPanel);

            // 在控件右上角，显示一个图标
            moreBtn = new PictureBox
            {
                Image = ResourcesHelper.Instance.MoreImg,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 15,
                Height = 15,
            };
            //  把图标定位在右上角
            moreBtn.Location = new Point(this.Width - moreBtn.Width - 5, 5);

            this.Controls.Add(moreBtn);
        }


        private void BuildMoreOptsMenu(Control icon)
        {

            // 创建一个菜单项
            var deleteItem = new ToolStripMenuItem("删除");

            // 为菜单项添加点击事件
            deleteItem.Click += (sender, args) =>
            {
                var wallpaper = ((WallpaperPreviewPanel)Parent.Parent)?.Wallpaper;
                if (wallpaper == null)
                {
                    return;
                }
                // 在此处添加删除逻辑
                DelWallpaperEvent?.Invoke(this, new WallpaperOpsEventArgs(wallpaper.Path));
            };

            // 创建一个菜单
            var menu = new ContextMenuStrip();

            // 把菜单项添加到菜单中
            menu.Items.Add(deleteItem);

            foreach (var monitor in monitors)
            {
                var setItem = new ToolStripMenuItem($"设置到{monitor.Key}");
                setItem.Click += (sender, args) =>
                {
                    var wallpaper = ((WallpaperPreviewPanel)Parent.Parent)?.Wallpaper;
                    if (wallpaper == null)
                    {
                        return;
                    }
                    // 在此处添加设置逻辑
                    SetWallpaperEvent?.Invoke(this, new WallpaperOpsEventArgs(wallpaper.Path, monitor.Value));
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
