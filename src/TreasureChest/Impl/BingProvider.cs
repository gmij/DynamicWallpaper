using Newtonsoft.Json;

namespace DynamicWallpaper.TreasureChest.Impl
{
    internal class BingProvider : NetworkProviderBase
    {

        public override string ProviderName => "Bing";

        public override async Task<bool> DownLoadWallPaper(IBoxOptions opt)
        {
            return await DownLoadWallPaper<Bing, BingImage>(opt);
        }
#pragma warning disable CS8618
        private class Bing : IProviderData<BingImage>
        {
            //public List<BingImage>? images;
            [JsonProperty("images")]
            public IList<BingImage> Images { get; set; }
        }

        // 必应的 json 数据转换成类模型
        private class BingImage: IProviderDataItem
        {

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("title")]
            public string Id { get; set; }
        }
#pragma warning restore CS8618
    }
}