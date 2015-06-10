using System.Security.Cryptography;
using System.Text;

namespace InstantMessenger.Client.Base
{
    public static class Helper
    {
        public static byte[] HashPassword(string password)
        {
            var sha = new SHA512Managed();
            return sha.ComputeHash(Encoding.Unicode.GetBytes(password));
        }
    }
}
