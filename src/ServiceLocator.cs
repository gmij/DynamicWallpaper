using DynamicWallpaper.Impl;
using DynamicWallpaper.TreasureChest;
using IDesktopWallpaperWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DynamicWallpaper
{
    internal static class ServiceLocator
    {

        private static IServiceProvider _serviceProvider;

        public static void Initialize()
        {
            var services = new ServiceCollection();
            Initialize(services);
        }

        public static void Initialize(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                         .SetBasePath(WallpaperSetting.LocalPath)
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .Build();

            //  开启日志
            services.AddLogging(builder =>
            {
                // configure Logging with NLog
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(config);
            });
            // 添加多语言支持
            services.AddLocalization(options => options.ResourcesPath = "res");

            services.AddSingleton<Tools.ResourcesHelper>();
            services.AddSingleton<DesktopWallpaper>();
            services.AddSingleton<SettingForm>();
            services.AddSingleton<WallpaperManager>();
            services.AddSingleton<WallpaperSetting>();
            services.AddSingleton<IWallPaperPool, LocalWallpaperPool>();

            services.AddSingleton<ProgressLog>();
            //services.AddSingleton<WoodenBox>();
            //services.AddSingleton<IronBox>();
            //services.AddSingleton<INetworkPaperProvider, BingDailyWallpaper>();
            //services.AddSingleton<INetworkPaperProvider, PixabayWallpaperPool>();
            //services.AddSingleton<INetworkPaperProvider, WallhavenWallpaperPool>();



            _serviceProvider = services.BuildServiceProvider();


            services.AddSingleton<ITreasureChest>(TreasureChestBuilderFactory.CreateTreasureChest4Bing());
            services.AddSingleton<ITreasureChest>(TreasureChestBuilderFactory.CreateTreasureChest4Pixabay());
            services.AddSingleton<ITreasureChest>(TreasureChestBuilderFactory.CreateTreasureChest4Wallhaven());

            _serviceProvider = services.BuildServiceProvider();
        }

        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }

    }
}
