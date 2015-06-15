using System.Windows;
using InstantMessenger.Client.Base;
using InstantMessenger.Client.Properties;
using Application = System.Windows.Application;

namespace InstantMessenger.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Bootstrapper.Init(Settings.Default.Hostname, Settings.Default.Port);

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
