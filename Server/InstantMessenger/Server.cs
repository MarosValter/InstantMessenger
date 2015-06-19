using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Data;
using InstantMessenger.Core;
using InstantMessenger.DataModel.BRO;
using InstantMessenger.DataModel.DataManagers;

namespace InstantMessenger.Server
{
    public class Server : IDisposable
    {
        #region Events

        public event EventHandler<ClientEventArgs> ClientsChanged;

        #endregion

        #region Attributes

        private bool _disposed;

        private readonly TcpListener _server;
        private readonly X509Certificate _cert;

        public ObservableCollection<ClientContext> Clients { get; private set; }

        private readonly object _lock = new object();

        #endregion

        #region Constructors

        public Server(X509Certificate cert, int port)
        {
            Clients = new ObservableCollection<ClientContext>();
            BindingOperations.EnableCollectionSynchronization(Clients, _lock);

            _cert = cert;
            _server = new TcpListener(IPAddress.Any, port);
            ObjectFactory.GetInstance<BROUsers>().LogoutAllUsers();
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

            try
            {              
                ssl.AuthenticateAsServer(_cert, false, SslProtocols.Tls, false);               
                Clients.Add(user);

                user.Read();
            }
            catch (Exception)
            {
                user.Dispose();
            }
        }

        private void UserOnDisposed(object sender, EventArgs e)
        {
            var user = sender as ClientContext;

            Clients.Remove(user);

            if (user.User != null)
            {
                ObjectFactory.GetInstance<UsersDataManager>().Logout(user.User.OID);
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _server.Stop();
                    var clients = Clients;
                    foreach (var client in clients)
                    {
                        client.Dispose();
                    }
                    clients.Clear();
                }
            }
            //dispose unmanaged resources
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
