using Microsoft.Xaml.Behaviors;
using System.Windows;


namespace Service1C
{
    public class NotifyTrayIconBehavior : Behavior<Window>//Attach behavior with window
    {
        private NotifyIcon? notifyTrayIcon;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Closing += AssociatedObject_Closing;
            AssociatedObject.StateChanged += AssociatedObject_StateChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Closing -= AssociatedObject_Closing;
            AssociatedObject.StateChanged -= AssociatedObject_StateChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeNotifyIcon();
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            AssociatedObject.Hide();
            ShowNotificationInTray("", "Приложение помещено в системный трей.");
        }

        private void AssociatedObject_StateChanged(object sender, EventArgs e)
        {
            if (AssociatedObject.WindowState == WindowState.Minimized)
            {
                AssociatedObject.ShowInTaskbar = false;
                ShowNotificationInTray("", "Приложение помещено в системный трей.");
            }
            else
            {
                AssociatedObject.ShowInTaskbar = true;
            }
        }

        private void InitializeNotifyIcon()
        {
            // Initialize NotifyIcon
            notifyTrayIcon = new NotifyIcon
            {
                Icon = GetIconFromImageSource(new Uri("pack://application:,,,/Service1C;component/TrayIcon.ico")),//Specify the icon to appear in the notification area.
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip(),
                Text = "Service 1C"
            };

            notifyTrayIcon.MouseClick += OpenMenuItem_Click;

            //// Add a context menu to the NotifyIcon
            ContextMenuStrip contextMenu = new();
            ToolStripMenuItem exitNotifyIconMenuItem = new()
            {
                Text = "Exit"
            };
            ToolStripMenuItem openNotifyIconMenuItem = new()
            {
                Text = "Open App",
                
            };
            
            openNotifyIconMenuItem.Click += OpenMenuItem_Click;
            exitNotifyIconMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(openNotifyIconMenuItem);
            contextMenu.Items.Add(exitNotifyIconMenuItem);
            notifyTrayIcon.ContextMenuStrip = contextMenu;
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            // Perform cleanup and exit the application
            notifyTrayIcon?.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if ( e != null && (e as MouseEventArgs).Button == MouseButtons.Left)
            {
                AssociatedObject.Show();
            }
            AssociatedObject.WindowState = WindowState.Normal;
            AssociatedObject.Activate();
        }

        private void ShowNotificationInTray(string title, string message)
        {
            //To showcase a balloon tip, utilize the notifyTrayIcon function and specify the duration (2000 milliseconds), title, message, and icon type (ToolTipIcon.Info).
            notifyTrayIcon.ShowBalloonTip(2000, title, message, ToolTipIcon.Info);
        }

        private Icon? GetIconFromImageSource(Uri uri)
        {
            using var stream = System.Windows.Application.GetResourceStream(uri)?.Stream;
            return stream != null ? new Icon(stream) : null;
        }
    }
}
