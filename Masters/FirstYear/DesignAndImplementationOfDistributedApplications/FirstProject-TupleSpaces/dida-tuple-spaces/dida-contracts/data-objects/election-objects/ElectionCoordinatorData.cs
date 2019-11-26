using System;
using System.Runtime.Serialization;
using dida_contracts.data_objects;
using dida_contracts.domain_objects;

namespace dida_contracts.data_objects.election_data_types
{
    [Serializable]
    public class ElectionCoordinatorData : ElectionData, ISerializable
    {
        private int viewId;
        private ServerData oldLeader;

        public ElectionCoordinatorData(ServerData sd, int vId, ServerData oldL) : base(sd)
        {
            viewId = vId;
            oldLeader = oldL;
        }

        public override string ToString() => $"Election Coordinator Data: NEW LEADER is <{serverData.ServerName}>, new View ID is {viewId}, old Leader was {oldLeader.ServerName}";

        #region Serialization
        public ElectionCoordinatorData(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.viewId = (int)info.GetValue("viewId", typeof(int));
            this.oldLeader = (ServerData)info.GetValue("oldLeader", typeof(ServerData));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("viewId", this.viewId);
            info.AddValue("oldLeader", this.oldLeader);
        }
        #endregion
    }
}
