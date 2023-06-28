using System.Security.Cryptography;
using System.Text;

namespace DynamicWallpaper
{
    public class WallpaperPreview
    {

        public virtual string Id { get
            {
                return GetLocalHashId(Path);
            }
        }

        private static string GetLocalHashId(string? path)
        {
            // 把wallpaper中的path做hash，生成唯一值，存放在Id属性中，以下代码为Cursor生成

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(path));
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }


        public string? Path { get; set; } 

        public Image? Image { get; set; }

        // 图片的宽高比，用于确定图片适合横屏还是竖屏
        public decimal Ratio { get; set; }

        public WallpaperOrientation Orientation => CalcWallpaperOrientation(Ratio);

        public static WallpaperOrientation CalcWallpaperOrientation(decimal ratio)
        {
            return ratio switch
            {
                > (decimal)1.3 => WallpaperOrientation.Horizontal,  //  宽比大于1.1，适合横屏
                < (decimal)0.7 => WallpaperOrientation.Vertical,    //  宽比小于0.7，适合竖屏
                _ => WallpaperOrientation.Both,                     //  其他情况，两者都适合
            };
        }
    }

    //  定义一个枚举，用于标识壁纸是适合横屏还是竖屏，或者两者都适合
    public enum WallpaperOrientation
    {
        Horizontal,
        Vertical,
        Both
    }

    public class WallpaperLoading: WallpaperPreview
    {
        public override string Id => "loading";
    }
}