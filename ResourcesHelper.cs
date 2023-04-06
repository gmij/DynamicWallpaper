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

        private Stream GetResource(string path)
        {
            if (_assembly != null) {
                return _assembly.GetManifestResourceStream(path);
            }
            throw new ArgumentNullException("path");
        }

        private Image GetImage(string path)
        {
            return new Bitmap(GetResource(path));
        }

        public Image ExitImg => GetImage($"{_icoResourcePath}exit.ico");

        public Image ScreenImg => GetImage($"{_icoResourcePath}screen.ico");

        public Image SettingImg => GetImage($"{_icoResourcePath}setting.ico");

        public Image RefreshImg => GetImage($"{_icoResourcePath}refresh.ico");
    }
}
