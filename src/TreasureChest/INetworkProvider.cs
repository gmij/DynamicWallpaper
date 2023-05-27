namespace DynamicWallpaper.TreasureChest
{
    internal interface INetworkProvider
    {

        Task<bool> DownLoadWallPaper(IBoxOptions options);

        string ProviderName { get; }

    }
}
