using System;
using System.Runtime.Serialization;
using dida_contracts.web_services;

namespace dida_contracts.exceptions
{
    [Serializable]
    public class DidaServerExceptions : ApplicationException, ISerializable
    {
        public IServerService iss;

        public DidaServerExceptions(string message, IServerService iss) : base(message)
        {
            this.iss = iss;
        }

        #region Serialization Methods
        public DidaServerExceptions(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            iss = (IServerService)info.GetValue("iss", typeof(IServerService));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("iss", iss);
        }
        #endregion
    }
}
