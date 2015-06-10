using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using InstantMessenger.Client.Base.Annotations;
using InstantMessenger.Common;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.Base
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event EventHandler<TransportObject> DataReceived;

        public bool Success;
        public string Error;
        public Guid ModelGuid;
        
        protected ModelBase()
        {
            Success = false;
            ModelGuid = new Guid();
        }

        public virtual void OKACtion()
        {
            var to = new TransportObject();
            CreateRequest(to);
            SendRequest(to);
        }

        internal virtual void CustomRequest(Action<TransportObject> action)
        {
            var to = new TransportObject();
            action(to);
            if (to == null)
                return;
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
                ProcessResponse(to);
            }
            else
            {
                Success = false;
                Error = to.Get<string>("Error");
            }

            var handler = DataReceived;
            if (handler != null)
                handler(this, to);          
        }

        protected virtual void ProcessResponse(TransportObject to)
        {
            
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
