using System;
using System.Runtime.Serialization;
using dida_contracts.data_objects;

namespace dida_contracts.data_objects
{
    [Serializable]
    public abstract class ElectionData : ISerializable
    {
        protected ServerData serverData;

        public ElectionData(ServerData sd) => serverData = sd;

        #region Serialization
        public ElectionData(SerializationInfo info, StreamingContext context) => this.serverData = (ServerData)info.GetValue("serverId", typeof(ServerData));

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue("serverId", this.serverData);
        #endregion
    }
}
