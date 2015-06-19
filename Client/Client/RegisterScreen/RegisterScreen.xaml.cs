using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.RegisterScreen
{
    /// <summary>
    /// Interaction logic for RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : WindowBase
    {
        #region Attributes
        public new RegisterModel Model { get { return (RegisterModel)base.Model; } }

        #endregion

        #region Constructor

        public RegisterScreen()
        {
            InitializeComponent();
            Init(new RegisterModel());
            Model.BeforeProcessResponse += ModelOnBeforeProcessResponse;
        }

        #endregion

        #region Event handlers

        private void _txtPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Model.Password = (sender as PasswordBox).SecurePassword;
        }

        private void _txtPasswordConfirm_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Model.PasswordRepeat = (sender as PasswordBox).SecurePassword;
        }

        private void ModelOnBeforeProcessResponse(object sender, TransportObject transportObject)
        {
            MessageBox.Show("Successfully registered. Now you can log in.", Properties.Resources.Success,
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
            Close();
        }

        #endregion

        #region Validation

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

            if (Model.PasswordRepeat.Length == 0)
            {
                MessageBox.Show(this, Properties.Resources.E005, Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            var pwd1 = Helper.HashPassword(Model.Password);
            var pwd2 = Helper.HashPassword(Model.PasswordRepeat);
            if (!pwd1.SequenceEqual(pwd2))
            {
                MessageBox.Show(this, Properties.Resources.E004, Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        #endregion
    }
}
