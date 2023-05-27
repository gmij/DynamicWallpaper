using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicWallpaper.Impl
{

    public class ResourceExistsEventArgs: CustomEventArgs
    {
        public ResourceExistsEventArgs() : base()
        {
        }
    }

    public class ResourceDownloadFailEventArgs : CustomEventArgs
    {
        public ResourceDownloadFailEventArgs(int num) : base(num)
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


        private int _randomNum;

        public int Num => _randomNum;

        protected void ResetNum()
        {
            _randomNum = new Random().Next(1, DefaultBox.Num);
        }

        

        public NetworkWallpaperProviderBase(WallpaperSetting setting, ILogger<NetworkWallpaperProviderBase> logger) {
            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            this.setting = setting;
            this.logger = logger;


            EventBus.Register("DownFail");

            EventBus.Register("Box.Ready");
            EventBus.Register("Box.Open");
            EventBus.Register("Box.Load");
            EventBus.Register("Box.Success");


            EventBus.Register("Box.Fail");
            EventBus.Register("Box.Lost");
            EventBus.Register("Box.Exists");
        }

        protected async Task<T> LoadUrl<T>(string url)
        {
            try
            {
                EventBus.Publish("Box.Open", new CustomEventArgs(this));

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseBody);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "加载壁纸清单失败");
                EventBus.Publish("DownFail", new ResourceDownloadFailEventArgs(3));
                EventBus.Publish("Box.Fail", new CustomEventArgs(this));
            }
            return default;
        }

        protected async void SaveToCache(string url, string fileName)
        {
            var cachePath = Path.Combine(CachePath, setting.Today);
            var filePath = Path.Combine(cachePath, fileName);
            //  当天已经下载过的资源，不重复下载，退出
            if (File.Exists(filePath))
            {
                logger.LogInformation("资源已存在，不重复下载: {0}", filePath);
                EventBus.Publish("WallPaperChanged", new ResourceExistsEventArgs());
                EventBus.Publish("Box.Exists", new CustomEventArgs(this));
                return;
            }

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            try
            {
                EventBus.Publish("Box.Load", new CustomEventArgs(this));
                var fileBytes = await client.GetByteArrayAsync(url);
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                await stream.WriteAsync(fileBytes);
                EventBus.Publish("Box.Success", new CustomEventArgs(this));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "下载壁纸到磁盘失败");
                EventBus.Publish("DownFail", new ResourceDownloadFailEventArgs(1));
                EventBus.Publish("Box.Lost", new CustomEventArgs(this));
            }
            
            
        }

        public abstract Task<bool> DownLoadWallPaper();

        
    }
}
