using System;
using ProtoBuf;

namespace InstantMessenger.Common.Flats
{
    [ProtoContract]
    public class MessageFlat
    {
        [ProtoMember(15)]
        public long OID { get; set; }
        [ProtoMember(16)]
        public long UserOID { get; set; }
        [ProtoMember(17)]
        public string Text { get; set; }
        [ProtoMember(18)]
        public DateTime DateTime { get; set; }
        [ProtoMember(19)]
        public long Order { get; set; }
    }
}
