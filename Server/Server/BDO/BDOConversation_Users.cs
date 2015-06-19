using InstantMessenger.Core.Base.Implementation;

namespace InstantMessenger.DataModel.BDO
{
    public class Conversation_UsersMap : ClassMapBase<BDOConversation_Users>
    {
        public Conversation_UsersMap()
            : base("T05", "T05_T01_x_T04")
        {
            References(x => x.Conversation).Column("T04ConversationOID").Not.Nullable();
            References(x => x.User).Column("T01UserOID").Not.Nullable();
        }
    }
    public class BDOConversation_Users : BDOBase
    {
        public virtual BDOConversation Conversation { get; set; }
        public virtual BDOUser User { get; set; }
    }
}
