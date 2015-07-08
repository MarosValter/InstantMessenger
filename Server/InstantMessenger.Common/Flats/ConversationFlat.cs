using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstantMessenger.Common.OldTO;
using ProtoBuf;

namespace InstantMessenger.Common.Flats
{
    [ProtoContract]
    public class ConversationFlat : SerializableBase
    {
        [ProtoMember(11)]
        public long OID { get; set; }

        [ProtoMember(12)]
        public bool IsDialog { get; set; }

        [ProtoMember(13)]
        public bool? IsUserOnline { get; set; }

        [ProtoMember(14)]
        public string Name { get; set; }
    }
}
