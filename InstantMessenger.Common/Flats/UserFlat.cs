using InstantMessenger.Common.TransportObject;
using ProtoBuf;

namespace InstantMessenger.Common.Flats
{
    [ProtoContract]
    public class UserFlat : SerializableBase
    {
        [ProtoMember(5)]
        public long OID { get; set; }
        [ProtoMember(6)]
        public string Username { get; set; }
        [ProtoMember(7)]
        public bool IsOnline { get; set; }
    }
}
