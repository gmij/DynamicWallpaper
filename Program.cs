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

            //  ��������ע������
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

            // �ͷ���Դ������Ĵ���ΪCoplit�Զ�����
            _notifyIcon.Dispose();
            _mutex.ReleaseMutex();

        }

        private static void WhenWallpaperPoolEmpty(object? sender, EventArgs e)
        {
            new ToastContentBuilder()
                .AddHeader("DynamicWallpaper", "��Ϣ֪ͨ", "")
                .AddText("��ֽ�ؿ���")
                .AddText("��Ҫȥ��Ը����ȡ�±�ֽ~~")
                .Show();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .Build();

            //  ������־
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

            // ����ϵͳ���̣��ṩһ���˳�ѡ�����Ĵ���ΪCoplit�Զ�����
            // 1. ����һ��NotifyIcon����  
            _notifyIcon = new NotifyIcon();
            // 2. ����ͼ��
            _notifyIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            // 3. ���������ͣʱ��ʾ���ı�
            _notifyIcon.Text = "��̬��ֽ";
            // 4. ��ʾ����ͼ��
            _notifyIcon.Visible = true;
            // 5. ���̲˵�
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();



            _notifyIcon.ContextMenuStrip.Items.Add("�˳�", null, (s, e) =>
            {
                // �˳�����
                Application.Exit();
            });

            // ����ͼ��˫��ʱ�������ô��壬����Ĵ���ΪCoplit�Զ�����
            _notifyIcon.DoubleClick += (s, e) => ShowSetting();

            // ����ͼ���Ҽ��˵��У����һ�����ò˵�������Ĵ���ΪCoplit�Զ�����
            _notifyIcon.ContextMenuStrip.Items.Add("����", null, (s, e) => ShowSetting());


            //  ����ͼ���Ҽ��˵��У����һ��ˢ�²˵���
            _notifyIcon.ContextMenuStrip.Items.Add("ˢ��", null, (s, e) => Refresh());
        }

        private static void Refresh()
        {
            _manager.Refresh();
        }

        private static void MutexApp()
        {
            // ʹ��Mutex��������̿��ƣ�ֻ������һ��Ӧ�ã������µ�ʱ��ֱ���˳�������Ĵ���ΪCoplit�Զ�����
            // 1. ����һ��Mutex����
            _mutex = new Mutex(true, "DynamicWallpaper", out bool createdNew);
            // 2. ����Ѿ�����һ�����̣��˳���ǰ����
            if (!createdNew)
            {
                MessageBox.Show("app already start...");
                // �˳���ǰ����
                Process.GetCurrentProcess().Kill();
                return;
            }
        }
    }
}