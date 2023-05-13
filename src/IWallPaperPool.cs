namespace DynamicWallpaper
{
    public interface IWallPaperPool
    {
        /// <summary>
        /// 换一个新壁纸
        /// </summary>
        /// <param name="excludePath">排除当前的壁纸</param>
        /// <returns></returns>
        string Renew(string excludePath = "");

        /// <summary>
        /// 壁纸池是否为空
        /// </summary>
        bool IsEmpty { get; }

        //  获取所有本地图片的预览图
        IList<WallpaperPreview> GetWallpaperPreviews();

        //EventHandler<WallpaperChangedEventArgs> WallPaperChanged { get; set; }

        void Delete(string path);
    }

    public class WallpaperChangedEventArgs
    {
        public WallpaperPreview Data { get;  }

        public string Id { get; }

        public WatcherChangeTypes Mode { get; }

        public WallpaperChangedEventArgs(WallpaperPreview? preview, WatcherChangeTypes mode)
        {
            if (preview == null) 
                throw new ArgumentNullException(nameof(preview));

            this.Data = preview;
            this.Mode = mode;
            this.Id = preview.Id;
        }

    }

    public interface INetworkPaperProvider
    {
        Task<bool> DownLoadWallPaper();

        string ProviderName { get; }

        IBox DefaultBox { get; }
                
    }
}