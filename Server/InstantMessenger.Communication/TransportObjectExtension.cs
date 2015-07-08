using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using InstantMessenger.Core;

namespace InstantMessenger.Communication
{
    public static class TransportObjectExtension
    {
        public static void Process<TDataManager>(this TransportObject to, Expression<Func<TDataManager, Func<TransportObject, TransportObject>>> method)
        {
            var processor = ObjectFactory.GetInstance<ITransportObjectProcessor>();
            processor.Process(typeof(TDataManager), ServiceMethodHelper<TDataManager>.GetMethodInfo(method), to);
        }     
    }
}
