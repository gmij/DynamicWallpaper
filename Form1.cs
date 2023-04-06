using System.Reflection;
using System.Windows.Forms;

namespace DynamicWallpaper
{
    public partial class SettingForm : Form
    {
        /// <summary>
        /// ��ֽ��
        /// </summary>
        private readonly IWallPaperPool _paperPool;
        private readonly ResourcesHelper rh;
        private bool _mouseEnter = false;
        private Panel _deletePanel;

        public SettingForm(IWallPaperPool paperPool, ResourcesHelper rh)
        {
            InitializeComponent();
            _paperPool = paperPool;
            this.rh = rh;
            InitPreviewImages();
            InitializeDeletePanel();
        }

        private void InitializeDeletePanel()
        {
            // ����ɾ��Panel
            _deletePanel = new Panel();
            _deletePanel.Size = new Size(30, 30);
            _deletePanel.BackColor = Color.Transparent;
            // ���ɾ��ͼ��
            PictureBox deleteIcon = new PictureBox();
            deleteIcon.Image = rh.ScreenImg;
            deleteIcon.SizeMode = PictureBoxSizeMode.Zoom;
            deleteIcon.Dock = DockStyle.Fill;
            // ���ɾ���¼�,ɾ��ͼƬ������ΪCoplit����
            deleteIcon.Click += (s, e) =>
            {
                var pic = (s as PictureBox)?.Parent as PictureBox;
                var path = (pic?.Tag as WallpaperPreview)?.Path;
                if (File.Exists(path))
                {
                    File.Delete(path);
                    flowLayoutPanel1.Controls.Remove(pic);
                }
            };

            _deletePanel.Controls.Add(deleteIcon);
        }

        //  �ӱ�ֽ���л�ȡ���б���ͼƬ��Ԥ����Ϣ�����ڽ�������ʾ����
        private void InitPreviewImages()
        {
            if (_paperPool != null)
            {
                var previews = _paperPool.GetLocalWallpaperPreviews();
                // ��Ԥ��ͼ���ص�flowLayoutPanel1��
                foreach (var preview in previews)
                {
                    var pic = new PictureBox
                    {
                        Image = preview.Image,
                        Size = new Size(220, 180),
                        BorderStyle = BorderStyle.FixedSingle,
                        WaitOnLoad = false,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Tag = preview
                    };

                    pic.MouseEnter += Pic_MouseHover;
                    pic.MouseLeave += Pic_MouseLeave;
                    
                    
                    flowLayoutPanel1.Controls.Add(pic);
                }
            }
        }

        private void Pic_MouseHover(object? sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic != null)
            {
                Bitmap bitmap = new Bitmap(pic.Width, pic.Height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // ����͸���ɰ�
                    g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Gray)), pic.ClientRectangle);
                    // ����ɾ��Panel
                    _deletePanel.Location = new Point((pic.Width - _deletePanel.Width) / 2, (pic.Height - _deletePanel.Height) / 2);
                    pic.Controls.Add(_deletePanel);
                    // ˢ��PictureBox
                    pic.Image = bitmap;
                    pic.Refresh();
                }
            }
        }

        private void Pic_MouseLeave(object? sender, EventArgs e)
        {
            // ����뿪ʱ�Ƴ��ɰ��ɾ��Panel
            var pic = sender as PictureBox;
            if (pic != null)
            {
                pic.Controls.Remove(_deletePanel);
                pic.Image = (pic.Tag as WallpaperPreview)?.Image;
                pic.Refresh();
            }
        }
    }
}