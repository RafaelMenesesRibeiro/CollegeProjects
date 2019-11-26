using System;
using dida_contracts.data_objects;

namespace dida_clients.data_objects
{
    class SMRResponse
    {
        public ServerData serverData { get; }
        public ReplyData replyData { get; }

        #region Constructors
        public SMRResponse(ServerData sData, ReplyData rData)
        {
            serverData = sData;
            replyData = rData;
        }
        #endregion
    }
}
