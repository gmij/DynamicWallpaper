using Microsoft.Win32;

namespace DynamicWallpaper.Tools
{
    static class RegeditHelper
    {
        internal static void SetAutoStart(string appName)
        {
            //  加入注册表自启动项，以下代码为Copilit生成
            //  1. 创建一个RegistryKey对象
            var reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (reg != null)
            {
                // 2. 设置自启动项
                reg.SetValue(appName, Application.ExecutablePath);

                reg.Close();
            }

            //  对任务管理器中禁用启动项进行处理，以下代码为Copilit生成
            reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run", true);
            if (reg != null)
            {
                var disableItems = reg.GetValueNames();
                if (disableItems.Contains(appName))
                {
                    reg.DeleteValue(appName);
                }
            }
            reg?.Close();
        }
    }
}
