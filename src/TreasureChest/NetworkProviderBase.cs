
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicWallpaper.TreasureChest
{

    class ResourceExistsEventArgs : CustomEventArgs
    {

    }

    class ResourceDownloadFailEventArgs : CustomEventArgs
    {

        public ResourceDownloadFailEventArgs(int num) : base(num)
        {

        }
    }


    internal abstract class NetworkProviderBase : INetworkProvider
    {
        private Lazy<WallpaperSetting> setting = new(() => ServiceLocator.GetService<WallpaperSetting>(), LazyThreadSafetyMode.ExecutionAndPublication);
        private Lazy<ILogger<NetworkProviderBase>> logger = new(() => ServiceLocator.GetService<ILogger<NetworkProviderBase>>(), LazyThreadSafetyMode.ExecutionAndPublication);
        protected HttpClient client;

        private string? _cachePath;

        protected string CachePath => _cachePath ??= Path.Combine(Setting.CachePath, "Internet");

        protected WallpaperSetting Setting => setting.Value;
        protected ILogger<NetworkProviderBase> Logger => logger.Value;


        public abstract string ProviderName { get; }

        public NetworkProviderBase()
        {
            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            EventBus.Register("DownFail");
            EventBus.Register("Box.Ready");
            EventBus.Register("Box.Open");
            EventBus.Register("Box.Load");
            EventBus.Register("Box.Success");
            EventBus.Register("Box.Fail");
            EventBus.Register("Box.Lost");
            EventBus.Register("Box.Exists");
            EventBus.Register("Box.Random");
            EventBus.Register("Box.Finish");
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
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载壁纸清单失败");
                EventBus.Publish("DownFail", new ResourceDownloadFailEventArgs(3));
                EventBus.Publish("Box.Fail", new CustomEventArgs(this));
            }
            return default;
        }

        protected async Task SaveToCache(string url, string fileName)
        {
            var cachePath = Path.Combine(CachePath, Setting.Today);
            var filePath = Path.Combine(cachePath, fileName);
            //  当天已经下载过的资源，不重复下载，退出
            if (File.Exists(filePath))
            {
                Logger.LogInformation("资源已存在，不重复下载: {0}", filePath);
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
                Logger.LogError(ex, "下载壁纸到磁盘失败");
                EventBus.Publish("DownFail", new ResourceDownloadFailEventArgs(1));
                EventBus.Publish("Box.Lost", new CustomEventArgs(this));
            }


        }

        public async Task<bool> DownLoadWallPaper(IBoxOptions opt)
        {
            var num = new Random().Next(0, opt.RandomHarvest);
            EventBus.Publish("Box.Random", new CustomEventArgs(num));
            var r = await DownLoadWallPaper(opt, num);
            Task.WaitAll();
            EventBus.Publish("Box.Finish", new CustomEventArgs(this));
            return r;
            
        }

        protected abstract Task<bool> DownLoadWallPaper(IBoxOptions opt, int num);


    }
}
