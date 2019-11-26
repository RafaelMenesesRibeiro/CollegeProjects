using System;
using System.Runtime.Serialization;

namespace dida_contracts.data_objects
{
    public enum EReplyType { NoReply, Acknowledge, DiscardedMessage, LockRefused, NewView, NoTupleFound, TupleReply, TupleSetReply, AcceptedViewProposal, IAmAlive }

    [Serializable]
    public abstract class ReplyData : ISerializable
    {
        protected readonly int mid; 

        public ReplyData(int mid)
        {
            this.mid = mid;
        }

        public ReplyData(SerializationInfo info, StreamingContext context)
        {
            this.mid = (int)info.GetValue("mid", typeof(int));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mid", this.mid);
        }
    }
}
