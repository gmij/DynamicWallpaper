﻿using DynamicWallpaper.Tools;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DynamicWallpaper
{
    internal class Program
    {
        private static Mutex? _mutex;

        private static WallpaperManager? _manager;

        private static NotifyIcon? _notifyIcon;

        private static SettingForm? _settingForm;

        internal static WallpaperManager? Manager { get => _manager; set => _manager = value; }

        private static readonly string AppName = typeof(Program).Assembly.GetName().Name ?? "DynamicWallpaper";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            MutexApp(AppName);

            RegeditHelper.SetAutoStart(AppName);

            ServiceLocator.Initialize();

            _manager = ServiceLocator.GetService<WallpaperManager>();
            _manager.Start();

            CreateNotifyIcon();

            Application.ThreadException += Application_ThreadException;

            Application.Run();

            // 释放资源，下面的代码为Coplit自动生成
            _notifyIcon?.Dispose();
            _mutex?.ReleaseMutex();
        }

       

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {

            var log = ServiceLocator.GetService<ILogger<Program>>();
            log?.LogCritical(e.Exception, "异常退出");

            MessageBox.Show($"{e.Exception.Message}\r\n{e.Exception.StackTrace}");
        }



        private static void ShowSetting()
        {
            if (_settingForm != null && _settingForm.Visible)
            {
                // 如果对话框不在居中位置时，让他回到居中位置
                _settingForm.Center();
                _settingForm.Activate();
                return;
            }

            if (_settingForm != null && !_settingForm.IsDisposed)
            {
                _settingForm.ShowDialog();
            }
            else
            {
                //  创建设置窗体
                //  处理GetService返回空引用
                _settingForm = ServiceLocator.GetService<SettingForm>();
                _settingForm?.ShowDialog();
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
            _notifyIcon.Text = ResourcesHelper.GetString("ApplicationName");

            // 4. 显示托盘图标
            _notifyIcon.Visible = true;
            // 5. 托盘菜单
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

            var rh = ServiceLocator.GetService<ResourcesHelper>();

            _notifyIcon.ContextMenuStrip.Items.Add(ResourcesHelper.GetString("Quit"), rh?.ExitImg, (s, e) =>
            {
                // 退出程序
                Application.Exit();
            });

            // 托盘图标双击时，打开设置窗体，下面的代码为Coplit自动生成
            _notifyIcon.DoubleClick += (s, e) => ShowSetting();

            // 托盘图标右键菜单中，添加一个设置菜单，下面的代码为Coplit自动生成
            _notifyIcon.ContextMenuStrip.Items.Add(ResourcesHelper.GetString("Setting"), rh?.SettingImg, (s, e) => ShowSetting());

            //  托盘图标右键菜单中，添加一个刷新菜单，
            _notifyIcon.ContextMenuStrip.Items.Add(ResourcesHelper.GetString("Refresh"), rh?.RefreshImg, (s, e) => Refresh());
        }

        private static void Refresh()
        {
            _manager?.Refresh();
        }

        private static void MutexApp(string appName)
        {
            // 使用Mutex，加入进程控制，只能启动一个应用，启动新的时候，直接退出。下面的代码为Coplit自动生成
            // 1. 创建一个Mutex对象
            _mutex = new Mutex(true, appName, out bool createdNew);
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