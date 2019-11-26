using dida_contracts.data_objects;

namespace dida_clients.helpers
{
    public enum ERequestStatus { NotReady, Ready, InProgress, Finished }
    public enum EXuLiskovOperation { Read, Write, TakeOne, TakeTwo, Unlock }
    public enum ESMROperation { Read, Write, Take }

    public class Request
    {
        public RequestData RequestData { get; }
        public ERequestStatus Status { get; set; }

        public Request(RequestData requestData)
        {
            RequestData = requestData;
        }
    }

    public class XuLiskovRequest : Request
    {
        public EXuLiskovOperation Operation { get; set; }

        public XuLiskovRequest(RequestData requestData, EXuLiskovOperation operation) : base(requestData)
        {
            Operation = operation;
        }
    }

    public class SMRRequest : Request
    {
        public ESMROperation Operation { get; set; }

        public SMRRequest(RequestData requestData, ESMROperation operation) : base(requestData)
        {
            Operation = operation;
        }
    }
}
