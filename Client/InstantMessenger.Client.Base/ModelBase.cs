using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using InstantMessenger.Client.Base.Properties;
using InstantMessenger.Common;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.Base
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event EventHandler<TransportObject> BeforeProcessResponse;
        internal event EventHandler<string> ErrorReceived;
        public event EventHandler<TransportObject> AfterDataReceived;

        public bool Success;
        private readonly Guid _modelGuid;
        
        protected ModelBase()
        {
            Success = false;
            _modelGuid = Guid.NewGuid();
            Client.Disconnected += ClientOnDisconnected;
        }

        protected virtual void ClientOnDisconnected(object sender, EventArgs eventArgs)
        {
            if (ErrorReceived != null)
                ErrorReceived(this, "Server is unavailable.");
        }

        public virtual void OKAction()
        {
            var to = new TransportObject();
            CreateRequest(to);
            SendRequest(to);
        }

        internal virtual void CustomRequest(Action<TransportObject> action)
        {
            var to = new TransportObject();
            action(to);
            SendRequest(to);
        }

        public void GetInitData()
        {
            var to = new TransportObject();
            CreateInitRequest(to);
            SendRequest(to);
        }

        private void SendRequest(TransportObject to)
        {
            to.Add("ModelGuid", _modelGuid);
            Client.SendRequest(to, this);
        }

        protected abstract void CreateRequest(TransportObject to);

        protected virtual void CreateInitRequest(TransportObject to)
        {
            
        }

        internal void GetResponse(TransportObject to)
        {
            if (to.Type == Protocol.MessageType.IM_OK)
            {
                Success = true;
                if (BeforeProcessResponse != null)
                    BeforeProcessResponse(this, to); 
                ProcessResponse(to);
            }
            else
            {
                Success = false;
                var error = to.Get<string>("Error");
                if (ErrorReceived != null)
                    ErrorReceived(this, error);
            }

            if (AfterDataReceived != null)
                AfterDataReceived(this, to);
        }

        protected virtual void ProcessResponse(TransportObject to)
        {
            
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
