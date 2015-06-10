using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace InstantMessenger.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Server _server;
        public MainWindow(X509Certificate2 cert, int port)
        {
            InitializeComponent();

            _server = new Server(cert, port);
            Grid.ItemsSource = _server.Clients;
            _server.ClientsChanged += ServerOnClientsChanged;
        }

        private void ServerOnClientsChanged(object sender, ClientEventArgs clientEventArgs)
        {
            Grid.Dispatcher.Invoke(Grid.Items.Refresh);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _server.Dispose();
        }
    }
}
