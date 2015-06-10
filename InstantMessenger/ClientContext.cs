using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;
using InstantMessenger.Common.TransportObject;
using InstantMessenger.DataModel.DataManagers;

namespace InstantMessenger.Server
{
    public class ClientContext : IDisposable
    {
        public event EventHandler LoggedIn;
        public event EventHandler Disposed;

        //public const int BufferSize = 5000;

        public TcpClient Socket { get; set; }

        private SslStream _stream;
        public SslStream Stream
        {
            get { return _stream; }
            set
            {
                if (_stream == value)
                    return;

                if (_stream != null)
                    _stream.Dispose();

                _stream = value;
            }
        }

        public string IPAddress { get; private set; }

        public UserFlat User { get; set; }


        public ClientContext(TcpClient client, SslStream stream)
        {
            Socket = client;
            Stream = stream;
            IPAddress = ((IPEndPoint) (Socket.Client.RemoteEndPoint)).Address.ToString();
        }

        public void Read()
        {
            try
            {
                var message = TransportObject.Deserialize(Stream);
                if (message != null)
                {
                    Process(message);
                }
                else
                {
                    var result = new TransportObject(Protocol.MessageType.IM_ERROR);
                    result.Add("Error", "Object was not succesfully received.");
                    result.Serialize(Stream);
                }
                Read();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                Dispose();
            }
        }

        private void Process(TransportObject to)
        {
            var result = new TransportObject();
            switch (to.Type)
            {
                case Protocol.MessageType.IM_Login:

                    result = UsersDataManager.LoginUser(to);
                    if (result.Type == Protocol.MessageType.IM_OK)
                    {
                        User = result.Get<UserFlat>("UserFlat");
                        if (LoggedIn != null)
                            LoggedIn(this, null);
                    }
                    break;
                case Protocol.MessageType.IM_FriendsRequests:
                    result = UsersDataManager.MainWindowInit(to);
                    break;
                case Protocol.MessageType.IM_Register:
                    result = UsersDataManager.Register(to);
                    break;
                case Protocol.MessageType.IM_Find:
                    result = UsersDataManager.FindUsers(to);
                    break;
                case Protocol.MessageType.IM_GetRequests:
                    result = FriendshipsDataManager.GetRequests(to);
                    break;
                case Protocol.MessageType.IM_Add:
                    result = FriendshipsDataManager.SendRequest(to);
                    break;
                case Protocol.MessageType.IM_Accept:
                    result = FriendshipsDataManager.AcceptRequest(to);
                    break;
                case Protocol.MessageType.IM_DeleteRequest:
                    result = FriendshipsDataManager.DeleteRequest(to);
                    break;
            }
            
            result.Serialize(Stream);
        }

        public void Dispose()
        {
            Socket.Client.Close();
            Socket.Close();
            Stream.Close();
            Stream.Dispose();

            if (Disposed != null)
                Disposed(this, null);
        }
    }
}
