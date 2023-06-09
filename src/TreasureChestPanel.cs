﻿namespace DynamicWallpaper
{
    using System;
    using System.Threading;
    using DynamicWallpaper.Tools;
    using DynamicWallpaper.TreasureChest;

    /// <summary>
    /// 宝箱面板
    /// </summary>
    public class TreasureChestPanel: DoubleBufferPanel
    {
        private IBoxOptions? boxOptions;

        private IBoxOptions BoxOpt => boxOptions ?? throw new NullReferenceException();

        private PictureBox? pic;
        private readonly Label label;
        private readonly Timer timer;
        private readonly Timer labelTimer;
        private TimeSpan labelTime;


        internal EventHandler<IBoxOptions>? OpenHandler;

        public TreasureChestPanel()
        {
            label = new Label();
            timer = new Timer(ResetBox, null, Timeout.Infinite, Timeout.Infinite);
            labelTimer = new Timer(LabelTimerStart, null, Timeout.Infinite, Timeout.Infinite);
        }

        internal void Open()
        {
            this.Box_MouseDoubleClick(null, null);
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
                pic.Image = BoxOpt.Style.CloseBox;
            }
            
            labelTimer?.Change(Timeout.Infinite, Timeout.Infinite);

            if (label != null)
                label.Text = ResourcesHelper.GetString("Open");

            //currentNum = boxOptions.RandomHarvest;
            EventBus.Publish(EventName.BoxReady, new CustomEventArgs(BoxOpt));
        }

        internal void InitializeComponent(IBoxOptions opt)
        {
            this.Size = new Size(180, 180);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.boxOptions = opt;
            //currentNum = opt.RandomHarvest;

            //  添加一个图像框
            pic = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = opt.Style.CloseBox //box.CloseBox
            };

            pic.MouseDoubleClick += Box_MouseDoubleClick;
            this.Controls.Add(pic);

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Size = new Size(250, 30),
                BackColor = Color.FromArgb(120, Color.Gray)
            };
            
            label.Dock = DockStyle.Right;
            label.Text = ResourcesHelper.GetString("Open");
            label.BackColor = Color.Transparent;
            label.AutoSize = true;
            bottomPanel.Controls.Add(label);
            this.Controls.Add(bottomPanel);
        }

        private void Box_MouseDoubleClick(object? sender, MouseEventArgs? e)
        {
            
            BoxOpt.LastOpenTime = DateTime.Now;
            if (pic != null)
            {
                pic.Image = BoxOpt.Style.OpenBox;
                OpenHandler?.Invoke(this, BoxOpt);
                pic.MouseDoubleClick -= Box_MouseDoubleClick;
            }
            if (label != null)
            {
                label.Text = ResourcesHelper.GetString("Wait");
                timer?.Change((int)BoxOpt.ResetTime.TotalSeconds *1000, Timeout.Infinite);
                labelTime = BoxOpt.ResetTime;
                labelTimer?.Change(1000, 1000);
            }
        }
    }

}
