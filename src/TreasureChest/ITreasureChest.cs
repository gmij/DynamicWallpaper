namespace DynamicWallpaper.TreasureChest
{
    /// <summary>
    /// 宝箱
    /// </summary>
    internal interface ITreasureChest
    {

        IBoxOptions Box { get; }

        INetworkProvider Provider { get; }

        Control Panel { get; }

    }
}
