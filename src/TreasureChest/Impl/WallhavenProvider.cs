using Newtonsoft.Json;

namespace DynamicWallpaper.TreasureChest.Impl
{
    internal class WallhavenProvider : NetworkProviderBase
    {

        private class WallhavenResponse: IProviderData<WallhavenImage>
        {
            [JsonProperty("data")]
            public IList<WallhavenImage> Images { get; set; }
        }

        private class WallhavenImage: IProviderDataItem
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("path")]
            public string Url { get; set; }
        }

        public override string ProviderName => "Wallhaven";


        public override async Task<bool> DownLoadWallPaper(IBoxOptions opt)
        {
            return await DownLoadWallPaper<WallhavenResponse, WallhavenImage>(opt);
        }
    }
}
