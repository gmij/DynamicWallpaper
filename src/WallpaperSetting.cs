namespace DynamicWallpaper
{
    internal class WallpaperSetting
    {

        protected static bool AutoStart => Environment.UserInteractive;


        public static string LocalPath => AutoStart ?  (Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory) : Environment.CurrentDirectory;

        public static Size PreviewImgSize => new(220, 180);

        public string CachePath { get; private set; } = Path.Combine(LocalPath, "Cache");

        //刷新时间(间隔几分钟更换壁纸)
        public int RefreshTime { get; set; } = 60;

        public string Today => DateTime.Now.ToString("yyyyMMdd");

        public WallpaperSetting() { 
        
            //  Check CachePath Auth
            if (!Directory.Exists(CachePath))
            {
                try
                {
                    Directory.CreateDirectory(CachePath);
                }
                catch(UnauthorizedAccessException)
                {
                    var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    if (appDataDir != null)
                    {
                        var cacheDir = Path.Combine(appDataDir, "DynamicWallpaper", "Cache");
                        if (!Directory.Exists(cacheDir))
                        {
                            Directory.CreateDirectory(cacheDir);
                        }
                        CachePath = cacheDir;
                    }
                    else
                    {
                        throw new Exception("缓存目录无法创建");
                    }
                }
            }
        
        }

    }
}
