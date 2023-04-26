namespace DynamicWallpaper
{
    internal class WallpaperSetting
    {

        protected static bool AutoStart => Environment.UserInteractive;


        public static string LocalPath => AutoStart ?  (Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory) : Environment.CurrentDirectory;

        public string CachePath => Path.Combine(LocalPath, "Cache");

        //刷新时间(间隔几分钟更换壁纸)
        public int RefreshTime { get; set; } = 60;

        public string Today => DateTime.Now.ToString("yyyyMMdd");

    }
}
