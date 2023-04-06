namespace DynamicWallpaper
{
    public interface IWallPaperPool
    {
        /// <summary>
        /// 换一个新壁纸
        /// </summary>
        /// <param name="excludePath">排除当前的壁纸</param>
        /// <returns></returns>
        string Renew(string excludePath);

        /// <summary>
        /// 壁纸池是否为空
        /// </summary>
        bool IsEmpty { get; }

        //  获取所有本地图片的预览图
        List<WallpaperPreview> GetLocalWallpaperPreviews();
    }
}