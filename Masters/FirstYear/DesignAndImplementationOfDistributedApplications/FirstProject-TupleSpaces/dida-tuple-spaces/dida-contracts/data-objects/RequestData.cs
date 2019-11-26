using System;
using System.Runtime.Serialization;
using dida_contracts.domain_objects;
using dida_contracts.helpers;

namespace dida_contracts.data_objects
{
    [Serializable]
    public class RequestData : ISerializable
    {
        public int RequestId { get; }
        public int ViewId { get; }
        public string ClientId { get; }
        public string ClientURL { get; }
        public DIDATuple TupleData { get; set; }
        public EOperationType Operation { get; }

        public RequestData(int requestId, int viewId, string clientId, string clientUrl, DIDATuple tuple, EOperationType operationType)
        {
            RequestId = requestId;
            ViewId = viewId;
            ClientId = clientId;
            ClientURL = clientUrl;
            TupleData = tuple;
            Operation = operationType;
        }

        #region Serialization Methods
        public RequestData(SerializationInfo info, StreamingContext context)
        {
            RequestId = (int)info.GetValue("RequestId", typeof(int));
            ViewId = (int)info.GetValue("ViewId", typeof(int));
            ClientId = (string)info.GetValue("ClientId", typeof(string));
            ClientURL = (string)info.GetValue("ClientURL", typeof(string));
            TupleData = (DIDATuple)info.GetValue("TupleData", typeof(DIDATuple));
            Operation = (EOperationType)info.GetValue("Operation", typeof(EOperationType));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RequestId", RequestId);
            info.AddValue("ViewId", ViewId);
            info.AddValue("ClientId", ClientId);
            info.AddValue("ClientURL", ClientURL);
            info.AddValue("TupleData", TupleData);
            info.AddValue("Operation", Operation);
        }
        #endregion
    }
}
