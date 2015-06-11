using System;
using System.ComponentModel;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Timers;
using InstantMessenger.Common;
using InstantMessenger.Common.TransportObject;
using Timer = System.Timers.Timer;

namespace InstantMessenger.Client.Base
{
    public static class Client
    {
        #region Events

        public static event EventHandler Connected;
        public static event EventHandler Disconnected;
        public static event EventHandler<int> Reconnecting;
        //public static event EventHandler<TransportObject> DataReceived;

        #endregion

        #region Constants

        private const int ReconnectTimeout = 3000;
        private const int ReconnectAttempts = 3;
        private const int Timeout = 3000000;

        #endregion

        #region Attributes

        private static TcpClient _client;
        private static string _host;
        private static int _port;
        private static SslStream _stream;
        private static ModelBase _requestingModel;

        private static readonly BackgroundWorker _worker;
        private static readonly Timer _timer;

        private static long? _myOid;

        public static bool IsConnected
        {
            get
            {
                return _client != null &&
                       _client.Client != null &&
                       _client.Client.IsBound &&
                       _stream != null &&
                       _stream.IsAuthenticated;
            }
        }

        private static readonly ReaderWriterLockSlim SlimLock= new ReaderWriterLockSlim();
        private static bool _isWorking;
        public static bool IsWorking
        {
            get
            {
                SlimLock.EnterReadLock();
                var result = _isWorking;
                SlimLock.ExitReadLock();
                return result;
            }
            set
            {
                SlimLock.TryEnterWriteLock(50);
                _isWorking = value;
                SlimLock.ExitWriteLock();
            }
        }

        #endregion

        #region Constructor

        static Client()
        {
            _client = new TcpClient();

            _timer = new Timer(Timeout);
            _timer.Elapsed += TimerTimeout;

            _worker = new BackgroundWorker();
            _worker.DoWork += DoWork;
            _worker.RunWorkerCompleted += WorkerCompleted;
        }

        #endregion

        #region Connection methods

        public static void Connect(bool showMessage)
        {
            if (IsConnected)
                return;

            try
            {
                _client = new TcpClient(_host, _port);
                _stream = new SslStream(_client.GetStream(), false, UserCertificateValidationCallback);
                _stream.AuthenticateAsClient("localhost");


                if (Connected != null)
                    Connected(null, null);
            }
            catch (Exception)
            {
                if (showMessage)
                {
                    //MessageBox.Show(Properties.Messages.E001, Properties.Messages.Error, MessageBoxButton.OK,
                    //                MessageBoxImage.Error);
                }
                else
                {
                    throw;
                }
            }
        }

        private static void Reconnect()
        {
            var attempt = 1;
            if (IsConnected)
                Disconnect(false);

            while (!IsConnected && attempt <= ReconnectAttempts)
            {
                if (Reconnecting != null)
                    Reconnecting(null, attempt);

                try
                {
                    Connect(false);
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
            if (_client != null)
            {
                _client.Client.Close();
            }
            if (_stream != null)
            {
                _stream.Close();
            }

            if (fire && Disconnected != null)
                Disconnected(null, null);
        }

        #endregion

        public static void Init(string host, int port)
        {
            _host = host;
            _port = port;
        }
        private static void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _timer.Stop();
            IsWorking = false;

            TransportObject result;
            if (e.Cancelled || e.Error != null)
            {
                result = new TransportObject(Protocol.MessageType.IM_ERROR);
                result.Add("Error", e.Error.Message);
            }
            else
            {
                result = e.Result as TransportObject;
                if (result != null && !_myOid.HasValue)
                {
                    _myOid = result.Get<long?>("MyOid");
                }
            }

            _requestingModel.GetResponse(result);
        }

        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            var to = e.Argument as TransportObject;
            try
            {
                to.Serialize(_stream);
            }
            catch (Exception)
            {
                Reconnect();
            }
            

            var result = TransportObject.Deserialize(_stream);
            e.Result = result;
        }

        private static void TimerTimeout(object sender, ElapsedEventArgs e)
        {
            Reconnect();
        }
      

        public static void SendRequest(TransportObject to, ModelBase model)
        {
            if (_host.IsNullOrEmpty())
            {
                throw new Exception("Host is not set. You must set host and port before sending request");
            }

            if (!IsConnected)
            {
                Connect(false);
                //throw new Exception("Not connected to server. Connect before sending request.");
            }

            if (IsWorking)
            {
                //TODO
            }

            if (_myOid.HasValue)
            {
                to.Add("MyOid", _myOid);
            }
            _requestingModel = model;
            _timer.Start();
            _worker.RunWorkerAsync(to);
        }

        #region Other methods

        private static bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion
    }   
}
