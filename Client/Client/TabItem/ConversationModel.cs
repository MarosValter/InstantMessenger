using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using InstantMessenger.Client.Base;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.TabItem
{
    public class ConversationModel : ModelBase
    {
        #region Attributes

        private ObservableCollection<MessageFlat> _messages;
        public ObservableCollection<MessageFlat> Messages
        {
            get { return _messages; }
            set
            {
                if (_messages == value)
                    return;
                _messages = value;
                OnPropertyChanged();
            }
        }
        public string Text { get; set; }

        //public int MessageCount { get; set; }

        private readonly long _conversationOid;

        //private bool AllMessages
        //{
        //    get
        //    {
        //        return !Messages.Any() || Messages.Min(x => x.Order) == 0;
        //    }
        //}

        private long? FirstMessageOrder
        {
            get
            {
                return Messages.Any()
                        ? Messages.Min(x => x.Order)
                        : (long?)null;
            }
        }

        private long? LastMessageOrder
        {
            get
            {
                return Messages.Any()
                        ? Messages.Max(x => x.Order)
                        : (long?) null;
            }
        }

        private readonly Timer _refreshTimer;
        private const long RefreshTime = 1000;

        #endregion

        #region Constructor

        public ConversationModel(long conversationOid)
        {
            _conversationOid = conversationOid;
            Messages = new ObservableCollection<MessageFlat>();
            //MessageCount = Constants.MessageBatchSize;

            _refreshTimer = new Timer(RefreshTime);
            _refreshTimer.AutoReset = true;
            _refreshTimer.Elapsed += (sender, args) => GetInitData();

            GetInitData();
        }

        #endregion

        #region Event handlers

        protected override void ClientOnDisconnected(object sender, EventArgs eventArgs)
        {
            _refreshTimer.Stop();
            base.ClientOnDisconnected(sender, eventArgs);
        }

        #endregion

        #region Public methods

        #endregion

        #region Overrides

        public void LoadNextMessages(TransportObject to)
        {
            if (!FirstMessageOrder.HasValue || FirstMessageOrder.Value == 0)
            {
                to.Type = Protocol.MessageType.IM_DONT_SEND;
                return;
            }

            to.Type = Protocol.MessageType.IM_GetOldMessages;
            to.Add("ConversationOid", _conversationOid);
            to.Add("FirstMessageOrder", FirstMessageOrder.Value);
        }

        protected override void CreateRequest(TransportObject to)
        {
            to.Type = Protocol.MessageType.IM_Send;
            to.Add("ConversationOid", _conversationOid);
            to.Add("Text", Text);
        }

        protected override void CreateInitRequest(TransportObject to)
        {
            to.Type = Protocol.MessageType.IM_InitConversation;
            to.Add("ConversationOid", _conversationOid);
            to.Add("LastMessageOrder", LastMessageOrder);
        }

        protected override void ProcessResponse(TransportObject to)
        {
            var messages = to.Get<List<MessageFlat>>("Messages");
            if (!messages.Any())
                return;

            var old = to.Get<bool?>("Old");

            if (!Messages.Any())
            {
                Messages = new ObservableCollection<MessageFlat>(messages);
            }
            else if (old.HasValue)
            {
                Messages.InsertRange(messages);
            }
            else
            {
                Messages.AddRange(messages);
            }

            if (!_refreshTimer.Enabled)
            {
                _refreshTimer.Start();
            }
        }

        #endregion
    }
}
