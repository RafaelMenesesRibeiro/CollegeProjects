using dida_contracts.data_objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dida_contracts.web_services
{
    public interface IClientService
    {
        void recieveNewView();
        void haltRequests();
        void ReceiveAnswer(ServerData serverData, ReplyData replyData);

    }
}
