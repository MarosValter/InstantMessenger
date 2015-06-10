﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.MainWindow
{
    public class MainModel : ModelBase
    {
        #region Attributes

        private ObservableCollection<UserFlat> _onlineFriends;

        public ObservableCollection<UserFlat> OnlineFriends
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
        private ObservableCollection<UserFlat> _offlineFriends;

        public ObservableCollection<UserFlat> OfflineFriends
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
            }
        }

        public string RequestCountGui { get { return string.Format("({0})", _requestCount); } }

        #endregion

        #region Constructor

        public MainModel()
        {
            OnlineFriends = new ObservableCollection<UserFlat>();
            OfflineFriends = new ObservableCollection<UserFlat>();
        }

        #endregion

        #region Overrides

        protected override void CreateRequest(TransportObject to)
        {
        }

        protected override void CreateInitRequest(TransportObject to)
        {
            to.Type = Protocol.MessageType.IM_FriendsRequests;
        }

        protected override void ProcessResponse(TransportObject to)
        {
            var friends = to.Get<List<UserFlat>>("Friends");
            var requestCount = to.Get<int>("RequestCount");

            OnlineFriends = new ObservableCollection<UserFlat>(friends.Where(x => x.IsOnline));
            OfflineFriends = new ObservableCollection<UserFlat>(friends.Where(x => !x.IsOnline));
            RequestCount = requestCount;
        }

        #endregion
    }
}
