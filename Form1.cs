using DynamicWallpaper.Impl;
using System.Windows.Forms;

namespace DynamicWallpaper
{
    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            DoubleBuffered = true;
        }
    }

    public class DoubleBufferFlowPanel: FlowLayoutPanel
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
        private DoubleBufferPanel? _deletePanel;

        private Size _imageSize;


        public SettingForm(WallpaperManager paperManager, ResourcesHelper rh, IEnumerable<INetworkPaperProvider> paperProviders)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            _imageSize = new Size(220, 180);
            _paperManager = paperManager;
            _paperManager.WallpaperChanged += WhenWallpaperChanged;
            this.rh = rh;
            InitPreviewImages();
            InitializeDeletePanel();
            foreach(var provider in paperProviders)
            {
                flowLayoutPanel2.Controls.Add(new TreasureChestPanel(provider));
            }
        }

        private void WhenWallpaperChanged(object? sender, EventArgs e)
        {
            Thread.Sleep(2000);
            InitPreviewImages();
        }

        private void DownWallpaper(object? sender, EventArgs e)
        {
            _paperManager.GetInternetWallpaper();
        }

        private void AddPic(PictureBox pic)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() =>
                {
                    flowLayoutPanel1.Controls.Add(pic);
                });
            }
            else
            {
                flowLayoutPanel1.Controls.Add(pic);
            }
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
                    var pic = new PictureBox
                    {
                        Image = preview.Image,
                        Size = _imageSize,
                        BorderStyle = BorderStyle.FixedSingle,
                        WaitOnLoad = false,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Tag = preview
                    };

                    pic.MouseEnter += Pic_MouseHover;
                    AddPic(pic);
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