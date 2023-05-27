using Newtonsoft.Json;

namespace DynamicWallpaper.TreasureChest.Impl
{
    internal class PixabayProvider : NetworkProviderBase
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
            public string LargeImageURL { get; set; }
        }


        public override string ProviderName => "Pixabay";


        /// <summary>
        /// 获取图片，以下代码由Coplit 优化
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override async Task<bool> DownLoadWallPaper(IBoxOptions opt, int num)
        {
            var images = await LoadUrl<PixabayResponse>(opt.ListUrl);

            if (images == null || images.Hits == null || !images.Hits.Any())
            {
                throw new Exception("No images found.");
            }
            else
            {
                var topImage = images.Hits.Take(num);

                //foreach (var item in topImage)
                //{
                //    await SaveToCache(item.LargeImageURL, item.Id);
                //}
                Parallel.ForEach(topImage, async x => await SaveToCache(x.LargeImageURL, x.Id));
                //topImage.AsParallel().ForAll(x => SaveToCache(x.LargeImageURL, x.Id)) ;
                
                return true;
            }
        }
    }
}
