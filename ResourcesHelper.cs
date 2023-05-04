﻿using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;

namespace DynamicWallpaper
{
    public class ResourcesHelper
    {
        private Assembly _assembly;
        private string _icoResourcePath = "DynamicWallpaper.ico.";
        private ILogger<ResourcesHelper> logger;

        public ResourcesHelper(ILogger<ResourcesHelper> logger)
        {
            _assembly = Assembly.GetExecutingAssembly();
            this.logger = logger;
        }


        private static readonly Lazy<ResourcesHelper> _instance = new(() =>
        {
            var logger = ServiceLocator.GetService<ILogger<ResourcesHelper>>();
            return new ResourcesHelper(logger);
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        public static ResourcesHelper Instance => _instance.Value;

        public static string GetString(string name)
        {
            var service = ServiceLocator.GetService<IStringLocalizer<ResourcesHelper>>();
            return service.GetString($"{name}");
        }

        public static string GetString<T>(string name)
        {
            var service = ServiceLocator.GetService<IStringLocalizer<T>>();
            return service.GetString($"{name}");
        }

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
            else
            {
                logger.LogError($"无效的资源路径:{path}");
                throw new ArgumentNullException("path");
            }
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
