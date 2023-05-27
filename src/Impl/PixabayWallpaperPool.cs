using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicWallpaper.Impl
{
    internal class PixabayWallpaperPool : NetworkWallpaperProviderBase
    {
        private readonly IronBox box;

        public class PixabayResponse
        {
            [JsonProperty("hits")]
            public List<PixabayImage>? Hits { get; set; }
        }

        public class PixabayImage
        {
            public string Id { get; set; }
            public string? PreviewUrl { get; set; }
            public string LargeImageURL { get; set; }
        }

        public PixabayWallpaperPool(WallpaperSetting setting, IronBox box, ILogger<NetworkWallpaperProviderBase> logger):base(setting, logger)
        {
            this.box = box;

            ResetNum();
        }

        public bool IsEmpty { get; private set; } = true;

        public override string ProviderName => "Pixabay";

        public override IBox DefaultBox => box;

        /// <summary>
        /// 获取图片，以下代码由Coplit 优化
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<bool> DownLoadWallPaper()
        {
            string apiKey = "35011350-04a87bff3b45e5d929d805228";
            string uri = $"https://pixabay.com/api/?key={apiKey}&image_type=photo&per_page={DefaultBox.Num}&order=latest&lang=zh";

            var images = await LoadUrl<PixabayResponse>(uri);

            if (images == null || images.Hits == null || !images.Hits.Any())
            {
                throw new Exception("No images found.");
            }
            else
            {
                var topImage = images.Hits.Take(this.Num);

                topImage.AsParallel().ForAll(x => SaveToCache(x.LargeImageURL, x.Id));
                ResetNum();
                return true;
            }
        }
    }
}
