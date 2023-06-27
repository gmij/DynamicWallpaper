namespace DynamicWallpaper.TreasureChest.Impl
{
    internal abstract class DefaultBoxOptions : IBoxOptions
    {
        public virtual bool AutoOpen { get; set; } = false;
        public virtual TimeSpan ResetTime { get; set; } = TimeSpan.FromMinutes(30);
        public virtual int MaxCount { get; set; } = 1;
        public virtual int RandomHarvest { get; set; } = 5;

        private BoxStatus _status = BoxStatus.Close;
        public virtual BoxStatus Status { get => _status; set => _status = value; }

        public virtual BoxAppearance Appearance { get; } = BoxAppearance.normal;

        public abstract string SourceRemark { get; }

        public DateTime? LastOpenTime { get; set; }

        public abstract string ListUrl { get; }

        public virtual string ItemBaseUrl { get; } = string.Empty;

        public virtual string ApiKey { get; } = string.Empty;

        private BoxStyle? _style;
        public BoxStyle Style => _style ??= new BoxStyle(Appearance);

        public ITreasureChest? TreasureChest { get; set; } 
    }


    internal class BingBoxOptions : DefaultBoxOptions
    {

        public override TimeSpan ResetTime { get; set; } = TimeSpan.FromDays(1);
        public override int MaxCount { get; set; } = 1;
        public override int RandomHarvest { get; set; } = 1;

        public override string SourceRemark { get => "images from https://www.Bing.com"; }

        public override string ListUrl => $"https://global.bing.com/HPImageArchive.aspx?format=js&idx=0&n={RandomHarvest}&pid=hp&FORM=BEHPTB&uhd=1&uhdwidth=3840&uhdheight=2160&setmkt=zh-CN&setlang=en";

        public override string ItemBaseUrl => "https://cn.bing.com";
    }

    class PixabayBoxOptions : DefaultBoxOptions
    {
        public override TimeSpan ResetTime { get; set; } = TimeSpan.FromHours(4);
        public override int MaxCount { get; set; } = 3;
        public override int RandomHarvest { get; set; } = 5;

        public override string SourceRemark { get => "images from https://pixabay.com/"; }

        public override string ListUrl => $"https://pixabay.com/api/?key={ApiKey}&image_type=photo&per_page=&order=latest&lang=zh";

        public override string ApiKey => "35011350-04a87bff3b45e5d929d805228";

        public override BoxAppearance Appearance => BoxAppearance.yellow;

    }

    class UnsplashBoxOptions : DefaultBoxOptions
    {
        public override TimeSpan ResetTime { get; set; } = TimeSpan.FromHours(4);
        public override int MaxCount { get; set; } = 3;
        public override int RandomHarvest { get; set; } = 5;

        public override string SourceRemark { get => "images from https://unsplash.com/"; }

        public override string ListUrl => throw new NotImplementedException();
    }

    class WallhavenBoxOptions : DefaultBoxOptions
    {
        public override TimeSpan ResetTime { get; set; } = TimeSpan.FromHours(4);

        public override int MaxCount { get; set; } = 3;
        public override int RandomHarvest { get; set; } = 5;

        public override string SourceRemark { get => "images from https://wallhaven.cc/"; }

        public override string ListUrl => $"https://wallhaven.cc/api/v1/search?sorting=random&atleast=1920x1080&apikey={ApiKey}&page=1&per_page={RandomHarvest}";

        public override string ApiKey => "V6aP1MzDUsF9kl7kdGIck9Qx5zPIuzry";

        public override BoxAppearance Appearance => BoxAppearance.blue;
    }

    class DefaultOptions : DefaultBoxOptions
    {
        public override TimeSpan ResetTime { get; set; } = TimeSpan.FromMinutes(3);

        public override int MaxCount { get; set; } = 3;
        public override int RandomHarvest { get; set; } = 5;

        public override string SourceRemark { get => "images from Gmij.win"; }

        public override string ListUrl => $"https://dw.gmij.win/preview/index.json";

        public override string ItemBaseUrl => "https://dw.gmij.win/";

        public override bool AutoOpen { get; set; } = true;


    }
}
