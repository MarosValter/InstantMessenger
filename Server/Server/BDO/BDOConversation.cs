using System.Collections.Generic;
using System.Linq;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.Core.UOW;

namespace InstantMessenger.DataModel.BDO
{
    public class ConversationMap : ClassMapBase<BDOConversation>
    {
        public ConversationMap()
            : base("T04", "T04_CONVERSATIONS")
        {
            Map(x => x.Name).Column("T04Name").Nullable();
            Map(x => x.IsDialog).Column("T04IsDialog").Not.Nullable();

            HasMany(x => x.Messages).KeyColumn("T04ConversationOID").Inverse().BatchSize(20);
            HasMany(x => x.ConversationUsers).KeyColumn("T04ConversationOID").Inverse().Cascade.AllDeleteOrphan();
        }
    }
    public class BDOConversation : BDOBase
    {
        public virtual string Name { get; set; }
        public virtual bool IsDialog { get; set; }

        public virtual IList<BDOMessage> Messages { get; set; }

        public virtual IList<BDOConversation_Users> ConversationUsers { get; set; }

        /// <summary>
        /// Calling method must me decorated with <see cref="UnitOfWorkAttribute"/>
        /// </summary>
        public virtual IList<BDOUser> GetUsers
        {
            get { return ConversationUsers.Select(x => x.User).ToList(); }
        }
    }
}
