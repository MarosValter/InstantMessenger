using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;
using InstantMessenger.Common.TransportObject;
using InstantMessenger.Core;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.Core.UOW;
using InstantMessenger.DataModel.BDO;
using InstantMessenger.DataModel.BRO;

namespace InstantMessenger.DataModel.DataManagers
{
    public class ConversationsDataManager : DataManagerBase
    {
        public override void RegisterMapping(IConfiguration mapper)
        {
            mapper.CreateMap<BDOMessage, MessageFlat>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UserOID, opt => opt.MapFrom(src => src.From.OID));
        }

        [UnitOfWork]
        public virtual TransportObject Init(TransportObject to)
        {
            var conversationOid = to.Get<long>("ConversationOid");
            var lastMessageOrder = to.Get<long?>("LastMessageOrder");

            var messages = ObjectFactory.GetInstance<BROMessages>().GetNewByConversationOid(conversationOid, lastMessageOrder);

            var dto = new TransportObject();

            if (messages.Any())
            {
                dto.Type = Protocol.MessageType.IM_OK; 
                var flats = Mapper.Map<IList<BDOMessage>, IList<MessageFlat>>(messages);
                dto.Add("Messages", flats);
            }
            else
            {
                dto.Type = Protocol.MessageType.IM_DONT_SEND;
            }

            return dto;
        }

        [UnitOfWork]
        public virtual TransportObject GetOldMessages(TransportObject to)
        {
            var conversationOid = to.Get<long>("ConversationOid");
            var firstMessageOrder = to.Get<long>("FirstMessageOrder");

            var messages = ObjectFactory.GetInstance<BROMessages>().GetOldByConversationOid(conversationOid, firstMessageOrder);
            var flats = Mapper.Map<IList<BDOMessage>, IList<MessageFlat>>(messages);

            var dto = new TransportObject(Protocol.MessageType.IM_OK);
            dto.Add("Messages", flats);
            dto.Add("Old", true);

            return dto;
        }

        [UnitOfWork]
        public virtual TransportObject SendMessage(TransportObject to)
        {
            var conversationOid = to.Get<long>("ConversationOid");
            var myOid = to.Get<long>("MyOid");
            var text = to.Get<string>("Text");

            var myBdo = ObjectFactory.GetInstance<BROUsers>().GetByOid(myOid);
            long newOrder = 0;
            var conversationBdo = ObjectFactory.GetInstance<BROConversations>().GetByOid(conversationOid);
            if (conversationBdo.Messages.Any())
            {
                newOrder = ObjectFactory.GetInstance<BROMessages>().GetMaxOrderByConversation(conversationOid) + 1;
            }


            var bdo = new BDOMessage
            {
                From = myBdo,
                To = conversationBdo,
                Text = text,
                Order = newOrder,
            };

            ObjectFactory.GetInstance<BROMessages>().Create(bdo);

            var dto = new TransportObject(Protocol.MessageType.IM_OK);
            return dto;
        }
    }
}
