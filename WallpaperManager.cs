using IDesktopWallpaperWrapper;
using Microsoft.Extensions.Logging;
using System.Management;

namespace DynamicWallpaper
{
    internal class WallpaperManager
    {

        private readonly DesktopWallpaper _desktopWallpaper;
        private readonly IWallPaperPool _wallPaperPool;
        private readonly IEnumerable<INetworkPaperProvider> paperProviders;
        protected readonly ILogger _logger;
        protected readonly int _refreshTime;

        private IDictionary<string, string> _monitors;

        public WallpaperManager(ILogger<WallpaperManager> logger, IWallPaperPool wallPaperPool, IEnumerable<INetworkPaperProvider> paperProviders, DesktopWallpaper wallpaper, WallpaperSetting setting)
        {
            this._logger = logger;
            _wallPaperPool = wallPaperPool;
            this.paperProviders = paperProviders;
            _desktopWallpaper = wallpaper;
            _refreshTime = setting.RefreshTime;
            GetMonitors();
        }

        public event EventHandler? WallpaperChanged;
        public event EventHandler? WallpaperPoolEmpty;


        internal IList<WallpaperPreview> GetWallpaperPreviews()
        {
            return _wallPaperPool.GetWallpaperPreviews();
        }

        internal void Start()
        {
            // 创建一个线程，每隔1小时调用一次更换壁纸，以下代码为Coplit生成
            Task.Run(() =>
            {
                while (true)
                {
                    _logger.LogInformation("定时更换壁纸");
                    // 更换壁纸
                    ChangeWallpaper();
                    _logger.LogInformation("================================");

                    // 每隔1小时调用一次更换壁纸
                    Task.Delay(1000 * 60 * _refreshTime).Wait();
                }
            });
        }

        internal IDictionary<string, INetworkPaperProvider> Providers => paperProviders.ToDictionary(x => x.ProviderName);

        internal IDictionary<string, string> Monitors => _monitors;

        internal void Refresh()
        {
            Task.Run(() => ChangeWallpaper());
        }

        internal void GetInternetWallpaper()
        {
            Task.Run(async () => {
                foreach (var provider in paperProviders)
                {
                    var result = await provider.DownLoadWallPaper(3);
                    if (result)
                    {
                        WallpaperChanged?.Invoke(null, new EventArgs());
                    }
                }
            });
        }


        private void GetMonitors()
        {
            _monitors = new Dictionary<string, string>
            {
                { "所有屏幕", "all" }
            };

            // 创建 WMI 查询语句，查询 Win32_PnPEntity 类型的设备信息
            string query = "SELECT __RELPATH, Description FROM Win32_PnPEntity WHERE (PNPClass = 'Monitor')";

            // 创建 ManagementObjectSearcher 对象，传入查询语句
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            // 遍历查询结果，获取每一个设备的实例路径和描述信息
            foreach (ManagementObject obj in searcher.Get())
            {
                // 获取设备实例路径和描述信息
                string instancePath = obj["__RELPATH"].ToString();
                string deviceDescription = obj["Description"].ToString();
                if (!string.IsNullOrEmpty(deviceDescription) &&  !string.IsNullOrEmpty(instancePath))
                    _monitors.Add(deviceDescription, instancePath);
            }
        }

        private void ChangeWallpaper()
        {
            if (_wallPaperPool.IsEmpty)
            {
                _logger.LogDebug("壁纸池为空");
                if (WallpaperPoolEmpty != null)
                {
                    WallpaperPoolEmpty.Invoke(null, new EventArgs());
                }
                return;
            }
            var monitorIds = _desktopWallpaper.GetAllMonitorIDs();
            _logger.LogDebug("当前共{0}显示器", monitorIds.Length);
            foreach (var monitorId in monitorIds)
            {
                var currPaper = _desktopWallpaper.GetWallpaper(monitorId);
                var newPaper = _wallPaperPool.Renew(currPaper);
                _desktopWallpaper.SetWallpaper(monitorId, newPaper);
                _logger.LogDebug("{0}已更换壁纸{1}", monitorId, newPaper);
            }
        }
    }
}
