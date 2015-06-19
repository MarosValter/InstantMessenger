using System;
using System.Collections.Generic;
using InstantMessenger.Core.Base;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.DataModel.BDO;

namespace InstantMessenger.DataModel.BRO
{
    public class BROFriendship : BROGeneric<BDOFriendship>
    {
        public bool AlreadyRequested(BDOUser sender, BDOUser recipient)
        {
            return Repository.Count(x => x.User == sender && x.Friend == recipient) > 0;
        }

        public bool AlreadyAccepted(BDOUser sender, BDOUser recipient)
        {
            return Repository.Count(x => x.Friend == recipient && x.User == sender) > 0;
        }

        public bool AcceptRequest(BDOFriendship friendship)
        {
            friendship.IsAccepted = true;
            var friendship2 = new BDOFriendship
            {
                User = friendship.Friend,
                Friend = friendship.User,
                IsAccepted = true,
            };
            try
            {
                Create(friendship2);
                Update(friendship);

                return true;
            }
            catch (Exception)
            {
                return false;
            }                 
        }

        public IList<BDOFriendship> GetUserRequests(long userOid)
        {
            return Repository.FindAll(x => x.Friend.OID == userOid && !x.IsAccepted);
        }

        public int GetUserRequestCount(long userOid)
        {
            return Repository.Count(x => x.Friend.OID == userOid && !x.IsAccepted);
        }

        public BDOFriendship GetBySendeRecipient(BDOUser sender, BDOUser recipient)
        {
            return Repository.FindOne(x => x.User == recipient && x.Friend == sender);
        }

        public IList<BDOFriendship> GetFriendsByUser(long userOid)
        {
            return Repository.FindAll(x => x.User.OID == userOid && x.IsAccepted);
        }
    }
}
