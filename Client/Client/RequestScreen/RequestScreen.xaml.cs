using System.Windows;
using System.Windows.Controls;
using InstantMessenger.Client.Base;
using InstantMessenger.Client.RegisterScreen;
using InstantMessenger.Common;

namespace InstantMessenger.Client.RequestScreen
{
    /// <summary>
    /// Interaction logic for FindScreen.xaml
    /// </summary>
    public partial class RequestScreen : WindowBase
    {
        //private readonly Client _client;
        protected new RequestModel Model { get { return (RequestModel)base.Model; } }
        public RequestScreen()
        {
            //_client = client;
            InitializeComponent();
            Init(new RequestModel());
            //_grid.ItemsSource = client.Requests;
        }


        //private void _btnAccept_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var user = (sender as Button).DataContext as RequestFlat;
        //    if (user == null)
        //        return;

        //    var error = _client.AcceptRequest(user.UserOID);

        //    if (!string.IsNullOrEmpty(error))
        //    {
        //        MessageBox.Show(error, Properties.Resources.Error, MessageBoxButton.OK);
        //    }
        //}

        //private void _btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}

        //private void _btnDelete_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var user = (sender as Button).DataContext as UserFlat;
        //    if (user == null)
        //        return;

        //    var error = _client.DeleteRequest(user.OID);

        //    if (!string.IsNullOrEmpty(error))
        //    {
        //        MessageBox.Show(error, Properties.Resources.Error, MessageBoxButton.OK);
        //    }
        //}
    }
}
