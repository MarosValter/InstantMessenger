using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstantMessenger.Core;
using InstantMessenger.Core.Base;
using InstantMessenger.DataModel.BDO;
using InstantMessenger.DataModel.BRO;

namespace InstantMessenger.DataModel.BFO
{
    public class BFOConversations : IBFO
    {
        public bool CreateDialog(long userOid1, long userOid2)
        {
            var user1 = ObjectFactory.GetInstance<BROUsers>().GetByOid(userOid1);
            var user2 = ObjectFactory.GetInstance<BROUsers>().GetByOid(userOid2);

            var bdoConversation = new BDOConversation();
            bdoConversation.IsDialog = true;
            ObjectFactory.GetInstance<BROConversations>().Create(bdoConversation);

            var connection1 = new BDOConversation_Users();
            connection1.User = user1;
            connection1.Conversation = bdoConversation;

            var connection2 = new BDOConversation_Users();
            connection2.User = user2;
            connection2.Conversation = bdoConversation;

            ObjectFactory.GetInstance<BROConversation_Users>().Create(connection1);
            ObjectFactory.GetInstance<BROConversation_Users>().Create(connection2);

            return true;
        }
    }
}
