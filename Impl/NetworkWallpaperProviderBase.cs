using Microsoft.Extensions.Logging;

namespace DynamicWallpaper.Impl
{

    public class ResourceExistsEventArgs: CustomEventArgs
    {
        public ResourceExistsEventArgs() : base()
        {
        }
    }

    internal abstract class NetworkWallpaperProviderBase : INetworkPaperProvider
    {
        protected readonly WallpaperSetting setting;
        protected readonly ILogger<NetworkWallpaperProviderBase> logger;
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
                //  todo: 要通知外部，把多的等待框去掉.
                EventBus.Publish("WallPaperChanged", new ResourceExistsEventArgs());
                return;
            }

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            try
            {
                var fileBytes = await client.GetByteArrayAsync(url);
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                await stream.WriteAsync(fileBytes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "下载壁纸到磁盘失败");
            }
            
            
        }

        public abstract Task<bool> DownLoadWallPaper();

        
    }
}
