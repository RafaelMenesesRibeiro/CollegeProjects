using System;
using System.Runtime.Serialization;
using dida_contracts.domain_objects;

namespace dida_contracts.data_objects.reply_data_types
{
    [Serializable]
    public class ViewProposal : ReplyData, ISerializable
    {
        public View ServerView { get; }
        public ServerData ServerData { get; }

        public ViewProposal(View view, ServerData server) : base(-2)
        {
            ServerView = view;
            ServerData = server;
        }

        public ViewProposal(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ServerView = (View)info.GetValue("ServerView", typeof(View));
            ServerData = (ServerData)info.GetValue("ServerData", typeof(ServerData));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ServerView", ServerView);
            info.AddValue("ServerData", ServerData);
        }
    }
}