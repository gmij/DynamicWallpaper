using DynamicWallpaper.Impl;
using IDesktopWallpaperWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using NLog.Extensions.Logging;
using System.Diagnostics;

namespace DynamicWallpaper
{
    internal static class Program
    {
        private static Mutex? _mutex;

        private static WallpaperManager? _manager;

        private static NotifyIcon? _notifyIcon;

        private static ServiceProvider? _sp;

        private static SettingForm? _settingForm;

        internal static WallpaperManager? Manager { get => _manager; set => _manager = value; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MutexApp();

            //  ����ע�������������´���ΪCopilit����
            //  1. ����һ��RegistryKey����
            var reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            // 2. ������������
            reg?.SetValue("DynamicWallpaper", Application.ExecutablePath);


            //  ��������ע������
            var services = new ServiceCollection();

            ConfigureServices(services);

            _sp = services.BuildServiceProvider();

            _manager = _sp.GetRequiredService<WallpaperManager>();
            //_manager.WallpaperPoolEmpty += WhenWallpaperPoolEmpty;
            _manager.Start();
            
            CreateNotifyIcon();


            Application.ThreadException += Application_ThreadException;

            Application.Run();

            // �ͷ���Դ������Ĵ���ΪCoplit�Զ�����
            _notifyIcon?.Dispose();
            _mutex?.ReleaseMutex();

        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (_sp != null)
            {
                var log = _sp.GetService<ILogger>();
                log.LogCritical(e.Exception, "�쳣�˳�");
            }
            MessageBox.Show(e.ToString());
        }


        private static void ConfigureServices(ServiceCollection services)
        {
            var rootPath = Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory;
            //var rootPath = "E:\\";

            var config = new ConfigurationBuilder()
                         .SetBasePath(rootPath)
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

            services.AddSingleton<ResourcesHelper>();
            services.AddSingleton<DesktopWallpaper>();
            services.AddSingleton<SettingForm>();
            services.AddSingleton<WallpaperManager>();
            services.AddSingleton<WallpaperSetting>();
            services.AddSingleton<IWallPaperPool, LocalWallpaperPool>();

            services.AddSingleton<WoodenBox>();
            services.AddSingleton<IronBox>();
            services.AddSingleton<INetworkPaperProvider, BingDailyWallpaper>();
            services.AddSingleton<INetworkPaperProvider, PixabayWallpaperPool>();
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
                //  �������ô���
                //  ����GetService���ؿ�����

                _settingForm = _sp?.GetService<SettingForm>();
                _settingForm?.ShowDialog();
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

            var rh = _sp?.GetService<ResourcesHelper>();

            _notifyIcon.ContextMenuStrip.Items.Add("�˳�", rh?.ExitImg, (s, e) =>
            {
                // �˳�����
                Application.Exit();
            });

            // ����ͼ��˫��ʱ�������ô��壬����Ĵ���ΪCoplit�Զ�����
            _notifyIcon.DoubleClick += (s, e) => ShowSetting();

            // ����ͼ���Ҽ��˵��У����һ�����ò˵�������Ĵ���ΪCoplit�Զ�����
            _notifyIcon.ContextMenuStrip.Items.Add("����", rh?.SettingImg, (s, e) => ShowSetting());

            //  ����ͼ���Ҽ��˵��У����һ��ˢ�²˵���
            _notifyIcon.ContextMenuStrip.Items.Add("ˢ��", rh?.RefreshImg, (s, e) => Refresh());
        }

        private static void Refresh()
        {
            _manager?.Refresh();
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