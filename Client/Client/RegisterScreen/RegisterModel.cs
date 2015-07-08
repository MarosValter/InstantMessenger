using System.Security;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.DM;
using InstantMessenger.Communication;

namespace InstantMessenger.Client.RegisterScreen
{    
    public class RegisterModel : ModelBase<IUsersDataManager>
    {
        #region Attributes
        public string Username { get; set; }
        public SecureString PasswordRepeat { get; set; }
        public SecureString Password { get; set; }
        public string Email { get; set; }

        #endregion

        public RegisterModel()
        {
            MainMethod = i => i.Register;
        }

        protected override void CreateRequest(TransportObject to)
        {
            var pwd = Helper.HashPassword(Password);

            //to.Type = Protocol.MessageType.IM_Register;
            to.Add("Username", Username);
            to.Add("Password", pwd);
            to.Add("Email", Email);
        }
    }
}
