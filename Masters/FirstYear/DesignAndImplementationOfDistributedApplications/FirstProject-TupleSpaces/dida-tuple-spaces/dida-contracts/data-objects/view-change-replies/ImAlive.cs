using System;
using System.Runtime.Serialization;
using dida_contracts.domain_objects;

namespace dida_contracts.data_objects.reply_data_types
{
    [Serializable]
    public class ImAliveReply : ReplyData, ISerializable
    {
        public ServerData ServerData { get; }
        public View ServerView { get; }

        public ImAliveReply(ServerData server, View view) : base(-1)
        {
            ServerData = server;
            ServerView = view;
        }

        public ImAliveReply(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ServerData = (ServerData)info.GetValue("ServerData", typeof(ServerData));
            ServerView = (View)info.GetValue("ServerView", typeof(View));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ServerData", ServerData);
            info.AddValue("ServerView", ServerView);
        }
    }
}