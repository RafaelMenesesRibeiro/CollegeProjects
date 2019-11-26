using dida_contracts.data_objects;
using dida_contracts.domain_objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace dida_contracts.web_services
{
    public interface ISMRServerService
    {
        [OperationContract]
        void ReceiveTotalOrder(TotalOrderData totalorderData);

        [OperationContract]
        void ReceiveTakeTuple(RequestData takeCapsuleData);

        [OperationContract]
        List<DIDATuple> SendMatchingTuples(DIDATuple didaTuple);
    }
}
