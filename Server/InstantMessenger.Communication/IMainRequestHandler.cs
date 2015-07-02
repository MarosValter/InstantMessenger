using System.Reflection;

namespace InstantMessenger.Communication
{
    public interface IMainRequestHandler
    {
        TransportObject Execute(TransportObject to, object dataManager, MethodInfo method);
    }
}
