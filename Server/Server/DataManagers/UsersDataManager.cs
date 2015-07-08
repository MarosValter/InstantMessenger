using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using InstantMessenger.Common;
using InstantMessenger.Common.DM;
using InstantMessenger.Common.Flats;
using InstantMessenger.Communication;
using InstantMessenger.Core;
using InstantMessenger.Core.UOW;
using InstantMessenger.DataModel.BDO;
using InstantMessenger.DataModel.BRO;

namespace InstantMessenger.DataModel.DataManagers
{
    public class UsersDataManager :  IUsersDataManager
    {
        public void RegisterMapping(IConfiguration mapper)
        {
            mapper.CreateMap<BDOUser, UserFlat>();
            mapper.CreateMap<BDOFriendship, UserFlat>()
                  .ConstructProjectionUsing(src => Mapper.Map<BDOUser, UserFlat>(src.Friend));
            mapper.CreateMap<BDOConversation, ConversationFlat>();
            //.ConstructUsing(MapConversation);
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.Join(", ", src.GetUsers.Select(x => x.Username))));
        }

        //private ConversationFlat MapConversation(BDOConversation bdoConversation)
        //{
        //    var flat = new ConversationFlat();
        //    flat.OID = bdoConversation.OID;
        //    flat.IsDialog = bdoConversation.IsDialog;

        //    var users = bdoConversation.GetUsers;
        //    flat.Name = string.Join(", ", users.Select(x => x.Username));
        //    flat.IsUserOnline = bdoConversation.IsDialog
        //                            ? users.First(x => x.)
        //}

        public TransportObject LoginUser(TransportObject to)
        {
            var username = to.Get<string>("Username");
            var pwdHash = to.Get<byte[]>("Password");

            var dto = new TransportObject();

            var user = ObjectFactory.GetInstance<BROUsers>().GetUserByUsername(username);

            if (user == null)
            {
                //dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "User doesn't exist!");
                return dto;
            }

            if (user.IsOnline)
            {
                //dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "User is already online.");
                return dto;
            }

            if (!user.PasswordHash.SequenceEqual(pwdHash))
            {
                //dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "Wrong password!");
                return dto;
            }

            user.IsOnline = true;

            ObjectFactory.GetInstance<BROUsers>().Update(user);
            var flat = Mapper.Map<BDOUser, UserFlat>(user);

            //dto.Type = Protocol.MessageType.IM_OK;
            dto.Add("UserFlat", flat);

            return dto;
        }

        public void Logout(long oid)
        {
            var user = ObjectFactory.GetInstance<BROUsers>().GetByOid(oid);
            user.IsOnline = false;
            user.LastLogin = DateTime.Now;
            ObjectFactory.GetInstance<BROUsers>().Update(user);
        }

        public TransportObject Register(TransportObject to)
        {
            var username = to.Get<string>("Username");
            var pwd = to.Get<byte[]>("Password");
            var email = to.Get<string>("Email");

            var dto = new TransportObject();

            var user = ObjectFactory.GetInstance<BROUsers>().GetUserByUsername(username);

            if (user != null)
            {
                //dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "User already exists!");
                return dto;
            }

            user = new BDOUser
            {
                Username = username,
                PasswordHash = pwd,
                Email = email
            };

            ObjectFactory.GetInstance<BROUsers>().Create(user);
            //dto.Type = Protocol.MessageType.IM_OK;

            return dto;
        }

        [UnitOfWork]
        public virtual TransportObject MainWindowInit(TransportObject to)
        {
            var myOid = to.Get<long>("MyOid");
            //var friends = ObjectFactory.GetInstance<BROFriendship>().GetFriendsByUser(myOid);
            var requestCount = ObjectFactory.GetInstance<BROFriendship>().GetUserRequestCount(myOid);
            var user = ObjectFactory.GetInstance<BROUsers>().GetByOid(myOid);

            var conversations = user.GetConversations;
            var conversationFlats = Mapper.Map<IList<BDOConversation>, IList<ConversationFlat>>(conversations);

            foreach (var item in conversationFlats)
            {
                var bdo = conversations.First(x => x.OID == item.OID);
                var users = bdo.GetUsers;

                item.IsUserOnline = item.IsDialog
                    ? users.First(x => x.OID != myOid).IsOnline
                    : (bool?)null;
                item.Name = string.Join(", ", users.Where(x => x.OID != myOid).Select(x => x.Username));
            }

            //var friendFlats = Mapper.Map<IList<BDOFriendship>, IList<UserFlat>>(friends);
            var flat = Mapper.Map<BDOUser, UserFlat>(user);

            var dto = new TransportObject(/*Protocol.MessageType.IM_OK*/);
            //dto.Add("Friends", friendFlats);
            dto.Add("Conversations", conversationFlats);
            dto.Add("RequestCount", requestCount);
            dto.Add("UserFlat", flat);

            return dto;
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

