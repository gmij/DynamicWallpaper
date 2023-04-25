using Microsoft.Extensions.Logging;
using Timer = System.Threading.Timer;

namespace DynamicWallpaper.Impl
{
    internal class LocalWallpaperPool : IWallPaperPool
    {
        private readonly string _cachePath;
        private string[] _wallPaperList;
        private readonly ILogger<LocalWallpaperPool> _logger;
        private FileSystemWatcher _watcher;

        private static object locker = new object();
        private static DateTime _lastUpdateTime = DateTime.Now;

        private static int _refreshTimeSpan = 2000;

        public bool IsEmpty => _wallPaperList.Length == 0;

        private EventHandler<int> _wallPaperChanged;
        private Timer _refreshTask;

        public EventHandler<int> WallPaperChanged { get => _wallPaperChanged; set => _wallPaperChanged = value; }

        public LocalWallpaperPool(WallpaperSetting setting, ILogger<LocalWallpaperPool> logger)
        {
            _cachePath = setting.CachePath;
            _wallPaperList = new string[0];
            InitPaperPool();
            _logger = logger;
            
            _watcher = new FileSystemWatcher(_cachePath);
            InitWatcher();
        }


        private void InitWatcher()
        {
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size;

            _watcher.Created += FileChanged;
            _watcher.Renamed += FileChanged;
            _watcher.Deleted += FileChanged;

            _watcher.EnableRaisingEvents = true;

            _refreshTask = new Timer(FireFileChangedEvent, null, Timeout.Infinite, Timeout.Infinite);

        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"文件发生变化：{e.FullPath}, {e.ChangeType}");
            ReLoadCacheImage();
            //_logger.LogInformation($"文件发生变化：刷新缓存");

            //  通过定时器滑动，减少事件的激活次数。
            _logger.LogInformation($"文件发生变化：定时器滑动");
            _refreshTask.Change(_refreshTimeSpan, Timeout.Infinite);
        }

        private void FireFileChangedEvent(object? state)
        {
            WallPaperChanged.Invoke(null, _wallPaperList.Length);
            _refreshTask.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation($"文件发生变化事件已抛出：定时器禁用");
        }

        private void InitPaperPool()
        {
            _wallPaperList = Directory.GetFiles(_cachePath, "*", SearchOption.AllDirectories);
            _lastUpdateTime = DateTime.Now;
        }


        private void ReLoadCacheImage()
        {
            if (!string.IsNullOrEmpty(_cachePath))
            {
                if (Directory.Exists(_cachePath))
                {
                    if (DateTime.Now - _lastUpdateTime > TimeSpan.FromSeconds(2))
                    {
                        lock (locker)
                        {
                            if (DateTime.Now - _lastUpdateTime > TimeSpan.FromSeconds(2))
                            {
                                InitPaperPool();
                            }
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(_cachePath);
                }
            }
        }

        public string Renew(string excludePath)
        {
            if (!IsEmpty)
            {
                var list = _wallPaperList.Where(p => p != excludePath).ToArray();
                var poolSize = list.Count();
                var file = list[new Random().Next(poolSize)];
                if (File.Exists(file))
                {
                    return file;
                }
                else
                {
                    _logger.LogWarning("缓存图片失效:{0}", file);
                    ReLoadCacheImage();
                }
            }
            return string.Empty;
        }

        public List<WallpaperPreview> GetWallpaperPreviews()
        {
            if (_wallPaperList.Length == 0)
                ReLoadCacheImage();

            var previews = new List<WallpaperPreview>();
            foreach (var file in _wallPaperList)
            {
                if (File.Exists(file))
                {
                    // 以下代码为Coplit 自动生成，用于解决Image.FromFile的句柄占用问题
                    using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var bitmap = new Bitmap(stream);
                    // 在这里可以对bitmap进行进一步处理
                    var preview = new WallpaperPreview
                    {
                        Path = file,
                        Image = (Image)bitmap.Clone()
                    };
                    previews.Add(preview);
                    // 释放bitmap占用的内存
                    bitmap.Dispose();
                }
            }
            return previews;
        }
    }
}
