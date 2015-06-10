using InstantMessenger.Core;
using InstantMessenger.Core.Base.Implementation;

namespace InstantMessenger.DataModel.BDO
{
    public class FriendshipMap : ClassMapBase<BDOFriendship>
    {
        public FriendshipMap()
            : base("T02", "T02_FRIENDSHIPS")
        {
            Map(x => x.IsAccepted).Column("T02IsAccepted").Not.Nullable();

            References(x => x.User).Column("T01UserOID").Not.Nullable();
            References(x => x.Friend).Column("T01FriendOID").Not.Nullable();
        }
    }
    public class BDOFriendship : BDOBase
    {
        public virtual BDOUser User { get; set; }

        public virtual BDOUser Friend { get; set; }

        public virtual bool IsAccepted { get; set; }
    }
}

