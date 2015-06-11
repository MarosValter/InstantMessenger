using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UsersDataManager : DataManagerBase
    {
        public override void RegisterMapping(IConfiguration mapper)
        {
            mapper.CreateMap<BDOUser, UserFlat>();
            mapper.CreateMap<BDOFriendship, UserFlat>()
                  .ConstructProjectionUsing(src => Mapper.Map<BDOUser, UserFlat>(src.Friend));
        }

        public TransportObject LoginUser(TransportObject to)
        {
            var username = to.Get<string>("Username");
            var pwdHash = to.Get<byte[]>("Password");

            var dto = new TransportObject();

            var user = ObjectFactory.GetInstance<BROUsers>().GetUserByUsername(username);

            if (user == null)
            {
                dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "User doesn't exist!");
                return dto;
            }

            if (user.IsOnline)
            {
                dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "User is already online.");
                return dto;
            }

            if (!user.PasswordHash.SequenceEqual(pwdHash))
            {
                dto.Type = Protocol.MessageType.IM_ERROR;
                dto.Add("Error", "Wrong password!");
                return dto;
            }

            user.IsOnline = true;

            ObjectFactory.GetInstance<BROUsers>().Update(user);
            var flat = Mapper.Map<BDOUser, UserFlat>(user);

            dto.Type = Protocol.MessageType.IM_OK;
            dto.Add("MyOid", user.OID);
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
                dto.Type = Protocol.MessageType.IM_ERROR;
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
            dto.Type = Protocol.MessageType.IM_OK;

            return dto;
        }

        [UnitOfWork]
        public virtual TransportObject MainWindowInit(TransportObject to)
        {
            var myOid = to.Get<long>("MyOid");
            var friends = ObjectFactory.GetInstance<BROFriendship>().GetFriendsByUser(myOid);
            var friendFlats = Mapper.Map<IList<BDOFriendship>, IList<UserFlat>>(friends);
            var requestCount = ObjectFactory.GetInstance<BROFriendship>().GetUserRequestCount(myOid);

            var dto = new TransportObject(Protocol.MessageType.IM_OK);
            dto.Add("Friends", friendFlats);
            dto.Add("RequestCount", requestCount);

            return dto;
        }

        public TransportObject FindUsers(TransportObject to)
        {
            var username = to.Get<string>("Username");
            var users = ObjectFactory.GetInstance<BROUsers>().GetUsersStartingNameWith(username);
            var flats = Mapper.Map<IList<BDOUser>, IList<UserFlat>>(users);

            var dto = new TransportObject(Protocol.MessageType.IM_OK);
            dto.Add("Users", flats);

            return dto;
        }
    }
}

