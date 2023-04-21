using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaper.Impl
{
    internal abstract class NetworkWallpaperProviderBase : INetworkPaperProvider
    {
        protected readonly WallpaperSetting setting;
        protected HttpClient client;
        protected string CachePath => Path.Combine(setting.CachePath, "Internet");

        public abstract string ProviderName { get; }
        public abstract IBox DefaultBox { get; }

        public NetworkWallpaperProviderBase(WallpaperSetting setting) {
            client = new HttpClient();
            this.setting = setting;
        }

        protected async void SaveToCache(string url, string fileName)
        {
            var cachePath = Path.Combine(CachePath, setting.Today);
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var fileBytes = await client.GetByteArrayAsync(url);
            var filePath = Path.Combine(cachePath, fileName);
            if (File.Exists(filePath))
            {
                filePath += Guid.NewGuid().ToString("D");
            }
            File.WriteAllBytes(filePath, fileBytes);
        }

        public abstract Task<bool> DownLoadWallPaper(int num);

        
    }
}
