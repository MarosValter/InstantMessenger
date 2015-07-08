using InstantMessenger.Communication;

namespace InstantMessenger.Common.DM
{
    public interface IUsersDataManager : IDataManager
    {
        TransportObject LoginUser(TransportObject to);
        void Logout(long oid);
        TransportObject Register(TransportObject to);
        TransportObject MainWindowInit(TransportObject to);
        
    }
}