using System;
using System.Collections.Generic;
using System.Linq;
using InstantMessenger.Core.Base.Implementation;

namespace InstantMessenger.DataModel.BDO
{
    public class UserMap : ClassMapBase<BDOUser>
    {
        public UserMap()
            :base("T01", "T01_USERS")
        {
            Map(x => x.Username).Column("T01Username").Unique().Not.Nullable();
            Map(x => x.PasswordHash).Column("T01PasswordHash").Not.Nullable();
            Map(x => x.LastLogin).Column("T01LastLogin").Update().Nullable();
            Map(x => x.Edited).Column("T01Edited").Nullable();
            Map(x => x.IsOnline).Column("T01IsOnline").Update();
            Map(x => x.Email).Column("T01Email").Nullable();

            HasMany(x => x.Messages).KeyColumn("T01FromUserOID").Inverse().BatchSize(50);
            HasMany(x => x.ConversationUsers).KeyColumn("T01UserOID").Inverse();
        }
    }

    public class BDOUser : BDOBase
    {
        public virtual string Username { get; set; }

        public virtual byte[] PasswordHash { get; set; }

        public virtual DateTime? LastLogin { get; set; }

        public virtual bool IsOnline { get; set; }

        public virtual DateTime? Edited { get; set; }

        public virtual string Email { get; set; }

        public virtual IList<BDOMessage> Messages { get; set; }

        public virtual IList<BDOConversation_Users> ConversationUsers { get; set; }

        public virtual IList<BDOConversation> GetConversations
        {
            get { return ConversationUsers.Select(x => x.Conversation).ToList(); }
        }
    }
}

