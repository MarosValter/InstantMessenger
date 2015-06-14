using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Timers;
using System.Windows;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;
using InstantMessenger.Common.TransportObject;
using Timer = System.Timers.Timer;

namespace InstantMessenger.Client.Base
{
    public enum State
    {
        Connected,
        Disconnected,
        Reconnecting,
    }
    public static class Client
    {
        #region Events

        public static event EventHandler Connected;
        public static event EventHandler Disconnected;
        public static event EventHandler<int> Reconnecting;

        #endregion

        #region Constants

        private const int ReconnectTimeout = 3000;
        private const int ReconnectAttempts = 3;
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
        //private static readonly Timer SendTimer;
        private static readonly Queue<TransportObject> SendCache;
        private static volatile bool _isSending;

        private static readonly BackgroundWorker ReceiveWorker;

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

            //SendTimer = new Timer(Timeout);
            //SendTimer.Elapsed += SendTimerTimeout;

            SendWorker = new BackgroundWorker();
            SendWorker.DoWork += SendWorkerDoWork;
            SendWorker.RunWorkerCompleted += SendWorkerCompleted;

            SendCache = new Queue<TransportObject>();

            ReceiveWorker = new BackgroundWorker();
            ReceiveWorker.DoWork += ReceiveWorkerDoWork;
            ReceiveWorker.RunWorkerCompleted += ReceiveWorkerCompleted;
        }

        #endregion

        #region Connection methods

        public static void Connect()
        {
            if (IsConnected)
                return;

            try
            {
                _client = new TcpClient(_host, _port);
                _stream = new SslStream(_client.GetStream(), false, UserCertificateValidationCallback);
                _stream.AuthenticateAsClient("localhost");

                _connectionState = State.Connected;
                if (!ReceiveWorker.IsBusy)
                    ReceiveWorker.RunWorkerAsync();

                if (Connected != null)
                    Connected(null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void Reconnect()
        {
            if (IsReconnecting)
                return;

            _connectionState = State.Reconnecting;
            var attempt = 1;

            Disconnect(false);

            while (IsDisconnected && attempt <= ReconnectAttempts)
            {
                if (Reconnecting != null)
                    Reconnecting(null, attempt);

                try
                {
                    Connect();
                    return;
                }
                catch (Exception)
                {
                    ++attempt;
                    Thread.Sleep(ReconnectTimeout);
                }
            }

            Disconnect(true);
        }

        public static void Disconnect(bool fire)
        {
            if (!IsConnected)
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
                _connectionState = State.Disconnected;
                if (Disconnected != null)
                    Disconnected(null, null);
            }
        }

        #endregion

        public static void Init(string host, int port)
        {
            _host = host;
            _port = port;
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
                    var modelGuid = to.Get<Guid>("ModelGuid");
                    var flat = to.Get<UserFlat>("UserFlat");

                    var model = ModelDictionary[modelGuid];
                    if (!_myOid.HasValue)
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
            //SendTimer.Stop();

            if (SendCache.Count != 0 && e.Error != null)
            {
                var to = SendCache.Dequeue();
                SendWorker.RunWorkerAsync(to);
            }
            else
            {
                _isSending = false;
            }
        }

        private static void SendWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            _isSending = true;

            if (_host.IsNullOrEmpty())
            {
                throw new Exception("Host is not set. You must set host and port before sending request");
            }

            if (!IsConnected)
            {
                return;
            }

            var to = e.Argument as TransportObject;
            try
            {
                to.Serialize(_stream);
            }
            catch (Exception)
            {
                Reconnect();
            }
        }

        //private static void SendTimerTimeout(object sender, ElapsedEventArgs e)
        //{
        //    Reconnect();
        //}
      

        public static void SendRequest(TransportObject to, ModelBase model)
        {
            var modelGuid = to.Get<Guid>("ModelGuid");
            if (!ModelDictionary.ContainsKey(modelGuid))
            {
                ModelDictionary[modelGuid] = model;
            }

            if (_isSending && SendWorker.IsBusy)
            {
                if (IsConnected)
                    SendCache.Enqueue(to);
                return;
            }

            if (_myOid.HasValue)
            {
                to.Add("MyOid", _myOid);
            }

            //SendTimer.Start();
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
