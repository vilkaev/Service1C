namespace _1c_service
{
    internal static class Program
    {

        //private NotifyIcon trayIcon;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();


            //NotifyIcon trayIcon = new NotifyIcon()
            //{
            //    ContextMenuStrip = new ContextMenuStrip()
            //    {
            //        Items = { new ToolStripMenuItem("Exit", null, Exit) }
            //    },
            //    Visible = true
            //};

            var form = new MainForm();
            form.Hide();
            form.Opacity = 0;
            form.ShowInTaskbar = false;

            //Application.WindowState = FormWindowState.Minimized;
            //Application.ShowInTaskbar = false;

            Application.Run(form);




        }

    }
}