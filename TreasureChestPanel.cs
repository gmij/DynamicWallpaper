namespace DynamicWallpaper
{
    using System;
    using System.Threading;

    public class TreasureChestPanel: DoubleBufferPanel
    {
        private readonly IBox box;
        private readonly INetworkPaperProvider provider;
        private PictureBox? pic;
        private Label? label;
        private Timer? timer;
        private Timer? labelTimer;
        private TimeSpan labelTime;

        public TreasureChestPanel(INetworkPaperProvider provider)
        {

            this.box = provider.DefaultBox;
            this.provider = provider;
            InitializeComponent();

            if (pic != null)
                pic.MouseDoubleClick += Box_MouseDoubleClick;

            timer = new Timer(ResetBox, null, Timeout.Infinite, Timeout.Infinite);
            labelTimer = new Timer(LabelTimerStart, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void LabelTimerStart(object? state)
        {
            labelTime = labelTime.Subtract(TimeSpan.FromSeconds(1));
            label.Text = labelTime.ToString();
        }

        private void ResetBox(object? state)
        {
            if (pic != null)
            {
                pic.MouseDoubleClick += Box_MouseDoubleClick;
                pic.Image = box.CloseBox;
            }
            if (label != null) 
                label.Text = "双击打开";
            
            labelTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        void InitializeComponent()
        {
            this.Size = new Size(180, 180);
            this.BorderStyle = BorderStyle.FixedSingle;

            //  添加一个图像框
            pic = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = box.CloseBox
            };
            this.Controls.Add(pic);

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Size = new Size(250, 30),
                BackColor = Color.FromArgb(120, Color.Gray)
            };
            label = new Label
            {
                Dock = DockStyle.Right,
                Text = "双击打开",
                BackColor = Color.Transparent,
                AutoSize = true,
            };
            bottomPanel.Controls.Add(label);
            this.Controls.Add(bottomPanel);
        }

        private void Box_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            provider.DownLoadWallPaper();
            if (pic != null)
            {
                pic.Image = box.OpenBox;
                pic.MouseDoubleClick -= Box_MouseDoubleClick;
            }
            if (label != null)
            {
                label.Text = "wait~~";
                timer?.Change(box.ResetTime.Seconds*1000, Timeout.Infinite);
                labelTime = box.ResetTime;
                labelTimer?.Change(1000, 1000);
            }
        }
    }

}
