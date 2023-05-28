using System.Runtime.Serialization.Formatters.Binary;

namespace DynamicWallpaper
{
    internal class FavoriteWallpaperStorage
    {


        private static HashSet<string> _favorites;
        private readonly static string _filePath = "favoriteWallpapers.bin";

        static FavoriteWallpaperStorage()
        {
            _favorites = new HashSet<string>();
            if (File.Exists(_filePath))
            {
                using var fileStream = File.OpenRead(_filePath);
                BinaryFormatter formatter = new();
                _favorites = formatter.Deserialize(fileStream) as HashSet<string>;
            }
        }

        /// <summary>
        /// 最多设置50个喜欢的壁纸
        /// </summary>
        public static int Count => _favorites.Count > 50 ? 50: _favorites.Count;

        public static void Add(string path)
        {
            if (Count <= 50 && _favorites.Add(path))
            {
                Save();
            }
        }

        public static bool Contains(string path)
        {
            return _favorites.Contains(path);
        }

        public static void Remove(string path)
        {
            if (_favorites.Remove(path))
            {
                Save();
            }
        }

        private static void Save()
        {
            using var fileStream = File.OpenWrite(_filePath);
            BinaryFormatter formatter = new();
            formatter.Serialize(fileStream, _favorites);
        }
    }
}
