using System;
using System.Reflection;

namespace InstantMessenger.Communication
{
    public interface ITransportObjectProcessor
    {
        void Process(Type dataManagerType, MethodInfo getMethodInfo, TransportObject to);
    }
}
