using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWallpaper
{
    public class ResourcesHelper
    {
        Assembly _assembly;
        string _icoResourcePath;

        public ResourcesHelper() {

            _assembly = Assembly.GetExecutingAssembly();
            _icoResourcePath = "DynamicWallpaper.ico.";
        }


        static ResourcesHelper _instance;

        static ResourcesHelper()
        {
            _instance = new ResourcesHelper();
        }

        public static ResourcesHelper Instance => _instance;

        private Stream GetResource(string path)
        {
            if (_assembly != null) {
                var stream = _assembly.GetManifestResourceStream(path);
                if (stream != null)
                {
                    return stream;
                }
                else
                    throw new ArgumentOutOfRangeException("path", "不存在的资源");
            }
            throw new ArgumentNullException("path");
        }

        internal Image GetImage(string path)
        {
            var rPath = $"{_icoResourcePath}{path}";
            return new Bitmap(GetResource(rPath));
        }

        public Image ExitImg => GetImage("exit.ico");

        public Image ScreenImg => GetImage("screen.ico");

        public Image SettingImg => GetImage("setting.ico");

        public Image RefreshImg => GetImage("refresh.ico");

        public Image BoxImg => GetImage("box.png");

        public Image OpenBoxImg => GetImage("open_box.png");

        public Image YellowBoxImg => GetImage("yellow_box.png");

        public Image YellowOpenBoxImg => GetImage("yellow_open_box.png");

        public Image BlueBoxImg => GetImage("blue_box.png");

        public Image BlueOpenBoxImg => GetImage("blue_open_box.png");

        public Image LoveImg => GetImage("love.png");

        public Image BrokenImg => GetImage("broken.png");

        public Image MoreImg => GetImage("more.png");
    }
}
