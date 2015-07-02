using System;
using System.Reflection;
using System.ServiceModel;
using InstantMessenger.Communication;
using InstantMessenger.Core;

namespace WebServices
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class MainService : IMainService
    {
        public Response ProcessRequest(Request parameter)
        {
            Type dataManagerType = Type.GetType(parameter.DataManagerTypeString);

            if (dataManagerType == null)
            {
                throw new Exception("Unable to find datamanger " + parameter.DataManagerTypeString);
            }

            object dataManager = ObjectFactory.GetInstance(dataManagerType);
            MethodInfo method = dataManager.GetType().GetMethod(parameter.MethodName);
            TransportObject to = parameter.TransportObject;

            var result = ObjectFactory.GetInstance<IMainRequestHandler>().Execute(to, dataManager, method);

            return new Response(result);
        }
    }
}
