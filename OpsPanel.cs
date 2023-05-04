using static DynamicWallpaper.WallpaperPreviewPanel;

namespace DynamicWallpaper
{
    /// <summary>
    /// 图片预览的操作面板
    /// </summary>
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
            

            var love = new DoubleBufferPictureBox
            {
                Image = ResourcesHelper.Instance.LoveImg,
                BackColor = Color.Transparent,
            };
            var broken = new DoubleBufferPictureBox
            {
                Image = ResourcesHelper.Instance.BrokenImg,
                BackColor = Color.Transparent,
            };

            love.Size = broken.Size = new Size(40, 40);

            centerPanel.Controls.Add(love);
            centerPanel.Controls.Add(broken);
            centerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            flowPanel.Controls.Add(centerPanel);

            //  计算居中偏移量
            centerPanel.Top = (flowPanel.Height - centerPanel.Height) / 2;
            centerPanel.Left = (flowPanel.Width - centerPanel.Width) / 2;


            this.Controls.Add(flowPanel);

            // 在控件右上角，显示一个图标
            moreBtn = new DoubleBufferPictureBox
            {
                Image = ResourcesHelper.Instance.MoreImg,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 15,
                Height = 15,
            };
            //  把图标定位在右上角
            moreBtn.Location = new Point(this.Width - moreBtn.Width - 5, 5);

            this.Controls.Add(moreBtn);

            //  鼠标离开时，把操作层隐藏
            this.MouseLeave += (sender, e) =>
            {
                this.Parent = null;
            };
        }


        private void BuildMoreOptsMenu(Control icon)
        {
            var delTxt = ResourcesHelper.GetString("Ops.Delete");
            var applyTxt = ResourcesHelper.GetString("Ops.Apply");

            // 创建一个菜单项
            var deleteItem = new ToolStripMenuItem(delTxt);

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
                var setItem = new ToolStripMenuItem($"{applyTxt}{monitor.Key}");
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
