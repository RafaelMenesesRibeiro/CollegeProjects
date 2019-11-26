using System;
using System.Runtime.Serialization;
using dida_contracts.domain_objects;

namespace dida_contracts.data_objects.reply_data_types
{
    // TODO VERIFY OTHER REPLYDATA CLASSES FOR NON ACCESSIBLE RESPONSE.
    [Serializable]
    public class TupleReply : ReplyData, ISerializable
    {
        public DIDATuple Tuple { get; }

        public TupleReply(int mid, DIDATuple tuple) : base(mid)
        {
            Tuple = tuple;
        }

        #region Serialization Methods
        public TupleReply(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Tuple = (DIDATuple)info.GetValue("Tuple", typeof(DIDATuple));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Tuple", Tuple);
        }
        #endregion
    }
}
