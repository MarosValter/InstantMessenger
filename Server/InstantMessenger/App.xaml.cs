using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using InstantMessenger.Core;
using InstantMessenger.Server.Properties;

namespace InstantMessenger.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Bootstrapper.Init();
            var certPath = Path.Combine(Environment.CurrentDirectory, Settings.Default.RelativeCertPath, Settings.Default.CertificateName);
            var cert = new X509Certificate2(certPath, Settings.Default.CertificatePassword);

            var main = new MainWindow(cert, Settings.Default.Port);
            main.ShowDialog();
        }
    }
}
