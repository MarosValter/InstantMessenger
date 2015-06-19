
using InstantMessenger.Core.Base.Implementation;

namespace InstantMessenger.DataModel.BDO
{
    public class MssageMap : ClassMapBase<BDOMessage>
    {
        public MssageMap()
            :base("T03", "T03_MESSAGES")
        {
            Map(x => x.Text).Column("T03Text").Not.Nullable();
            Map(x => x.Order).Column("T03Order").Not.Nullable();

            References(x => x.From).Column("T01FromUserOID").Not.Nullable();
            References(x => x.To).Column("T04ConversationOID").Not.Nullable();
        }
    }
    public class BDOMessage : BDOBase
    {
        public virtual BDOUser From { get; set; }

        public virtual BDOConversation To { get; set; }

        public virtual string Text { get; set; }

        public virtual long Order { get; set; }
    }
}

