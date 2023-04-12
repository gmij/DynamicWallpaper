using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynamicWallpaper.Impl
{
    internal class PixabayWallpaperPool : NetworkWallpaperProviderBase
    {

        public class PixabayResponse
        {
            [JsonProperty("hits")]
            public List<PixabayImage>? Hits { get; set; }
        }

        public class PixabayImage
        {
            public string Id { get; set; }
            public string? PreviewUrl { get; set; }
            public string? LargeImageURL { get; set; }
        }

        public PixabayWallpaperPool(WallpaperSetting setting):base(setting)
        {
        }

        public bool IsEmpty { get; private set; } = true;

        public override string ProviderName => "Pixabay";

        /// <summary>
        /// 获取图片，以下代码由Coplit 优化
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<bool> DownLoadWallPaper(int num)
        {
            string apiKey = "35011350-04a87bff3b45e5d929d805228";
            string searchQuery = "nature";
            string uri = $"https://pixabay.com/api/?key={apiKey}&q={searchQuery}&image_type=photo&per_page={num}";

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var images = JsonConvert.DeserializeObject<PixabayResponse>(responseBody);
                if (images == null || images.Hits == null || !images.Hits.Any())
                {
                    throw new Exception("No images found.");
                }

                images.Hits.ForEach(x => SaveToCache(x.LargeImageURL, x.Id));
                
                
                if (images.Hits.Count > 0)
                {
                    IsEmpty = false;
                }
                return true;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                throw new Exception($"Request failed with status code {response.StatusCode}");
            }

        }
    }
}
