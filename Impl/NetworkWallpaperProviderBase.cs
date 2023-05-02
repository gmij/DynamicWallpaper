using Microsoft.Extensions.Logging;

namespace DynamicWallpaper.Impl
{
    internal abstract class NetworkWallpaperProviderBase : INetworkPaperProvider
    {
        protected readonly WallpaperSetting setting;
        private readonly ILogger<NetworkWallpaperProviderBase> logger;
        protected HttpClient client;
        protected string CachePath => Path.Combine(setting.CachePath, "Internet");

        public abstract string ProviderName { get; }
        public abstract IBox DefaultBox { get; }

        public NetworkWallpaperProviderBase(WallpaperSetting setting, ILogger<NetworkWallpaperProviderBase> logger) {
            client = new HttpClient();
            this.setting = setting;
            this.logger = logger;
        }

        protected async void SaveToCache(string url, string fileName)
        {
            var cachePath = Path.Combine(CachePath, setting.Today);
            var filePath = Path.Combine(cachePath, fileName);
            //  当天已经下载过的资源，不重复下载，退出
            if (File.Exists(filePath))
            {
                logger.LogInformation("资源已存在，不重复下载: {0}", filePath);
                return;
            }

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var fileBytes = await client.GetByteArrayAsync(url);
            
            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            await stream.WriteAsync(fileBytes);
        }

        public abstract Task<bool> DownLoadWallPaper();

        
    }
}
