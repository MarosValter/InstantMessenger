using InstantMessenger.Communication;

namespace InstantMessenger.Common.DM
{
    public interface IConversationsDataManager : IDataManager
    {
        TransportObject Init(TransportObject to);
        TransportObject GetOldMessages(TransportObject to);
        TransportObject SendMessage(TransportObject to);
    }
}