namespace DynamicWallpaper
{
    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            DoubleBuffered = true;
        }
    }

    public class DoubleBufferFlowPanel : FlowLayoutPanel
    {
        public DoubleBufferFlowPanel()
        {
            DoubleBuffered = true;
        }
    }

    public class DoubleBufferPictureBox : PictureBox
    {
        public DoubleBufferPictureBox()
        {
            DoubleBuffered = true;
        }
    }
}
