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

    }

    public class WallpaperLoading: WallpaperPreview
    {
        public override string Id => "loading";
    }
}