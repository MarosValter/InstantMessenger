using System.Collections.Generic;
using InstantMessenger.Core.Base.Implementation;

namespace InstantMessenger.DataModel.BDO
{
    public class ConversationMap : ClassMapBase<BDOConversation>
    {
        public ConversationMap()
            : base("T04", "T04_CONVERSATIONS")
        {
            Map(x => x.Name).Column("T04Name").Nullable();

            HasMany(x => x.Messages).KeyColumn("T04ConversationOID").Inverse().BatchSize(20);
        }
    }
    public class BDOConversation : BDOBase
    {
        public virtual string Name { get; set; }

        public virtual IList<BDOMessage> Messages { get; set; } 
    }
}
