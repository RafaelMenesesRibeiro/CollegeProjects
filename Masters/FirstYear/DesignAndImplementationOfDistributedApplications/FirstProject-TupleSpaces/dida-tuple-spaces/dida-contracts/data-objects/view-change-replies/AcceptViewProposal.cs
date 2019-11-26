using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using dida_contracts.domain_objects;

namespace dida_contracts.data_objects.reply_data_types
{
    [Serializable]
    public class AcceptViewProposal : ReplyData, ISerializable
    {
        public ServerData ServerData { get; }
        public View ServerView { get; }
        public Dictionary<string, int> MessageIdTable { get; }
        public ConcurrentBag<object> TupleSpaceObjects { get; }

        public AcceptViewProposal(ServerData server, View view, Dictionary<string, int> messageIdTable, ConcurrentBag<object> tupleSpaceObjects) : base(-2)
        {
            ServerData = server;
            ServerView = view;
            MessageIdTable = messageIdTable;
            TupleSpaceObjects = tupleSpaceObjects;
        }

        public AcceptViewProposal(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ServerData = (ServerData)info.GetValue("ServerData", typeof(ServerData));
            ServerView = (View)info.GetValue("ServerView", typeof(View));
            MessageIdTable = (Dictionary<string, int>)info.GetValue("MessageIdTable", typeof(Dictionary<string, int>));
            TupleSpaceObjects = (ConcurrentBag<object>)info.GetValue("TupleSpaceObjects", typeof(ConcurrentBag<object>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ServerData", ServerData);
            info.AddValue("ServerView", ServerView);
            info.AddValue("MessageIdTable", MessageIdTable);
            info.AddValue("TupleSpaceObjects", TupleSpaceObjects);
        }
    }
}