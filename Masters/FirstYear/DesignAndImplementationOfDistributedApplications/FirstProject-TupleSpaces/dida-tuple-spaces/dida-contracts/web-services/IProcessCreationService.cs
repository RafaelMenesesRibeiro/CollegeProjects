using System.ServiceModel;

namespace dida_contracts.web_services
{
    public interface IProcessCreationService
    {
        [OperationContract]
        void Server(string name, int port, string urlName, int minDelay, int maxDelay, string mode);

        [OperationContract]
        void Client(string name, int port, string urlName, string scriptPath, string mode);
    }
}
