namespace DynamicWallpaper
{
    public interface IBox
    {

        Image CloseBox { get; }

        Image OpenBox { get; }

        TimeSpan ResetTime { get; }

    }

    public abstract class BaseBox : IBox
    {

        private readonly string closeImgName = "box.png";
        private readonly string openImgName = "open_box.png";
        private readonly ResourcesHelper rh;

        public BaseBox(string color, ResourcesHelper rh)
        {
            if (!string.IsNullOrEmpty(color))
            {
                closeImgName = $"{color}_{closeImgName}";
                openImgName = $"{color}_{openImgName}";
            }

            this.rh = rh;
        }

        public Image CloseBox => rh.GetImage(closeImgName);

        public Image OpenBox => rh.GetImage(openImgName);

        public abstract TimeSpan ResetTime { get; }
    }


    public class WoodenBox : BaseBox
    {
        

        public WoodenBox(ResourcesHelper rh): base("yellow", rh)
        {
        }

        public override TimeSpan ResetTime => TimeSpan.FromSeconds(5);
    }

    public class IronBox : BaseBox
    {

        public IronBox(ResourcesHelper rh): base("blue", rh)
        {
        }

        public override TimeSpan ResetTime => TimeSpan.FromSeconds(50);
    }
}
