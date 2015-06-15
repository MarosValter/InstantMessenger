
namespace InstantMessenger.Client.Base
{
    public static class Bootstrapper
    {
        public static void Init(string host, int port)
        {
            Client.Init(host, port);
        }
    }
}
