using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicWallpaper.Impl
{
    internal class WallhavenWallpaperPool : NetworkWallpaperProviderBase
    {
        private readonly string apiKey = "V6aP1MzDUsF9kl7kdGIck9Qx5zPIuzry";
        private readonly string baseUri = "https://wallhaven.cc/api/v1/search";
        private IronBox box;

        public WallhavenWallpaperPool(WallpaperSetting setting, IronBox box, ILogger<NetworkWallpaperProviderBase> logger):base(setting, logger)
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
            try
            {
                string uri = BuildUri();
                
                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ProcessResponse(responseBody);
                    
                    //if (box.Num > 0)
                    //{
                    //    IsEmpty = false;
                    //}
                    return true;
                }
                else
                {
                    logger.LogError($"Error: {response.StatusCode}");
                    throw new Exception($"Request failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to download wallpaper.");
                return false;
            }

        }
        
        private string BuildUri()
        {
            UriBuilder builder = new UriBuilder(baseUri);
            builder.Query = $"sorting=random&atleast=1920x1080&apikey={apiKey}&page=1&per_page={box.Num}";

            return builder.Uri.ToString();
        }

        private void ProcessResponse(string responseBody)
        {
            var results = JsonConvert.DeserializeObject<WallhavenResponse>(responseBody);
            
            if (results != null && results.Data != null && results.Data.Count > 0)
            {
                var topImage = results.Data.Take(box.Num);
                topImage.AsParallel().ForAll(x => SaveToCache(x.Path, x.Id));
            }
            else
            {
                throw new Exception("No images found.");
            }
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