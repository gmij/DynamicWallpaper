using IDesktopWallpaperWrapper;
using Microsoft.Extensions.Logging;

namespace DynamicWallpaper
{
    internal class WallpaperManager
    {

        private readonly DesktopWallpaper _desktopWallpaper;
        private readonly IWallPaperPool _wallPaperPool;
        protected readonly ILogger _logger;
        protected readonly int _refreshTime;

        public WallpaperManager(ILogger<WallpaperManager> logger, IWallPaperPool wallPaperPool, DesktopWallpaper wallpaper, WallpaperSetting setting)
        {
            this._logger = logger;
            _wallPaperPool = wallPaperPool;
            _desktopWallpaper = wallpaper;
            _refreshTime = setting.RefreshTime;
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

        internal void Refresh()
        {
            Task.Run(() => ChangeWallpaper());
        }

        private void ChangeWallpaper()
        {
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
