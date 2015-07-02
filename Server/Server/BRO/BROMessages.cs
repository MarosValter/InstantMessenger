using System.Collections.Generic;
using System.Linq;
using InstantMessenger.Common;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.DataModel.BDO;

namespace InstantMessenger.DataModel.BRO
{
    public class BROMessages : BROGeneric<BDOMessage>
    {
        public IList<BDOMessage> GetOldByConversationOid(long conversationOid, long order)
        {
            return Repository.CreateQuery().Where(x => x.To.OID == conversationOid &&
                                                       x.Order < order)
                                           .OrderByDescending(x => x.Order)
                                           .ThenByDescending(x => x.Created)
                                           .Take(Constants.MessageBatchSize)
                                           .ToList();
        }

        public IList<BDOMessage> GetNewByConversationOid(long conversationOid, long? order)
        {
            IQueryable<BDOMessage> q = Repository.CreateQuery()
                                                 .Where(x => x.To.OID == conversationOid)
                                                 ;
            q = order.HasValue 
                ? q.Where(x => x.Order > order.Value)
                   .OrderBy(x => x.Order)
                   .ThenBy(x => x.Created)
                : q.OrderByDescending(x => x.Order)
                   .ThenByDescending(x => x.Created)
                   .Take(Constants.MessageBatchSize);

            return q.ToList();
        }

        public int GetCountByConversationOid(long conversationOid)
        {
            return Repository.Count(x => x.To.OID == conversationOid);
        }

        public long GetMaxOrderByConversation(long conversationOid)
        {
            return Repository.CreateQuery().Where(x => x.To.OID == conversationOid).Max(x => x.Order);
        }
    }
}
