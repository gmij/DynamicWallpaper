using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicWallpaper.Impl
{
    internal class BingDailyWallpaper : NetworkWallpaperProviderBase
    {
        public BingDailyWallpaper(WallpaperSetting setting, WoodenBox box, ILogger<NetworkWallpaperProviderBase> logger) : base(setting, logger)
        {
            this.box = box;
        }

        public override string ProviderName => "必应每日一图";

        private WoodenBox box;

        public override IBox DefaultBox => box;

        public override async Task<bool> DownLoadWallPaper()
        {
            // 从必应获取图片
            string url = $"https://global.bing.com/HPImageArchive.aspx?format=js&idx=0&n={box.Num}&pid=hp&FORM=BEHPTB&uhd=1&uhdwidth=3840&uhdheight=2160&setmkt=zh-CN&setlang=en";

            var json = await client.GetStringAsync(url);

            // 将 json 数据转换成类模型
            var bing = JsonConvert.DeserializeObject<Bing>(json);

            bing?.images?.AsParallel().ForAll(img => SaveToCache($"https://cn.bing.com{img.url}", img.title));
            
            return true;
        }

        private class Bing
        {
            public List<BingImage>? images;
        }

        // 必应的 json 数据转换成类模型
        private class BingImage
        {
            public string? fullstartdate { get; set; }
            public string? url { get; set; }
            public string? urlbase { get; set; }
            public string? title { get; set; }
        }
    }
}
