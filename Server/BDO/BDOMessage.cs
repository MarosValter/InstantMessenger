
using InstantMessenger.Core.Base.Implementation;

namespace InstantMessenger.DataModel.BDO
{
    public class MssageMap : ClassMapBase<BDOMessage>
    {
        public MssageMap()
            :base("T03", "T03_MESSAGES")
        {
            Map(x => x.Text).Column("T03Text");

            References(x => x.From).Column("T01FromUserOID");
            References(x => x.To).Column("T04ConversationOID");
        }
    }
    public class BDOMessage : BDOBase
    {
        public virtual BDOUser From { get; set; }

        public virtual BDOConversation To { get; set; }

        public virtual string Text { get; set; }
    }
}

