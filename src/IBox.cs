namespace DynamicWallpaper
{
    public interface IBox
    {

        Image CloseBox { get; }

        Image OpenBox { get; }

        TimeSpan ResetTime { get; }

        /// <summary>
        /// 当前有几个箱子可以打开
        /// </summary>
        int Count { get; set; } 

        /// <summary>
        /// 每次开出几张图
        /// </summary>
        int Num { get; } 

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
        public int Count { get; set; } = 0;
        public virtual int Num { get; set; } = 3;
    }


    public class WoodenBox : BaseBox
    {
        
        public WoodenBox(ResourcesHelper rh): base("yellow", rh)
        {
        }

        public override TimeSpan ResetTime => TimeSpan.FromSeconds(5);

        public override int Num => 1;
    }

    public class IronBox : BaseBox
    {

        public IronBox(ResourcesHelper rh): base("blue", rh)
        {
        }

        public override TimeSpan ResetTime => TimeSpan.FromMinutes(10);
    }
}
