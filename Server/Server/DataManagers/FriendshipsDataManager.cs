using System;
using System.Collections.Generic;
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
    public class FriendshipsDataManager : DataManagerBase
    {
        public override void RegisterMapping(AutoMapper.IConfiguration mapper)
        {
            mapper.CreateMap<BDOFriendship, RequestFlat>();
        }

        public TransportObject GetRequests(TransportObject to)
        {
            var myOid = to.Get<long>("MyOid");

            var requests = ObjectFactory.GetInstance<BROFriendship>().GetUserRequests(myOid);
            var requestFlats = AutoMapper.Mapper.Map<IList<BDOFriendship>, IList<RequestFlat>>(requests);
            //var requestFlats = requests.Select(x => new RequestFlat
            //{
            //    UserOID = x.User.OID,
            //    UserUsername = x.User.Username,
            //    Created = x.Created
            //}).ToList();

            var dto = new TransportObject(Protocol.MessageType.IM_OK);
            dto.Add("Requests", requestFlats);

            return dto;
        }

        public TransportObject SendRequest(TransportObject to)
        {
            var senderOid = to.Get<long>("MyOid");
            var recipientOid = to.Get<long>("UserOid");

            //var broUsers = ObjectFactory.GetInstance<BROUsers>();
            //var broFriendship = ObjectFactory.GetInstance<BROFriendship>();

            var sender = ObjectFactory.GetInstance<BROUsers>().GetByOid(senderOid);
            var recipient = ObjectFactory.GetInstance<BROUsers>().GetByOid(recipientOid);

            var dto = new TransportObject();

            if (recipient == null)
            {
                dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "User doesn't exist.");
                return dto;
            }

            if (ObjectFactory.GetInstance<BROFriendship>().AlreadyRequested(sender, recipient))
            {
                var errorMsg = ObjectFactory.GetInstance<BROFriendship>().AlreadyAccepted(sender, recipient)
                                   ? "Already friends."
                                   : "User already asked for friendship.";
                dto.Add("Error", errorMsg);
                dto.Type = Protocol.MessageType.IM_ERROR;
                
                return dto;
            }

            var bdo = new BDOFriendship {User = sender, Friend = recipient, IsAccepted = false};
            ObjectFactory.GetInstance<BROFriendship>().Create(bdo);

            dto.Type = Protocol.MessageType.IM_OK;
            return dto;
        }

        [UnitOfWork]
        public TransportObject AcceptRequest(TransportObject to)
        {
            var userOid = to.Get<long>("UserOid");
            var myOid = to.Get<long>("MyOid");

            //var broUsers = ObjectFactory.GetInstance<BROUsers>();
            //var broFriendship = ObjectFactory.GetInstance<BROFriendship>();

            var sender = ObjectFactory.GetInstance<BROUsers>().GetByOid(myOid);
            var recipient = ObjectFactory.GetInstance<BROUsers>().GetByOid(userOid);

            var dto = new TransportObject();

            if (ObjectFactory.GetInstance<BROFriendship>().AlreadyAccepted(sender, recipient))
            {
                dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "Request already accepted.");
                return dto;
            }

            var request = ObjectFactory.GetInstance<BROFriendship>().GetBySendeRecipient(sender, recipient);
            var result = ObjectFactory.GetInstance<BROFriendship>().AcceptRequest(request);
                
            if (result)
            {
                dto.Type = Protocol.MessageType.IM_OK;
            }
            else
            {
                dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "Unable to accept request.");
                return dto;
            }

            return GetRequests(to);
        }

        public TransportObject DeleteRequest(TransportObject to)
        {
            throw new NotImplementedException();
        }
    }
}

