using System;
using System.Runtime.Serialization;
using dida_contracts.data_objects;

namespace dida_servers.data_objects.election_data_types
{
    [Serializable]
    public class ElectionRequestAnswerData : ElectionData, ISerializable
    {
        private bool isAccept;

        public ElectionRequestAnswerData(ServerData sd, bool isA) : base(sd) => isAccept = isA;

        #region Serialization
        public ElectionRequestAnswerData(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.isAccept = (bool)info.GetValue("isAccept", typeof(bool));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("isAccept", this.isAccept);
        }
        #endregion
    }
}
