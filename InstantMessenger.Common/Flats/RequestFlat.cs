using System;
using InstantMessenger.Common.TransportObject;
using ProtoBuf;

namespace InstantMessenger.Common.Flats
{
    [ProtoContract]
    public class RequestFlat : SerializableBase
    {
        [ProtoMember(8)]
        public long UserOID { get; set; }
        [ProtoMember(9)]
        public string UserUsername { get; set; }
        [ProtoMember(10)]
        public DateTime Created { get; set; }
        [ProtoIgnore]
        public bool IsSelected { get; set; }
    }
}
