using System.Windows;

namespace Service1C
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Renamer renamer = new Renamer();

            CancellationToken ct = new CancellationToken();
            renamer.ExecuteAsync(ct);

        }
    }
}