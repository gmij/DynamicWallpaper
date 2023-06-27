using DynamicWallpaper.Tools;

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

    public class BaseBox : IBox
    {

        private readonly string closeImgName = "box.png";
        private readonly string openImgName = "open_box.png";
        private readonly ResourcesHelper rh;

        public BaseBox(string? color, ResourcesHelper rh)
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

        public virtual TimeSpan ResetTime { get; }
        public int Count { get; set; } = 0;
        public virtual int Num { get; set; } = 5;
    }


    public class BoxOption
    {
        public int Count { get; set; }

        public int Num { get; set; }

        public TimeSpan ResetTime { get; set; }

        public bool AutoOpen { get; set; }

        public string? Color { get; set; }
    }


    public class BoxBuilder
    {
        private readonly ResourcesHelper rh;

        public BoxBuilder()
        {
            rh = ResourcesHelper.Instance;
        }

        public BoxBuilder AddProvider(INetworkPaperProvider provider)
        {
            this.Provider = provider;
            return this;
        }

        public BoxBuilder AddOptions(BoxOption opt)
        {
            Opt = opt;
            return this;
        }

        public IBox Build()
        {
            if (Opt == null)
                throw new ArgumentNullException(nameof(Opt));
            if (Provider == null)
                throw new ArgumentNullException(nameof(Provider));
            var k = new BaseBox(Opt.Color, rh);
            
            return k;
        }

        public BoxOption? Opt { get; private set; }
        public INetworkPaperProvider? Provider { get; private set; }
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

        public override TimeSpan ResetTime => TimeSpan.FromMinutes(1);
    }
}
