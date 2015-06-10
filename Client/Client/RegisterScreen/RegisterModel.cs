using System.Security;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.RegisterScreen
{    
    public class RegisterModel : ModelBase
    {
        #region Attributes
        public string Username { get; set; }
        public SecureString PasswordRepeat { get; set; }
        public SecureString Password { get; set; }
        public string Email { get; set; }

        #endregion

        protected override void CreateRequest(TransportObject to)
        {
            var pwd = Helper.HashPassword(Password);

            to.Type = Protocol.MessageType.IM_Register;
            to.Add("Username", Username);
            to.Add("Password", pwd);
            to.Add("Email", Email);
        }
    }
}
