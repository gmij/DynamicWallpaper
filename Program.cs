namespace DynamicWallpaper
{
    internal static class Program
    {



        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());


            // 加入系统托盘，提供一个退出选项（注释），下面的代码为Coplit自动生成
            // 1. 创建一个NotifyIcon对象  
            NotifyIcon notifyIcon = new NotifyIcon();
            // 2. 托盘图标
            notifyIcon.Icon = new Icon("icon.ico");
            // 3. 托盘鼠标悬停时显示的文本
            notifyIcon.Text = "动态壁纸";
            // 4. 显示托盘图标
            notifyIcon.Visible = true;
            // 5. 托盘菜单
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("退出", null, (s, e) =>
            {
                // 退出程序
                Application.Exit();
            });
            
        }
    }
}