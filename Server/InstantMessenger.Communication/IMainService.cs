using System.ServiceModel;

namespace InstantMessenger.Communication
{
    [ServiceContract]
    public interface IMainService
    {
        [OperationContract]
        Response ProcessRequest(Request parameter);
    }
}
