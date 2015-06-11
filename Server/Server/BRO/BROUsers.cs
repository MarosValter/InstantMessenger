using System.Collections.Generic;
using System.Linq;
using InstantMessenger.Core.Base;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.DataModel.BDO;

namespace InstantMessenger.DataModel.BRO
{
    public class BROUsers : BROGeneric<BDOUser>
    {
        #region SQL queries

        private const string QLogoutAllUsers = "UPDATE T01Users " +
                                               "SET T01IsOnline = 'False'";

        #endregion

        public BROUsers(IRepository<BDOUser> repository)
            : base(repository)
        { }

        public BDOUser GetUserByUsername(string username)
        {
            return Repository.FindOne(x => x.Username == username);
        }

        public IList<BDOUser> GetUsersStartingNameWith(string username)
        {
            return Repository.CreateQuery()
                             .Where(x => x.Username.StartsWith(username))
                             .ToList();
        }

        public void LogoutAllUsers()
        {
            Repository.CreateSQL(QLogoutAllUsers);
        }
    }
}
