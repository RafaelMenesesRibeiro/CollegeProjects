using System;
using System.Runtime.Serialization;
namespace dida_contracts.data_objects.election_data_types
{
    [Serializable]
    public class ElectionRequestData : ElectionData, ISerializable
    {
        private int viewId;

        public ElectionRequestData(ServerData sd, int vId) : base(sd) => viewId = vId;

        #region Serialization
        public ElectionRequestData(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.viewId = (int)info.GetValue("viewId", typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("viewId", this.viewId);
        }
        #endregion
    }
}
