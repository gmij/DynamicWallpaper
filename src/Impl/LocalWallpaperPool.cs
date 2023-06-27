using Microsoft.Extensions.Logging;

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
            LoadImages(files);
            this.logger = logger;
            watcher = new FileSystemWatcher(cachePath);
            InitWatcher();
            
        }

        private static void RegisterEvent()
        {
            EventBus.Register("WallPaperChanged");
            EventBus.Register("DeleteOldFile");
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
            if (item != null)
                EventBus.Publish("WallPaperChanged", new CustomEventArgs(new WallpaperChangedEventArgs(item, e.ChangeType)));

            CleanDisk();
        }


        private void CleanDisk()
        {
            var files = Directory.GetFiles(cachePath, "*", SearchOption.AllDirectories);
            if (files.Length > 100)
            {
                

                var orderFiles = files.Where(f => !FavoriteWallpaperStorage.Contains(f)).OrderByDescending(f => File.GetCreationTime(f));
                var delFiles = orderFiles.Skip(100 - FavoriteWallpaperStorage.Count);

                EventBus.Publish("DeleteOldFile", new CustomEventArgs($"{delFiles.Count()}/{files.Length}"));
                delFiles.AsParallel().ForAll(f =>
                {
                    File.Delete(f);
                });
            }
        }


        private void LoadImages(string[] files)
        {
            lock (locker)
            {
                previews.Clear();
                files.AsParallel().ForAll(p =>
                {
                    if (File.Exists(p))
                    {
                        previews.Add(LoadImage(p));
                    }
                });
            }
        }

        private WallpaperPreview LoadImage(string file)
        {

            // 切换一下线程，让之前的写文件句柄得到释放。
            Thread.Sleep(5);
            var fixedSize = WallpaperSetting.PreviewImgSize;
            // 以下代码为Coplit 自动生成，用于解决Image.FromFile的句柄占用问题

            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var bitmap = new Bitmap(stream);
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;
            float ratio = Math.Min((float)fixedSize.Width / originalWidth, (float)fixedSize.Height / originalHeight);
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);
            using var result = new Bitmap(newWidth, newHeight);
            using var graphics = Graphics.FromImage(result);
            graphics.DrawImage(bitmap, new Rectangle(0, 0, newWidth, newHeight));

            // 在这里可以对bitmap进行进一步处理
            var preview = new WallpaperPreview
            { 
                Path = file,
                Image = (Image)result.Clone()
            };
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
                WallpaperPreview[]? list = null;
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
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
