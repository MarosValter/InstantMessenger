using System.Windows;
using System.Windows.Forms;
using InstantMessenger.Client.Properties;
using Application = System.Windows.Application;

namespace InstantMessenger.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Application_Startup(object sender, StartupEventArgs e)
        {
            Base.Client.Init(Settings.Default.Hostname, Settings.Default.Port);
            //Base.Client.Connect(true);

            var login = new LoginScreen.LoginScreen();
            var main = new MainWindow.MainWindow();

            var logged = login.ShowDialog();
            if (!logged)
            {
                Close();
                return;
            }
            main.ShowDialog();
        }

        private static void Close()
        {
            
        }

    }
}
