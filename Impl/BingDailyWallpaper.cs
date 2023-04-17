using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaper.Impl
{
    internal class BingDailyWallpaper : NetworkWallpaperProviderBase
    {
        public BingDailyWallpaper(WallpaperSetting setting) : base(setting)
        {
        }

        public override string ProviderName => "必应每日一图";

        public override async Task<bool> DownLoadWallPaper(int num)
        {
            // 从必应获取图片
            string url = $"https://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n={num}";

            var json = await client.GetStringAsync(url);

            // 将 json 数据转换成类模型
            var bing = JsonConvert.DeserializeObject<Bing>(json);

            bing?.images.ForEach(img => SaveToCache($"https://cn.bing.com{img.url}", img.title));

            return true;
        }

        private class Bing
        {
            public List<BingImage> images;
        }

        // 必应的 json 数据转换成类模型
        private class BingImage
        {
            public string fullstartdate { get; set; }
            public string url { get; set; }
            public string urlbase { get; set; }
            public string title { get; set; }
        }
    }
}
