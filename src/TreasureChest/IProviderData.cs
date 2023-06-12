namespace DynamicWallpaper.TreasureChest
{

    public interface IProviderData<T> where T : class
    {
        
        public IList<T> Images { get; }
    }

    public interface IProviderDataItem
    {
        public string Id { get; }

        public string Url { get; }
    }



}
