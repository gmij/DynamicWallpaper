using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Timer = System.Threading.Timer;

namespace DynamicWallpaper.Impl
{


    class LocalWallpaperPool : IWallPaperPool
    {
        static object locker = new object();
        private IList<WallpaperPreview> previews;
        private readonly ILogger<LocalWallpaperPool> logger;

        private FileSystemWatcher watcher;
        private string cachePath;


        public LocalWallpaperPool(WallpaperSetting setting, ILogger<LocalWallpaperPool> logger)
        {
            cachePath = setting.CachePath;
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            var files = Directory.GetFiles(cachePath, "*", SearchOption.AllDirectories);
            previews = new List<WallpaperPreview>();
            EventBus.Register("WallPaperChanged");
            LoadImages(files);
            this.logger = logger;
            watcher = new FileSystemWatcher(cachePath);
            InitWatcher();
            
        }

        private void InitWatcher()
        {
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size;

            watcher.Created += FileChanged;
            watcher.Renamed += FileChanged;
            watcher.Deleted += FileChanged;

            watcher.EnableRaisingEvents = true;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            WallpaperPreview? item = null;
            lock (locker)
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        item = previews.FirstOrDefault(p => p.Path == e.FullPath);
                        if (item != null)
                        {
                            logger.LogWarning("文件已存在:{0}", e.FullPath);
                        }
                        else
                        {
                            logger.LogInformation("文件已添加:{0}", e.FullPath);
                            item = LoadImage(e.FullPath);
                            previews.Add(item);
                        }
                        break;
                    case WatcherChangeTypes.Deleted:
                        item = previews.FirstOrDefault(p => p.Path == e.FullPath);
                        if (item != null)
                        {
                            logger.LogInformation("文件已删除:{0}", e.FullPath);
                            previews.Remove(item);
                        }
                        else
                        {
                            logger.LogWarning("文件不存在:{0}", e.FullPath);
                        }
                        break;
                    case WatcherChangeTypes.Changed:
                    case WatcherChangeTypes.Renamed:
                        item = previews.FirstOrDefault(p => p.Path == e.FullPath);
                        if (item != null)
                        {
                            logger.LogInformation("文件已修改:{0}", e.FullPath);
                            previews.Remove(item);
                            item = LoadImage(e.FullPath);
                            previews.Add(item);
                        }
                        else
                        {
                            logger.LogWarning("文件不存在:{0}", e.FullPath);
                        }
                        break;
                }
            }

            EventBus.Publish("WallPaperChanged", new CustomEventArgs(new WallpaperChangedEventArgs(item, e.ChangeType)));

            //WallPaperChanged?.Invoke(this, new WallpaperChangedEventArgs(item, e.ChangeType));
        }

        private void LoadImages(string[] files)
        {
            lock (locker)
            {
                previews.Clear();
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        previews.Add(LoadImage(file));
                    }
                }
            }
        }

        private WallpaperPreview LoadImage(string file)
        {
            // 切换一下线程，让之前的写文件句柄得到释放。
            Thread.Sleep(5);
            // 以下代码为Coplit 自动生成，用于解决Image.FromFile的句柄占用问题
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var bitmap = new Bitmap(stream);
            // 在这里可以对bitmap进行进一步处理
            var preview = new WallpaperPreview
            {
                Path = file,
                Image = (Image)bitmap.Clone()
            };
            // 释放bitmap占用的内存
            bitmap.Dispose();
            return preview;
        }

        public bool IsEmpty => !previews.Any();

        //public EventHandler<WallpaperChangedEventArgs> WallPaperChanged { get; set; }

        public IList<WallpaperPreview> GetWallpaperPreviews()
        {
            return previews;
        }

        public string Renew(string excludePath)
        {
            if (!IsEmpty)
            {
                WallpaperPreview[] list = null;
                if (!string.IsNullOrEmpty(excludePath))
                {
                    list = previews.Where(p => p.Path != excludePath).ToArray();
                }
                list ??= previews.ToArray();
                var poolSize = list.Length;
                if (poolSize == 0)
                {
                    return string.Empty;
                }
                var file = list[new Random().Next(poolSize)];
                if (File.Exists(file.Path))
                {
                    return file.Path;
                }
                else
                {
                    logger.LogWarning("缓存图片失效:{0}", file.Path);
                }
            }
            return string.Empty;
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
