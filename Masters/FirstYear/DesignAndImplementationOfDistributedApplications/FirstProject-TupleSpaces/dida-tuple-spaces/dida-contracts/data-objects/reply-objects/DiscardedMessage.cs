using System;
using System.Runtime.Serialization;

namespace dida_contracts.data_objects.reply_data_types
{
    [Serializable]
    public class DiscardedMsgReply : ReplyData, ISerializable
    {
        public DiscardedMsgReply(int mid) : base(mid)
        {
        }

        public DiscardedMsgReply(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
