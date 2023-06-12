using Newtonsoft.Json;

namespace DynamicWallpaper.TreasureChest.Impl
{
    internal class PixabayProvider : NetworkProviderBase
    {

        public class PixabayResponse: IProviderData<PixabayImage>
        {

            [JsonProperty("hits")]
            public IList<PixabayImage> Images { get; set; }
        }

        public class PixabayImage: IProviderDataItem
        {
            public string Id { get; set; }
            public string? PreviewUrl { get; set; }

            [JsonProperty("LargeImageURL")]
            public string Url { get; set; }
        }


        public override string ProviderName => "Pixabay";


        /// <summary>
        /// 获取图片，以下代码由Coplit 优化
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<bool> DownLoadWallPaper(IBoxOptions opt)
        {
            return await DownLoadWallPaper<PixabayResponse, PixabayImage>(opt);
           
        }
    }
}
