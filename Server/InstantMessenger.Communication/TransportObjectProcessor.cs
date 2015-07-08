using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessenger.Communication
{
    public class TransportObjectProcessor : ITransportObjectProcessor
    {
        public void Process(Type dataManagerType, MethodInfo method, TransportObject to)
        {
            InitTO(to);
            var response = CallMainService(dataManagerType, method, to);
            ProcessReturnedTO(response, to);
        }

        private void InitTO(TransportObject to)
        {
        }

        private TransportObject CallMainService(Type dataManagerType, MethodInfo method, TransportObject to)
        {
            using (var factory = new ChannelFactory<IMainService>())
            {
                var proxy = factory.CreateChannel();

                var param = new Request(dataManagerType, method, to);

                try
                {
                    var response = proxy.ProcessRequest(param);
                    return response.TransportObject;
                }
                catch (TargetInvocationException ex)
                {
                    factory.Abort();

                    throw;
                }
            }
        }

        private void ProcessReturnedTO(TransportObject response, TransportObject to)
        {
            to.AddFromDict(response.Items);
        }
    }
}
