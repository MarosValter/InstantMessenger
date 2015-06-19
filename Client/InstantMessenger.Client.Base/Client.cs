using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.Base
{
    internal enum State
    {
        Connected,
        Connecting,
        Disconnected,
        Reconnecting,
    }

    internal static class Client
    {
        #region Events

        public static event EventHandler Connected;
        public static event EventHandler Disconnected;
        public static event EventHandler Reconnecting;
        public static event EventHandler Reconnected;

        #endregion

        #region Constants

        private const int ReconnectTimeout = 1000;
        private const int MaxReconnectAttempts = 3;
        private static int _reconnectAttempts = 0;
        //private const int Timeout = 3000000;

        #endregion

        #region Attributes

        private static volatile State _connectionState;

        private static TcpClient _client;
        private static string _host;
        private static int _port;
        private static SslStream _stream;

        private static readonly ConcurrentDictionary<Guid, ModelBase> ModelDictionary; 

        private static readonly BackgroundWorker SendWorker;
        private static ConcurrentQueue<TransportObject> _sendCache;
        private static volatile bool _isSending;

        private static readonly BackgroundWorker ReceiveWorker;

        private static readonly BackgroundWorker ConnectWorker;

        private static long? _myOid;

        private static bool IsConnected { get { return _connectionState == State.Connected; } }
        private static bool IsDisconnected { get { return _connectionState == State.Disconnected; } }
        private static bool IsReconnecting { get { return _connectionState == State.Reconnecting; } }

        #endregion

        #region Constructor

        static Client()
        {
            _connectionState = State.Disconnected;
            _client = new TcpClient();

            ModelDictionary = new ConcurrentDictionary<Guid, ModelBase>();

            ConnectWorker = new BackgroundWorker();
            ConnectWorker.DoWork += ConnectWorkerOnDoWork;
            ConnectWorker.RunWorkerCompleted += ConnectWorkerOnRunWorkerCompleted;

            SendWorker = new BackgroundWorker();
            SendWorker.DoWork += SendWorkerDoWork;
            SendWorker.RunWorkerCompleted += SendWorkerCompleted;

            _sendCache = new ConcurrentQueue<TransportObject>();

            ReceiveWorker = new BackgroundWorker();
            ReceiveWorker.DoWork += ReceiveWorkerDoWork;
            ReceiveWorker.RunWorkerCompleted += ReceiveWorkerCompleted;
        }

        #endregion

        #region Connection methods

        private static void Connect()
        {
            _client = new TcpClient(_host, _port);
            _stream = new SslStream(_client.GetStream(), false, UserCertificateValidationCallback);
            _stream.AuthenticateAsClient("localhost");

            _connectionState = State.Connected;
            if (!ReceiveWorker.IsBusy)
                ReceiveWorker.RunWorkerAsync();
        }

        private static void Reconnect()
        {
            if (ConnectWorker.IsBusy)
                return;

            _connectionState = State.Reconnecting;
            _reconnectAttempts = 0;

            ConnectWorker.RunWorkerAsync();

            if (Reconnecting != null)
                Reconnecting(null, null);
        }

        private static void Disconnect(bool fire)
        {
            if (IsDisconnected)
                return;

            if (_client != null)
            {
                _client.Client.Close();
            }
            if (_stream != null)
            {
                _stream.Close();
            }

            if (fire)
            {               
                _sendCache = new ConcurrentQueue<TransportObject>();
                if (Disconnected != null)
                    Disconnected(null, null);

                _connectionState = State.Disconnected;
            }
        }

        #endregion

        internal static void Init(string host, int port)
        {
            _host = host;
            _port = port;
        }

        private static void ConnectWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (Reconnected != null)
                    Reconnected(null, null);

                return;
            }

            if (_reconnectAttempts < MaxReconnectAttempts)
            {
                Disconnect(false);

                Thread.Sleep(ReconnectTimeout);
                ConnectWorker.RunWorkerAsync();
            }
            else
            {
                Disconnect(true);
            }
        }

        private static void ConnectWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            ++_reconnectAttempts;

            Connect();
        }

        private static void ReceiveWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            Reconnect();
        }

        private static void ReceiveWorkerDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                while (IsConnected)
                {
                    var to = TransportObject.Deserialize(_stream);
                    if (to == null)
                        continue;

                    var modelGuid = to.Get<Guid>("ModelGuid");
                    var flat = to.Get<UserFlat>("UserFlat");

                    var model = ModelDictionary[modelGuid];
                    if (!_myOid.HasValue && flat != null)
                    {
                        _myOid = flat.OID;
                    }

                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        model.GetResponse(to);
                    }
                    else
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => model.GetResponse(to)));
                    }
                }
            }
            catch (Exception e)
            {

            }   
        }

        private static void SendWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_sendCache.Count != 0 && IsConnected)
            {
                TransportObject to;
                _sendCache.TryDequeue(out to);

                if (to != null)
                    SendWorker.RunWorkerAsync(to);
            }
            else
            {
                _isSending = false;
            }
        }

        private static void SendWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (IsReconnecting)
                return;

            _isSending = true;
            var to = e.Argument as TransportObject;

            if (_myOid.HasValue)
            {
                to.Add("MyOid", _myOid);
            }

            try
            {
                if (IsDisconnected)
                {
                    _connectionState = State.Connecting;
                    Connect();
                    if (Connected != null)
                        Connected(null, null);
                }

                to.Serialize(_stream);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception)
            {
                _sendCache.Enqueue(to);
                Reconnect();
            }
        }      

        internal static void SendRequest(TransportObject to, ModelBase model)
        {
            var modelGuid = to.Get<Guid>("ModelGuid");
            if (!ModelDictionary.ContainsKey(modelGuid))
            {
                ModelDictionary[modelGuid] = model;
            }

            // if worker is sending data or is busy, store data and it will handle it
            // after its done
            if (_isSending || SendWorker.IsBusy)
            {
                _sendCache.Enqueue(to);
                return;
            }

            SendWorker.RunWorkerAsync(to);
        }

        #region Other methods

        private static bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion
    }   
}
