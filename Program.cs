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


            // ����ϵͳ���̣��ṩһ���˳�ѡ�ע�ͣ�������Ĵ���ΪCoplit�Զ�����
            // 1. ����һ��NotifyIcon����  
            NotifyIcon notifyIcon = new NotifyIcon();
            // 2. ����ͼ��
            notifyIcon.Icon = new Icon("icon.ico");
            // 3. ���������ͣʱ��ʾ���ı�
            notifyIcon.Text = "��̬��ֽ";
            // 4. ��ʾ����ͼ��
            notifyIcon.Visible = true;
            // 5. ���̲˵�
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("�˳�", null, (s, e) =>
            {
                // �˳�����
                Application.Exit();
            });
            
        }
    }
}