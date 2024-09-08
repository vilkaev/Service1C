using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Service1C
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool chkStartUp { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Renamer renamer = new Renamer();

            CancellationToken ct = new CancellationToken();
            renamer.ExecuteAsync(ct);

            ReadAutoRunState();
        }

        void ReadAutoRunState()
        {

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            var str = rk.GetValue("Service1c") ?? "";

            if (File.Exists(str.ToString()))
            {
                chb1.IsChecked = true;
            }
            else
            {
                chb1.IsChecked = false;
            }



        }

        private void changeAutoStartap(object sender, RoutedEventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (chb1.IsChecked ?? false)
                rk.SetValue("Service1c", AppContext.BaseDirectory + "Service1c.exe");
            else
                rk.DeleteValue("Service1c", false);

        }
    }
}