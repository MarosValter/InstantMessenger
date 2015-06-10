using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace InstantMessenger.Client
{
    public static class Helper
    {
        public static byte[] HashPassword(SecureString password)
        {
            var sha = new SHA512Managed();
            var bstr = Marshal.SecureStringToBSTR(password);
            var length = Marshal.ReadInt32(bstr, -4);
            var bytes = new byte[length];

            var bytesPin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                Marshal.Copy(bstr, bytes, 0, length);
                var result = sha.ComputeHash(bytes);

                return result;
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
                bytesPin.Free();
            }
        }
    }
}
