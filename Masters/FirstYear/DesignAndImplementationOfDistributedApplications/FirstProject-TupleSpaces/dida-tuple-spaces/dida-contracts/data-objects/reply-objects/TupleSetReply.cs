using dida_contracts.domain_objects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace dida_contracts.data_objects.reply_data_types
{
    [Serializable]
    public class TupleSetReply : ReplyData, ISerializable
    {
        public List<DIDATuple> TupleSet { get; }

        public TupleSetReply(int mid, List<DIDATuple> tupleSet) : base(mid)
        {
            this.TupleSet = tupleSet;
        }


        #region Serialization Methods
        public TupleSetReply(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.TupleSet = (List<DIDATuple>)info.GetValue("tupleSet", typeof(List<DIDATuple>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("tupleSet", this.TupleSet);
        }
        #endregion
    }
}
