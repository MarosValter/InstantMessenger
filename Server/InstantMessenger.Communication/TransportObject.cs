using System.Collections.Generic;
using System.Runtime.Serialization;


namespace InstantMessenger.Communication
{
    [DataContract]
    public class TransportObject
    {
        [DataMember]
        private Dictionary<string, object> _items = new Dictionary<string, object>();

        private readonly object _addLocker = new object();

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }
        
        public T Get<T>(string key)
        {
            if (!_items.ContainsKey(key))
                return default(T);

            return (T)_items[key];
        }

        public void Add(string key, object value)
        {
            Add<object>(key, value);
        }

        public void Add<T>(string key, T value)
        {
            lock (_addLocker)
            {
                _items.Add(key, value);
            }
        }
    }
}
