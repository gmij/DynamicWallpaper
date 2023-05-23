using DynamicWallpaper.Tools;

namespace DynamicWallpaper
{
    internal class WallpaperPreviewPanel: DoubleBufferPanel
    {



        public WallpaperPreview Wallpaper { get; }

        private PictureBox pic;

        public string Id { get; private set; }

        public WallpaperPreviewPanel(WallpaperPreview wallpaper)
        {
            if (wallpaper == null) 
                throw new ArgumentNullException(nameof(wallpaper));

            this.Id = wallpaper.Id;

            if (wallpaper.Image == null) 
                throw new ArgumentNullException(nameof(wallpaper.Image));

            InitComponent(wallpaper.Image);
            this.Wallpaper = wallpaper;
        }

        private void InitComponent(Image img)
        {
            this.Size = WallpaperSetting.PreviewImgSize;

            this.BorderStyle = BorderStyle.FixedSingle;

            pic = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = img
            };

            pic.MouseEnter += (sender, e) => {
                this.OnMouseEnter(e);
            };

            //pic.Controls.Add(new OpsPanel());

            this.Controls.Add(pic);
        }

        public void ShowMaskPanel(Control mask)
        {
            mask.Parent = pic;
        }


        public class WallpaperOpsEventArgs
        {

            public WallpaperOpsEventArgs(string filePath)
            {
                FilePath = filePath;
            }

            public WallpaperOpsEventArgs(string filePath, string monitorId):this(filePath)
            {
                MonitorId = monitorId;
            }

            public string FilePath { get; }
            public string MonitorId { get; }
        }
    }


    class WallpaperLoadingPanel : WallpaperPreviewPanel
    {
        public WallpaperLoadingPanel(): base(new WallpaperLoading() { Image = ResourcesHelper.Instance.LoadingImg})
        {
            
        }
    }
}
