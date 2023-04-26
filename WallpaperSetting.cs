﻿namespace DynamicWallpaper
{
    internal class WallpaperSetting
    {

        public string LocalPath => Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory;

        public string CachePath => Path.Combine(LocalPath, "Cache");

        //刷新时间(间隔几分钟更换壁纸)
        public int RefreshTime { get; set; } = 60;

        public string Today => DateTime.Now.ToString("yyyyMMdd");

    }
}
