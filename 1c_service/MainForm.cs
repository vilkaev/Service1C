using System.Resources;

namespace _1c_service
{
    public partial class MainForm : Form
    {
    

        public MainForm()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        void Exit(object? sender, EventArgs e)
        {
            //trayIcon.Visible = false;
            Application.Exit();
        }


    }
}
