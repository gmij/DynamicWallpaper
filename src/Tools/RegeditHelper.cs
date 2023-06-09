﻿using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Security.AccessControl;

namespace DynamicWallpaper.Tools
{
    static class RegeditHelper
    {

        private const string LockScreen = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PersonalizationCSP";
        private const string StartUp = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string TaskManagerDisableStartUp = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run";

        internal static void SetAutoStart(string appName)
        {
            //  加入注册表自启动项，以下代码为Copilit生成
            //  1. 创建一个RegistryKey对象
            OpenRegKey(StartUp, reg =>
            {
                reg.SetValue(appName, Application.ExecutablePath);
            });

            //  对任务管理器中禁用启动项进行处理，以下代码为Copilit生成
            OpenRegKey(TaskManagerDisableStartUp, reg =>
            {
                var disableItems = reg.GetValueNames();
                if (disableItems.Contains(appName))
                {
                    reg.DeleteValue(appName);
                }
            });

        }

        internal static string GetLockScreenImage()
        {
            var currImagePath = string.Empty;
            OpenRegKey(LockScreen, reg =>
            {
                currImagePath = reg.GetValue("LockScreenImagePath") as string;
            });
            return currImagePath;
        }

        internal static void SetLockScreenImage(string imagePath, bool force = false)
        {
            try
            {
                OpenRegKey(LockScreen, reg =>
                {
                    reg.SetValue("LockScreenImagePath", imagePath);
                    //reg.SetValue("LockScreenImageUrl", imagePath);
                    reg.SetValue("LockScreenImageStatus", 1, RegistryValueKind.DWord);
                    reg.Flush();
                }, true);
            }
            catch (UnauthorizedAccessException)
            {
                if (!force)
                    return;
                ProcessStartInfo startInfo = new()
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
#if DEBUG

                    WorkingDirectory = Path.Combine(WallpaperSetting.LocalPath, "libs"),
#else
                    WorkingDirectory = WallpaperSetting.LocalPath,
#endif
                    FileName = @"DynamicWallpaper.Command.exe",
                    Arguments = imagePath,
                    Verb = "runas"
                };

                Process.Start(startInfo);


                //  休眠2秒，等待注册表写入完成
                Thread.Sleep(2000);
                SetLockScreenImage(imagePath, true);
            }

        }

        private static void OpenRegKey(string path, Action<RegistryKey> callback, bool root = false)
        {
            var reg = (root ? Registry.LocalMachine : Registry.CurrentUser).CreateSubKey(path);
            if (reg != null)
            {
                callback(reg);
            }
            reg?.Close();
        }


    }
}
