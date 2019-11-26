using System;
using System.Runtime.Serialization;

namespace dida_contracts.data_objects
{
    [Serializable]
    public class TotalOrderData : ISerializable
    {
        public int RequestId { get; }
        public int TotalOrderId { get; }

        public TotalOrderData(int requestId, int totalorderId)
        {
            RequestId = requestId;
            TotalOrderId = totalorderId;
        }

        #region Serialization Methods
        public TotalOrderData(SerializationInfo info, StreamingContext context)
        {
            RequestId = (int)info.GetValue("RequestId", typeof(int));
            TotalOrderId = (int)info.GetValue("TotalOrderId", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RequestId", RequestId);
            info.AddValue("TotalOrderId", TotalOrderId);
        }
        #endregion
    }
}

