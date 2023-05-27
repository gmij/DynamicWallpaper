using DynamicWallpaper.Tools;

namespace DynamicWallpaper.TreasureChest
{

    internal interface IBoxOptions
    {
        /// <summary>
        /// 是否可以自动打开
        /// </summary>
        bool AutoOpen { get; set; }

        /// <summary>
        /// 重置时间
        /// </summary>
        TimeSpan ResetTime { get; set; }

        /// <summary>
        /// 最大累计宝箱数
        /// </summary>
        int MaxCount { get; set; }


        /// <summary>
        /// 随机收获数
        /// </summary>
        int RandomHarvest { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        BoxStatus Status { get; set; }

        /// <summary>
        /// 箱子外观
        /// </summary>
        BoxAppearance Appearance { get;  }

        /// <summary>
        /// 箱子的来源信息
        /// </summary>
        string SourceRemark { get; }

        DateTime LastOpenTime { get; set; }

        string ListUrl { get; }

        string ItemBaseUrl { get; }

        string ApiKey { get; }

        BoxStyle Style { get; }

    }

    /// <summary>
    /// 箱子状态
    /// </summary>
    public enum BoxStatus
    {
        /// <summary>
        /// 开启状态
        /// </summary>
        Open,
        /// <summary>
        /// 关闭状态
        /// </summary>
        Close,
        /// <summary>
        /// 打开中（正在打开1个，获取相关的资源）
        /// </summary>
        Opening,
        /// <summary>
        /// 有多个待打开的宝箱
        /// </summary>
        Many,
        /// <summary>
        /// 冷却中，无法打开，需要等待重置时间到了才能打开
        /// </summary>
        Cooling,
    }


    public enum BoxAppearance
    {
        normal,
        yellow,
        blue,
    }

    public class BoxStyle
    {

        private readonly string closeImgName = "box.png";
        private readonly string openImgName = "open_box.png";

        public BoxStyle(BoxAppearance appearance)
        {

            if (appearance != BoxAppearance.normal)
            {
                closeImgName = $"{appearance}_{closeImgName}";
                openImgName = $"{appearance}_{openImgName}";
            }

            OpenBox = ResourcesHelper.Instance.GetImage($"{openImgName}");
            CloseBox = ResourcesHelper.Instance.GetImage($"{closeImgName}");
        }

        public Image OpenBox { get; }

        public Image CloseBox { get; }
    }

}
