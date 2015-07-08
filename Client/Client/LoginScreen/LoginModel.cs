using System.Security;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.DM;
using InstantMessenger.Communication;
using TransportObject = InstantMessenger.Communication.TransportObject;

namespace InstantMessenger.Client.LoginScreen
{
    public class LoginModel : ModelBase<IUsersDataManager>
    {
        #region Attributes

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public SecureString Password { get; set; }

        #endregion

        #region Constructor

        public LoginModel()
        {
            InitMethod = i => i.LoginUser;
            Username = Properties.Settings.Default.Username;
        }

        #endregion

        #region Overrides

        protected override void CreateRequest(TransportObject to)
        {
            var pwd = Helper.HashPassword(Password);

            //to.Type = Protocol.MessageType.IM_Login;
            to.Add("Username", Username);
            to.Add("Password", pwd);

            Properties.Settings.Default.Username = Username;
        }

        #endregion
    }
}
