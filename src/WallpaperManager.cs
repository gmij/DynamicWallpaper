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
        //private int _localWallpaperCount;

        //private FileSystemWatcher _watcher;

        public WallpaperManager(ILogger<WallpaperManager> logger, IWallPaperPool wallPaperPool, IEnumerable<INetworkPaperProvider> paperProviders, DesktopWallpaper wallpaper, WallpaperSetting setting)
        {
            this._logger = logger;
            _wallPaperPool = wallPaperPool;



            this.paperProviders = paperProviders;
            _desktopWallpaper = wallpaper;
            _refreshTime = setting.RefreshTime;

            _monitors = new Dictionary<string, string>();

            GetMonitors();

            
        }


        public void DeleteWallpaper(string path)
        {
            _wallPaperPool.Delete(path);
        }

        //public event EventHandler<WallpaperChangedEventArgs> WallpaperChanged;
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

        internal async void GetInternetWallpaper()
        {
            await Task.WhenAll(paperProviders.Select(provider => provider.DownLoadWallPaper()));
        }


        private void GetMonitors()
        {
            // 创建 WMI 查询语句，查询 Win32_PnPEntity 类型的设备信息
            string query = "SELECT __RELPATH, Description FROM Win32_PnPEntity WHERE (PNPClass = 'Monitor')";

            // 创建 ManagementObjectSearcher 对象，传入查询语句
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            var monitorIds = _desktopWallpaper.GetAllMonitorIDs();
            if (monitorIds.Length > 1)
            {
                _monitors.Add(ResourcesHelper.GetString("Ops.AllScreen"), "all");
            }
            
            // 遍历查询结果，获取每一个设备的实例路径和描述信息
            foreach (ManagementObject obj in searcher.Get())
            {
                // 获取设备实例路径和描述信息
                string instancePath = obj["__RELPATH"]?.ToString();
                string deviceDescription = obj["Description"]?.ToString();
                if (!string.IsNullOrEmpty(deviceDescription) && !string.IsNullOrEmpty(instancePath))
                {
                    //  把WMIC取得的设备实例路径进行数据处理，并和IDW上的数据进行匹配
                    instancePath = instancePath.Replace("Win32_PnPEntity.DeviceID=", "");
                    instancePath = instancePath.Trim('"');
                    instancePath = instancePath.Replace("\\\\", "#");
                    var m = monitorIds.FirstOrDefault(m => m.ToUpper().Contains(instancePath));
                    if (m != null)
                    {
                        // 对可以存在的显示器名称重复进行处理，以下代码由Cursor生成
                        //  主要出现的场景，显示器未安装驱动，在OS中多显示器均显示为通用显示器之类的，引发的重名
                        var i = 0;
                        string newKey = deviceDescription;
                        while (!_monitors.TryAdd(newKey, m) && i < 10)
                        {
                            newKey = $"{deviceDescription} ({++i})";
                        }
                    }
                }
            }
            if (_monitors.Count == 2)
            {
                //  实际开机显示器只有1台时，移除所有屏幕的选项
                _monitors.Remove(ResourcesHelper.GetString("Ops.AllScreen"));
            }
        }


        /// <summary>
        /// 给所有显示器更换随机壁纸
        /// </summary>
        private void ChangeWallpaper()
        {
            if (_wallPaperPool.IsEmpty)
            {
                _logger.LogDebug("壁纸池为空");
                var handler = WallpaperPoolEmpty;
                if (handler != null)
                {
                    handler.Invoke(null, new EventArgs());
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

        /// <summary>
        /// 给指定的显示器更换壁纸
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="monitorId"></param>
        public void ChangeWallpaper(string filePath, string monitorId)
        {
            _desktopWallpaper.SetWallpaper(monitorId, filePath);
            _logger.LogDebug("{0}已更换壁纸{1}", monitorId, filePath);
        }

        /// <summary>
        /// 给所有显示器更换同一壁纸
        /// </summary>
        /// <param name="filePath"></param>
        public void ChangeWallpaper(string filePath)
        {
            var monitorIds = _desktopWallpaper.GetAllMonitorIDs();
            _logger.LogDebug("当前共{0}显示器", monitorIds.Length);
            foreach (var monitorId in monitorIds)
            {
                _desktopWallpaper.SetWallpaper(monitorId, filePath);
                _logger.LogDebug("{0}已更换壁纸{1}", monitorId, filePath);
            }
        }
    }
}
