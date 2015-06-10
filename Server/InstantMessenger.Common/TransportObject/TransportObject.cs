using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using InstantMessenger.Common.Flats;
using ProtoBuf;

namespace InstantMessenger.Common.TransportObject
{
    [ProtoContract]
    public class TransportObject
    {
        [ProtoIgnore]
        private static readonly IDictionary<int, Type> Types = new Dictionary<int, Type>
        {
            {1, typeof (UserFlat)},
            {2, typeof (string)},
            {3, typeof (long)},
            {4, typeof (byte[])},
            {5, typeof (int)},
            {6, typeof (RequestFlat)},
            {30, typeof(List<>)},
        };

        #region Proto members

        [ProtoMember(1, IsRequired = true)]
        private readonly IDictionary<string, object> _dict;

        [ProtoMember(2, IsRequired = true)]
        private Protocol.MessageType? _type;
        public Protocol.MessageType Type
        {
            get
            {
                if (!_type.HasValue)
                    throw new InvalidOperationException("Transport object doesn't contain MessageType.");
                return _type.Value;
            }
            set
            {
                if (_type.HasValue)
                    throw new InvalidOperationException("Transport object already contains MessageType.");
                _type = value;
            }
        }

        #endregion

        #region Constructors
        public TransportObject()
        {
            _dict = new Dictionary<string, object>();
        }

        public TransportObject(Protocol.MessageType type)
            :this()
        {
            _type = type;
        }

        #endregion

        #region Public methods

        public void Add(string key, object value)
        {
            _dict.Add(key, value);
        }

        public T Get<T>(string key)
        {
            if (!_dict.ContainsKey(key))
                return default(T);

            return (T)_dict[key];
        }

        public void Serialize(SslStream stream)
        {
            if (!Validate())
            {
                throw new InvalidOperationException("Transport object doesn't contain all necessary fields.");
                
            }

            Serializer.SerializeWithLengthPrefix(stream, Type, PrefixStyle.Base128);
            Serializer.SerializeWithLengthPrefix(stream, _dict.Count, PrefixStyle.Base128);

            foreach (var item in _dict)
            {
                Serializer.SerializeWithLengthPrefix(stream, item.Key, PrefixStyle.Base128);
                Serializer.NonGeneric.SerializeWithLengthPrefix(stream, item.Value, PrefixStyle.Base128, GetNumByType(item.Value.GetType()));
            }
           
            stream.Flush();
        }

        public static TransportObject Deserialize(SslStream stream)
        {
            Protocol.MessageType msgType;
            try
            {
                msgType = Serializer.DeserializeWithLengthPrefix<Protocol.MessageType>(stream, PrefixStyle.Base128);
            }
            catch (ObjectDisposedException)
            {
                return null;
            }

            var itemsCount = Serializer.DeserializeWithLengthPrefix<int>(stream, PrefixStyle.Base128);
            var dto = new TransportObject(msgType);
         
            while (itemsCount-- > 0)
            {
                var key = Serializer.DeserializeWithLengthPrefix<string>(stream, PrefixStyle.Base128);
                object value;
                var result = Serializer.NonGeneric.TryDeserializeWithLengthPrefix(stream, PrefixStyle.Base128,
                    GetTypeByNum, out value);

                if (result)
                {
                    dto.Add(key, value);
                }
                else
                {
                    return null;
                }
            }

            return dto;
        }

        #endregion

        private bool Validate()
        {
            if (!_type.HasValue)
                throw new SerializationException("Transport object must contain MessageType.");

            switch (Type)
            {
                case Protocol.MessageType.IM_Login:
                    return _dict.ContainsKey("Username") &&
                           _dict.ContainsKey("Password");
                case Protocol.MessageType.IM_Register:
                    return _dict.ContainsKey("Username") &&
                           _dict.ContainsKey("Password");
            }

            return true;
        }

        #region Other methods

        private int GetNumByType(Type T)
        {
            if (T.IsConstructedGenericType)
            {
                if (Types.Values.All(x => x.Name != T.Name))
                {
                    throw new SerializationException("Generic type {0} is not listed in serialization dictionary.", T);
                }

                if (T.GenericTypeArguments.Count() != 1)
                {
                    throw new SerializationException("Can serialize only types with one generic parameter.");
                }
                
                if (Types.Values.All(x => x != T.GenericTypeArguments[0]))
                {
                    throw new SerializationException("Generic type argument {0} is not listed in serialization dictionary.", T);
                }

                var typeNum = Types.Single(t => t.Value.Name == T.Name).Key;
                var genericNum = Types.Single(t => t.Value == T.GenericTypeArguments[0]).Key;

                return typeNum + genericNum;
            }

            if (!Types.Values.Contains(T))
            {
                throw new SerializationException("Type {0} is not listed in serialization dictionary.", T);
            }

            return Types.Single(t => t.Value == T).Key;
        }

        private static Type GetTypeByNum(int index)
        {
            if (index > 30)
            {
                var type = Types[index - (index % 30)];
                var genericArgument = Types[index%30];
                var genericType = type.MakeGenericType(genericArgument);

                return genericType;
            }

            if (!Types.ContainsKey(index))
            {
                throw new SerializationException("Type for given index is not listed in serialization dictionary.");
            }

            return Types[index];
        }

        #endregion
    }
}
