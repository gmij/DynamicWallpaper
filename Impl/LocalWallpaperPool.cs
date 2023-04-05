namespace DynamicWallpaper.Impl
{
    internal class LocalWallpaperPool : IWallPaperPool
    {
        private readonly string _cachePath;
        private readonly string[] _wallPaperList;

        public LocalWallpaperPool(WallpaperSetting setting)
        {
            _cachePath = setting.CachePath;
            if (Directory.Exists(_cachePath))
            {
                _wallPaperList = Directory.GetFiles(_cachePath, "*", SearchOption.AllDirectories);
            }
            else
            {
                Directory.CreateDirectory(_cachePath);
            }
        }
        

        public string Renew(string excludePath)
        {
            if (_wallPaperList.Length >0)
            {
                var list = _wallPaperList.Where(p => p != excludePath).ToArray();
                var poolSize = list.Count();
                return list[new Random().Next(poolSize)];
            }
            return string.Empty;
        }
    }
}
