using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using InstantMessenger.Core;
using InstantMessenger.DataModel.DataManagers;

namespace InstantMessenger.Server
{
    public class Server : IDisposable
    {
        #region Events

        public event EventHandler<ClientEventArgs> ClientsChanged;

        #endregion

        #region Attributes

        private readonly TcpListener _server;
        private readonly X509Certificate _cert;

        private bool _continueTransmission = false;

        public ISet<ClientContext> Clients { get; private set; }

        private readonly object _lock = new object();

        #endregion

        #region Constructors

        public Server(X509Certificate cert, int port)
        {
            Clients = new HashSet<ClientContext>();
            _cert = cert;
            _server = new TcpListener(IPAddress.Any, port);
            Start(); 
        }

        public Server(string certPath, int port)
            : this(X509Certificate.CreateFromCertFile(certPath), port)
        {
        }

        #endregion

        #region Public methods

        public void Start()
        {
            try
            {
                _server.Start();
                _server.BeginAcceptTcpClient(OnConnection, _server);
            }
            catch (Exception)
            {
                
            }
        }

        public void Stop()
        {
            _server.Stop();
        }

        #endregion

        #region Event handlers

        public void OnConnection(IAsyncResult iar)
        {
            var server = (TcpListener) iar.AsyncState;

            // if socket was closed
            if (server.Server == null || !server.Server.IsBound)
                return;

            TcpClient client = null;
            try
            {
                client = server.EndAcceptTcpClient(iar);            
            }
            catch (SocketException ex)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                if (client != null)
                    client.Close();
                return;
            }

            // Handle new connections
            server.BeginAcceptTcpClient(OnConnection, server);
            
            var stream = client.GetStream();
            var ssl = new SslStream(stream, false);
            var user = new ClientContext(client, ssl);
            user.Disposed += UserOnDisposed;
            user.LoggedIn += UserOnLoggedIn;

            try
            {              
                ssl.AuthenticateAsServer(_cert, false, SslProtocols.Tls, false);
                
                ClientEventArgs args;

                lock (_lock)
                {
                    Clients.Add(user);
                    args = new ClientEventArgs(Clients);
                }

                if (ClientsChanged != null)
                    ClientsChanged(this, args);

                user.Read();
            }
            catch (Exception)
            {
                user.Dispose();
            }
        }

        private void UserOnLoggedIn(object sender, EventArgs eventArgs)
        {
            ClientEventArgs args;
            lock (_lock)
            {
                args = new ClientEventArgs(Clients);
            }

            if (ClientsChanged != null)
                ClientsChanged(this, args);
        }

        private void UserOnDisposed(object sender, EventArgs e)
        {
            var user = sender as ClientContext;

            ClientEventArgs args;

            lock (_lock)
            {
                Clients.Remove(user);
                args = new ClientEventArgs(Clients);
            }

            if (ClientsChanged != null)
                ClientsChanged(this, args);

            if (user.User != null)
            {
                UsersDataManager.Logout(user.User.OID);
            }
        }

        #endregion

        #region IDisposable member

        public void Dispose()
        {
            _server.Stop();
            foreach (var client in Clients)
            {
                client.Dispose();
            }
        }

        #endregion
    }
}
