using System.Collections.Generic;
using System.Linq;
using InstantMessenger.Core.Base;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.Core.UOW;
using InstantMessenger.DataModel.BDO;

namespace InstantMessenger.DataModel.BRO
{
    public class BROUsers : BROGeneric<BDOUser>
    {
        #region SQL queries

        private const string QLogoutAllUsers = "UPDATE BDOUser " +
                                               "SET IsOnline = 0";

        #endregion

        public BDOUser GetUserByUsername(string username)
        {
            return Repository.FindOne(x => x.Username == username);
        }

        public IList<BDOUser> GetUsersStartingNameWith(string username)
        {
            return Repository.CreateQuery().Where(x => x.Username.StartsWith(username)).ToList();
        }

        [UnitOfWork]
        public virtual void LogoutAllUsers()
        {
            Repository.CreateHQL(QLogoutAllUsers).ExecuteUpdate();
        }
    }
}
