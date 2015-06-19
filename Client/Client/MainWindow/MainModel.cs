using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.MainWindow
{
    public class MainModel : ModelBase
    {
        #region Attributes

        public event EventHandler RequestCountChanged;

        private readonly Timer _refreshTimer;
        private const long RefreshTime = 1000;

        private ObservableCollection<ConversationFlat> _onlineFriends;
        public ObservableCollection<ConversationFlat> OnlineFriends
        {
            get {return _onlineFriends;}
            set
            {
                if (_onlineFriends == value)
                    return;
                _onlineFriends = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ConversationFlat> _offlineFriends;
        public ObservableCollection<ConversationFlat> OfflineFriends
        {
            get { return _offlineFriends; }
            set
            {
                if (_offlineFriends == value)
                    return;
                _offlineFriends = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ConversationFlat> _groupChats;
        public ObservableCollection<ConversationFlat> GroupChats
        {
            get { return _groupChats; }
            set
            {
                if (_groupChats == value)
                    return;
                _groupChats = value;
                OnPropertyChanged();
            }
        }

        private int _requestCount;
        public int RequestCount
        {
            get { return _requestCount; }
            set
            {
                if (_requestCount == value)
                    return;
                _requestCount = value;
                OnPropertyChanged("RequestCountGui");
                if (RequestCountChanged != null)
                    RequestCountChanged(this, null);
            }
        }

        public string RequestCountGui
        {
            get
            {
                return RequestCount > 0
                    ? string.Format("({0})", _requestCount)
                    : string.Empty;
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            private set
            {
                if (_username == value)
                    return;
                _username = value;
                OnPropertyChanged();
                }
        }

        #endregion

        #region Constructor

        public MainModel()
        {
            OnlineFriends = new ObservableCollection<ConversationFlat>();
            OfflineFriends = new ObservableCollection<ConversationFlat>();
            GroupChats = new ObservableCollection<ConversationFlat>();

            _refreshTimer = new Timer(RefreshTime);
            _refreshTimer.AutoReset = true;
            _refreshTimer.Elapsed += (sender, args) => GetInitData();
        }

        #endregion

        #region Overrides

        // Must be empty, otherwise Enter key press would call CreateRequest and then throw exception
        // that TransportObject doesn't have Type.
        public override void OKAction()
        {
        }

        protected override void CreateRequest(TransportObject to)
        {
        }

        protected override void CreateInitRequest(TransportObject to)
        {
            to.Type = Protocol.MessageType.IM_InitMain;
        }

        protected override void ProcessResponse(TransportObject to)
        {
            var requestCount = to.Get<int>("RequestCount");
            var flat = to.Get<UserFlat>("UserFlat");
            var conversations = to.Get<List<ConversationFlat>>("Conversations");

            OnlineFriends = new ObservableCollection<ConversationFlat>(conversations.Where(x => x.IsDialog && x.IsUserOnline.Value));
            OfflineFriends = new ObservableCollection<ConversationFlat>(conversations.Where(x => x.IsDialog && !x.IsUserOnline.Value));
            GroupChats = new ObservableCollection<ConversationFlat>(conversations.Where(x => !x.IsDialog));

            RequestCount = requestCount;
            Username = flat.Username;

            if (!_refreshTimer.Enabled)
            {
                _refreshTimer.Start();
            }
        }

        #endregion

        #region Event handlers

        protected override void ClientOnDisconnected(object sender, EventArgs eventArgs)
        {
            _refreshTimer.Stop();
            base.ClientOnDisconnected(sender, eventArgs);
        }

        #endregion
    }
}
