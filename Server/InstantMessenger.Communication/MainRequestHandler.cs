using System.Reflection;

namespace InstantMessenger.Communication
{
    public class MainRequestHandler : IMainRequestHandler
    {
        public TransportObject Execute(TransportObject to, object dataManager, MethodInfo method)
        {
            try
            {
                var result = (TransportObject) method.Invoke(dataManager, new object[] {to});

                return result;
            }
            catch (TargetInvocationException ex)
            {
                throw;
            }
        }
    }
}
