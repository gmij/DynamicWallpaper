using System.Windows.Forms;

namespace DynamicWallpaper
{
    internal partial class SettingForm : Form
    {

        /// <summary>
        ///  ����Panel��˫����
        /// </summary>
        public class DoubleBufferPanel : Panel
        {
            public DoubleBufferPanel()
            {
                DoubleBuffered = true;
            }
        }

        /// <summary>
        /// ��ֽ��
        /// </summary>
        private readonly WallpaperManager _paperManager;
        private readonly ResourcesHelper rh;
        private DoubleBufferPanel? _deletePanel;

        private Size _imageSize;

        private string _currentPaper;

        public SettingForm(WallpaperManager paperManager, ResourcesHelper rh)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            _imageSize = new Size(220, 180);
            _paperManager = paperManager;
            _paperManager.WallpaperChanged += WhenWallpaperChanged;
            this.rh = rh;
            BindControls();
            //InitDownLoadWallpaper();
            InitPreviewImages();
            InitializeDeletePanel();
        }

        private void WhenWallpaperChanged(object? sender, EventArgs e)
        {
            Thread.Sleep(2000);
            InitPreviewImages();
        }

        private void InitDownLoadWallpaper()
        {
            var img = new PictureBox() { Image = rh.RefreshImg, Size = _imageSize };
            img.Click += DownWallpaper;
            flowLayoutPanel1.Controls.Add(img);
        }

        private void DownWallpaper(object? sender, EventArgs e)
        {
            _paperManager.GetInternetWallpaper();
        }

        private void BindControls()
        {
            //  ���´���ΪCoplit����
            this.cmbMonitor.Items.Clear();
            cmbMonitor.DataSource = _paperManager.Monitors.ToList();
            cmbMonitor.DisplayMember = "Key";
            cmbMonitor.ValueMember = "Value";
            cmbMonitor.SelectedIndex = 0;

            this.cmbProvider.Items.Clear();
            cmbProvider.DataSource = _paperManager.Providers.ToList();
            cmbProvider.DisplayMember = "Key";
            cmbProvider.ValueMember = "Value";
            cmbProvider.SelectedIndex = 0;
        }

        private void InitializeDeletePanel()
        {
            // ����ɾ��Panel
            _deletePanel = new DoubleBufferPanel();
            //_deletePanel.Dock = DockStyle.Fill;
            _deletePanel.BackColor = Color.FromArgb(40, Color.Gray);
            // ���ɾ��ͼ��
            PictureBox deleteIcon = new PictureBox();
            deleteIcon.BackColor = Color.Transparent;
            deleteIcon.Image = rh.ScreenImg;
            deleteIcon.SizeMode = PictureBoxSizeMode.Zoom;
            deleteIcon.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            deleteIcon.Margin = new Padding(10, 5, 10, 5);
            // ���ɾ���¼�,ɾ��ͼƬ������ΪCoplit����
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
                //_deletePanel.Dock = DockStyle.Fill;
                _deletePanel.Size = new Size(parent.Width, parent.Height / 2);
                _deletePanel.Top = (parent.Height - _deletePanel.Height) / 2;
                _deletePanel.Focus();
            }
        }

        //  �ӱ�ֽ���л�ȡ���б���ͼƬ��Ԥ����Ϣ�����ڽ�������ʾ����
        private void InitPreviewImages()
        {
            if (_paperManager != null)
            {
                var previews = _paperManager.GetWallpaperPreviews();

                flowLayoutPanel1.Controls.Clear();
                InitDownLoadWallpaper();

                // ��Ԥ��ͼ���ص�flowLayoutPanel1��
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
                    //pic.MouseLeave += Pic_MouseLeave;
                    //  ����ΪCoplit����, ���ڴ�����̷߳��ʿؼ�������
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
            }
        }

        private void Pic_MouseHover(object? sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            SetPanelSize(pic);
            //_currentPaper = (pic.Tag as WallpaperPreview)?.Path ?? string.Empty;
        }

    }
}