namespace DynamicWallpaper
{
    internal interface IWallPaperPool
    {
        /// <summary>
        /// 换一个新壁纸
        /// </summary>
        /// <param name="excludePath">排除当前的壁纸</param>
        /// <returns></returns>
        string Renew(string excludePath);
    }
}