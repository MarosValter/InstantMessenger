using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.TransportObject;
using MessageBox = System.Windows.MessageBox;

namespace InstantMessenger.Client.LoginScreen
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : WindowBase
    {
        #region Attributes
        public new LoginModel Model { get { return (LoginModel)base.Model; } }

        //private Client _client;
        public bool Result { get; private set; }

        #endregion

        #region Constructor
        public LoginScreen()
        {

            Init(new LoginModel());
            //_client = client;
            InitializeComponent();
            Model.DataReceived += ModelOnDataReceived;
            _txtUsername.Focus();
        }

        #endregion

        public new bool ShowDialog()
        {
            base.ShowDialog();

            return Model.Success;
        }

        #region Event handlers

        private void ModelOnDataReceived(object sender, TransportObject transportObject)
        {
            if (Model.Success)
            {
                Close();
            }
            else
            {
                MessageBox.Show(this, Model.Error, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            var regScr = new RegisterScreen.RegisterScreen();
            var result = regScr.ShowDialog();
            if (result.HasValue && result.Value)
            {
                MessageBox.Show(Properties.Resources.RegisterOK, Properties.Resources.Success,
                                MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void _txtPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Model.Password = (sender as PasswordBox).SecurePassword;
        }

        #endregion Event handlers

        protected override bool Validate()
        {
            if (string.IsNullOrEmpty(Model.Username))
            {
                MessageBox.Show(this, Properties.Resources.E003, Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (Model.Password.Length == 0)
            {
                MessageBox.Show(this, Properties.Resources.E002, Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }
    }
}
