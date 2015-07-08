using System;
using System.Collections.Generic;
using AutoMapper;
using InstantMessenger.Common;
using InstantMessenger.Common.DM;
using InstantMessenger.Common.Flats;
using InstantMessenger.Communication;
using InstantMessenger.Core;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.Core.UOW;
using InstantMessenger.DataModel.BDO;
using InstantMessenger.DataModel.BFO;
using InstantMessenger.DataModel.BRO;

namespace InstantMessenger.DataModel.DataManagers
{
    public class FriendshipsDataManager : IFriendshipsDataManager
    {
        public void RegisterMapping(AutoMapper.IConfiguration mapper)
        {
            mapper.CreateMap<BDOFriendship, RequestFlat>()
                  .ForMember(dest => dest.UserOID, opt => opt.MapFrom(src => src.User.OID))
                  .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));
        }

        [UnitOfWork]
        public virtual TransportObject GetRequests(TransportObject to)
        {
            var myOid = to.Get<long>("MyOid");

            var requests = ObjectFactory.GetInstance<BROFriendship>().GetUserRequests(myOid);
            var requestFlats = AutoMapper.Mapper.Map<IList<BDOFriendship>, IList<RequestFlat>>(requests);

            var dto = new TransportObject(/*Protocol.MessageType.IM_OK*/);
            dto.Add("Requests", requestFlats);

            return dto;
        }

        public TransportObject SendRequest(TransportObject to)
        {
            var senderOid = to.Get<long>("MyOid");
            var recipientOid = to.Get<long>("UserOid");

            var sender = ObjectFactory.GetInstance<BROUsers>().GetByOid(senderOid);
            var recipient = ObjectFactory.GetInstance<BROUsers>().GetByOid(recipientOid);

            var dto = new TransportObject();

            if (recipient == null)
            {
                //dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "User doesn't exist.");
                return dto;
            }

            if (ObjectFactory.GetInstance<BROFriendship>().AlreadyRequested(sender, recipient))
            {
                var errorMsg = ObjectFactory.GetInstance<BROFriendship>().AlreadyAccepted(sender, recipient)
                                   ? "Already friends."
                                   : "User already asked for friendship.";
                dto.Add("Error", errorMsg);
                //dto.Type = Protocol.MessageType.IM_ERROR;
                
                return dto;
            }

            var bdo = new BDOFriendship {User = sender, Friend = recipient, IsAccepted = false};
            ObjectFactory.GetInstance<BROFriendship>().Create(bdo);

            //dto.Type = Protocol.MessageType.IM_OK;
            return dto;
        }

        [UnitOfWork]
        public virtual TransportObject AcceptRequest(TransportObject to)
        {
            var userOid = to.Get<long>("UserOid");
            var myOid = to.Get<long>("MyOid");

            var sender = ObjectFactory.GetInstance<BROUsers>().GetByOid(myOid);
            var recipient = ObjectFactory.GetInstance<BROUsers>().GetByOid(userOid);

            var dto = new TransportObject();

            if (ObjectFactory.GetInstance<BROFriendship>().AlreadyAccepted(sender, recipient))
            {
                //dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "Request already accepted.");
                return dto;
            }

            var request = ObjectFactory.GetInstance<BROFriendship>().GetBySendeRecipient(sender, recipient);
            var accepted = ObjectFactory.GetInstance<BROFriendship>().AcceptRequest(request);

            var dialogCreated = ObjectFactory.GetInstance<BFOConversations>().CreateDialog(userOid, myOid);

            if (accepted && dialogCreated)
            {
                //dto.Type = Protocol.MessageType.IM_OK;
            }
            else
            {
                var msg = accepted
                    ? "Unable to create conversation after accepting "
                    : "Unable to accept request.";
                //dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", msg);
                return dto;
            }

            return GetRequests(to);
        }

        public TransportObject DeleteRequest(TransportObject to)
        {
            throw new NotImplementedException();
        }

        [UnitOfWork]
        public virtual TransportObject FindUsers(TransportObject to)
        {
            var username = to.Get<string>("Username");
            var users = ObjectFactory.GetInstance<BROUsers>().GetUsersStartingNameWith(username);
            var flats = Mapper.Map<IList<BDOUser>, IList<UserFlat>>(users);

            var dto = new TransportObject(/*Protocol.MessageType.IM_OK*/);
            dto.Add("Users", flats);

            return dto;
        }
    }
}

