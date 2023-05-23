using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicWallpaper.Impl
{
    internal class WallhavenWallpaperPool : NetworkWallpaperProviderBase
    {
        private readonly string apiKey = "V6aP1MzDUsF9kl7kdGIck9Qx5zPIuzry";
        private readonly string baseUri = "https://wallhaven.cc/api/v1/search";
        private IronBox box;

        public WallhavenWallpaperPool(WallpaperSetting setting, IronBox box, ILogger<NetworkWallpaperProviderBase> logger) : base(setting, logger)
        {
            this.box = box;
        }

        public override string ProviderName => "Wallhaven";

        public override IBox DefaultBox => box;

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<bool> DownLoadWallPaper()
        {
            string uri = BuildUri();

            var results = await LoadUrl<WallhavenResponse>(uri);

            if (results != null && results.Data != null && results.Data.Count > 0)
            {
                var topImage = results.Data.Take(this.RandomNumer);
                topImage.AsParallel().ForAll(x => SaveToCache(x.Path, x.Id));
                return true;
            }
            else
            {
                throw new Exception("No images found.");
            }


        }

        private string BuildUri()
        {
            UriBuilder builder = new UriBuilder(baseUri);
            builder.Query = $"sorting=random&atleast=1920x1080&apikey={apiKey}&page=1&per_page={box.Num}";

            return builder.Uri.ToString();
        }


        private class WallhavenResponse
        {
            [JsonProperty("data")]
            public List<WallhavenImage>? Data { get; set; }
        }

        private class WallhavenImage
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("path")]
            public string Path { get; set; }
        }
    }
}