using Microsoft.Extensions.Logging;

namespace DynamicWallpaper.Impl
{
    internal class BingDailyWallpaper : NetworkWallpaperProviderBase
    {
        public BingDailyWallpaper(WallpaperSetting setting, WoodenBox box, ILogger<NetworkWallpaperProviderBase> logger) : base(setting, logger)
        {
            this.box = box;
        }

        public override string ProviderName => "Bing";

        private WoodenBox box;

        public override IBox DefaultBox => box;

        public override async Task<bool> DownLoadWallPaper()
        {
            
            // 从必应获取图片
            string url = $"https://global.bing.com/HPImageArchive.aspx?format=js&idx=0&n={this.RandomNumer}&pid=hp&FORM=BEHPTB&uhd=1&uhdwidth=3840&uhdheight=2160&setmkt=zh-CN&setlang=en";

            var bing = await LoadUrl<Bing>(url);

            if (bing == null || bing.images == null || !bing.images.Any())
            {
                throw new Exception("No images found.");
            }


            bing.images.AsParallel().ForAll(img => SaveToCache($"https://cn.bing.com{img.url}", img.title));
            
            return true;
        }

        private class Bing
        {
            public List<BingImage>? images;
        }

        // 必应的 json 数据转换成类模型
        private class BingImage
        {
            public string fullstartdate { get; set; } = string.Empty;
            public string url { get; set; } = string.Empty;
            public string urlbase { get; set; } = string.Empty;
            public string title { get; set; } = string.Empty;
        }
    }
}
