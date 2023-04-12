using Microsoft.Extensions.Logging;

namespace DynamicWallpaper.Impl
{
    internal class LocalWallpaperPool : IWallPaperPool
    {
        private readonly string _cachePath;
        private string[] _wallPaperList;
        private readonly ILogger<LocalWallpaperPool> _logger;

        public LocalWallpaperPool(WallpaperSetting setting, ILogger<LocalWallpaperPool> logger)
        {
            _cachePath = setting.CachePath;
            _wallPaperList = new string[0];
            ReLoadCacheImage();
            _logger = logger;
        }

        public bool IsEmpty => _wallPaperList.Length == 0;

        private void ReLoadCacheImage()
        {
            if (!string.IsNullOrEmpty(_cachePath))
            {
                if (Directory.Exists(_cachePath))
                {
                    //  单机程序，不考虑并发锁了。
                    _wallPaperList = Directory.GetFiles(_cachePath, "*", SearchOption.AllDirectories);
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
