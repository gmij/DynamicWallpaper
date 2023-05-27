namespace DynamicWallpaper.TreasureChest.Impl
{
    internal class BingProvider : NetworkProviderBase
    {

        public override string ProviderName => "Bing";

        protected override async Task<bool> DownLoadWallPaper(IBoxOptions opt, int num)
        {
            var bing = await LoadUrl<Bing>(opt.ListUrl);

            if (bing == null || bing.images == null || !bing.images.Any())
            {
                throw new Exception("No images found.");
            }

            Parallel.ForEach(bing.images, async img => await SaveToCache($"{opt.ItemBaseUrl}{img.url}", img.title));
            //bing.images.AsParallel().ForAll(img => SaveToCache($"{opt.ItemBaseUrl}{img.url}", img.title));
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