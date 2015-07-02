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
            ProcessReturnedTO(response);
        }

        private void InitTO(TransportObject to)
        {
            throw new NotImplementedException();
        }

        private TransportObject CallMainService(Type dataManagerType, MethodInfo method, TransportObject to)
        {
            var host = new ServiceHost(typeof (IMainService));
            host.Description.Endpoints
            ServiceEndpoint endpoint = default(ServiceEndpoint);

            using (var factory = new ChannelFactory<IMainService>(endpoint))
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

        private void ProcessReturnedTO(TransportObject to)
        {
            
        }
    }
}
