using Newtonsoft.Json;

namespace DynamicWallpaper.TreasureChest.Impl
{
    internal class WallhavenProvider : NetworkProviderBase
    {

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

        public override string ProviderName => "Wallhaven";


        protected override async Task<bool> DownLoadWallPaper(IBoxOptions opt, int num)
        {
            var results = await LoadUrl<WallhavenResponse>(opt.ListUrl);

            if (results != null && results.Data != null && results.Data.Count > 0)
            {
                var topImage = results.Data.Take(num);
                //foreach (var item in topImage)
                //{
                //    await SaveToCache(item.Path, item.Id);
                //}
                await Task.Run(() => Parallel.ForEach(topImage, async x => await SaveToCache(x.Path, x.Id)));
                //topImage.AsParallel().ForAll(x => SaveToCache(x.Path, x.Id));
                return true;
            }
            else
            {
                throw new Exception("No images found.");
            }
        }
    }
}
