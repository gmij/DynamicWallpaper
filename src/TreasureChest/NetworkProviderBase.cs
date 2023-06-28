
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
#pragma warning disable CS8603 // 可能返回 null 引用。
        private readonly Lazy<WallpaperSetting> setting = new(() => ServiceLocator.GetService<WallpaperSetting>(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly Lazy<ILogger<NetworkProviderBase>> logger = new(() => ServiceLocator.GetService<ILogger<NetworkProviderBase>>(), LazyThreadSafetyMode.ExecutionAndPublication);
#pragma warning restore CS8603 // 可能返回 null 引用。
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

        }

        private static void RegisterEvent()
        {
            EventBus.Register(EventName.DownFail);
            EventBus.Register(EventName.BoxReady);
            EventBus.Register(EventName.BoxOpen);
            EventBus.Register(EventName.BoxLoad);
            EventBus.Register(EventName.BoxSuccess);
            EventBus.Register(EventName.BoxFail);
            EventBus.Register(EventName.BoxLost);
            EventBus.Register(EventName.BoxExists);
            EventBus.Register(EventName.BoxRandom);
            EventBus.Register(EventName.BoxFinish);
        }

        protected async Task<T?> LoadUrl<T>(string url)
        {
            try
            {
                EventBus.Publish(EventName.BoxOpen, new CustomEventArgs(this));

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(responseBody);
                    if (result == null)
                    {
                        Logger.LogWarning("加载壁纸清单时，返回的json反序列化出来是空的");
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "加载壁纸清单失败");
                EventBus.Publish(EventName.DownFail, new ResourceDownloadFailEventArgs(3));
                EventBus.Publish(EventName.BoxFail, new CustomEventArgs(this));
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
                EventBus.Publish(EventName.WallPaperChanged, new ResourceExistsEventArgs());
                EventBus.Publish(EventName.BoxExists, new CustomEventArgs(this));
                return;
            }

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            try
            {
                EventBus.Publish(EventName.BoxLoad, new CustomEventArgs(this));
                var fileBytes = await client.GetByteArrayAsync(url);
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                await stream.WriteAsync(fileBytes);
                EventBus.Publish(EventName.BoxSuccess, new CustomEventArgs(this));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "下载壁纸到磁盘失败");
                EventBus.Publish(EventName.DownFail, new ResourceDownloadFailEventArgs(1));
                EventBus.Publish(EventName.BoxLost, new CustomEventArgs(this));
            }

        }

        protected virtual async Task<bool> DownLoadWallPaper<T, U>(IBoxOptions opt) where T : IProviderData<U> where U: class, IProviderDataItem
        {
            var num = new Random().Next(1, opt.RandomHarvest);
            EventBus.Publish(EventName.BoxRandom, new CustomEventArgs(num));
            //var r = await DownLoadWallPaper(opt, num);
            var images = await LoadUrl<T>(opt.ListUrl);

            if (images == null || images.Images == null || !images.Images.Any())
            {
                throw new Exception("No images found.");
            }
            else
            {
                var topImage = images.Images.Take(num);

                Parallel.ForEach(topImage, async x => await SaveToCache(x.Url, x.Id));

                Task.WaitAll();
                EventBus.Publish(EventName.BoxFinish, new CustomEventArgs(this));
                return true;
            }
        }

        public abstract Task<bool> DownLoadWallPaper(IBoxOptions opt);


    }
}
