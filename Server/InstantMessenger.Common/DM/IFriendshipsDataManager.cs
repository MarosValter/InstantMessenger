using InstantMessenger.Communication;

namespace InstantMessenger.Common.DM
{
    public interface IFriendshipsDataManager : IDataManager
    {
        TransportObject GetRequests(TransportObject to);
        TransportObject SendRequest(TransportObject to);
        TransportObject AcceptRequest(TransportObject to);
        TransportObject DeleteRequest(TransportObject to);
        TransportObject FindUsers(TransportObject to);
    }
}