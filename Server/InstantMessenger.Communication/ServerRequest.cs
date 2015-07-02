using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace InstantMessenger.Communication
{
    [DataContract]
    public class Request
    {
        [DataMember]
        public string DataManagerTypeString { get; set; }

        [DataMember]
        public string MethodName { get; set; }

        [DataMember]
        public TransportObject TransportObject { get; set; }

        public Request()
        {
        }

        public Request(Type dataManagerType, MethodInfo method, TransportObject transportObject)
        {
            DataManagerTypeString = dataManagerType.AssemblyQualifiedName;
            MethodName = method.Name;
            TransportObject = transportObject;
        }
    }

    [DataContract]
    public class Response
    {
        [DataMember]
        public TransportObject TransportObject { get; set; }

        public Response()
        {
        }

        public Response(TransportObject to)
        {
            TransportObject = to;
        }
    }
}
