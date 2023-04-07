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

        public List<WallpaperPreview> GetLocalWallpaperPreviews()
        {
            //  遍历_wallPaperList，并根据存储的路径，加载对应的图片文件，并返回图片
            var previews = new List<WallpaperPreview>();
            foreach (var file in _wallPaperList)
            {
                if (File.Exists(file))
                {
                    var preview = new WallpaperPreview
                    {
                        Path = file,
                        Image = Image.FromFile(file)
                    };
                    previews.Add(preview);
                }
            }
            return previews;
        }
    }
}
