namespace DynamicWallpaper
{
    internal class WallpaperSetting
    {

        public string LocalPath => Environment.CurrentDirectory;

        public string CachePath => Path.Combine(LocalPath, "Cache");

        //刷新时间(间隔几分钟更换壁纸)
        public int RefreshTime { get; set; } = 60;

    }
}
