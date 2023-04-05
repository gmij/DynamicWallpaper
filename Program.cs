using DynamicWallpaper.Impl;
using IDesktopWallpaperWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using NLog.Extensions.Logging;
using System.Diagnostics;

namespace DynamicWallpaper
{
    internal static class Program
    {
        private static Mutex _mutex;

        private static WallpaperManager _manager;

        private static NotifyIcon _notifyIcon;

        private static ServiceProvider _sp;

        private static SettingForm _settingForm;

        internal static WallpaperManager Manager { get => _manager; set => _manager = value; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MutexApp();

            //  加入依赖注入特性
            var services = new ServiceCollection();

            ConfigureServices(services);

            _sp = services.BuildServiceProvider();

            _manager = _sp.GetRequiredService<WallpaperManager>();
            _manager.WallpaperPoolEmpty += WhenWallpaperPoolEmpty;
            _manager.Start();

            CreateNotifyIcon();


            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ShowSetting();
            };

            Application.Run();

            // 释放资源，下面的代码为Coplit自动生成
            _notifyIcon.Dispose();
            _mutex.ReleaseMutex();

        }

        private static void WhenWallpaperPoolEmpty(object? sender, EventArgs e)
        {
            new ToastContentBuilder()
                .AddHeader("DynamicWallpaper", "消息通知", "")
                .AddText("壁纸池空啦")
                .AddText("我要去许愿，获取新壁纸~~")
                .Show();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
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

            services.AddSingleton<DesktopWallpaper>();
            services.AddSingleton<WallpaperManager>();
            services.AddSingleton<WallpaperSetting>();
            services.AddSingleton<IWallPaperPool, LocalWallpaperPool>();

        }

        private static void ShowSetting()
        {
            if (_settingForm != null && _settingForm.Visible)
            {
                _settingForm.Activate();
                return;
            }

            if (_settingForm != null && !_settingForm.IsDisposed)
            {
                _settingForm.ShowDialog();
            }
            else
            {
                _settingForm = new SettingForm();
                _settingForm.ShowDialog();
            }
        }

        private static void CreateNotifyIcon()
        {

            // 加入系统托盘，提供一个退出选项，下面的代码为Coplit自动生成
            // 1. 创建一个NotifyIcon对象  
            _notifyIcon = new NotifyIcon();
            // 2. 托盘图标
            _notifyIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            // 3. 托盘鼠标悬停时显示的文本
            _notifyIcon.Text = "动态壁纸";
            // 4. 显示托盘图标
            _notifyIcon.Visible = true;
            // 5. 托盘菜单
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();



            _notifyIcon.ContextMenuStrip.Items.Add("退出", null, (s, e) =>
            {
                // 退出程序
                Application.Exit();
            });

            // 托盘图标双击时，打开设置窗体，下面的代码为Coplit自动生成
            _notifyIcon.DoubleClick += (s, e) => ShowSetting();

            // 托盘图标右键菜单中，添加一个设置菜单，下面的代码为Coplit自动生成
            _notifyIcon.ContextMenuStrip.Items.Add("设置", null, (s, e) => ShowSetting());


            //  托盘图标右键菜单中，添加一个刷新菜单，
            _notifyIcon.ContextMenuStrip.Items.Add("刷新", null, (s, e) => Refresh());
        }

        private static void Refresh()
        {
            _manager.Refresh();
        }

        private static void MutexApp()
        {
            // 使用Mutex，加入进程控制，只能启动一个应用，启动新的时候，直接退出。下面的代码为Coplit自动生成
            // 1. 创建一个Mutex对象
            _mutex = new Mutex(true, "DynamicWallpaper", out bool createdNew);
            // 2. 如果已经存在一个进程，退出当前进程
            if (!createdNew)
            {
                MessageBox.Show("app already start...");
                // 退出当前进程
                Process.GetCurrentProcess().Kill();
                return;
            }
        }
    }
}