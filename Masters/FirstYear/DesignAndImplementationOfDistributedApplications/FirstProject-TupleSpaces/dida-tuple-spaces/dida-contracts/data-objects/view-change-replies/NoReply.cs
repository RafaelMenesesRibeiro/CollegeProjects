using System;
using System.Runtime.Serialization;

namespace dida_contracts.data_objects.reply_data_types
{
    [Serializable]
    public class NoReply : ReplyData, ISerializable
    {
        public ServerData ServerData { get; }

        public NoReply(ServerData server, int mid) : base(mid)
        {
            ServerData = server;
        }

        public NoReply(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ServerData = (ServerData)info.GetValue("ServerData", typeof(ServerData));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ServerData", ServerData);
        }
    }
}
