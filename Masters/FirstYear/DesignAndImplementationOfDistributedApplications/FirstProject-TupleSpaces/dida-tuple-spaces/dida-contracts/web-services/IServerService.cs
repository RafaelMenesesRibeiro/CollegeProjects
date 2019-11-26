using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;

using dida_contracts.exceptions;
using dida_contracts.data_objects;
using dida_contracts.domain_objects;
using System.Collections.Concurrent;

namespace dida_contracts.web_services
{
    public interface IServerService
    {
        #region Client Invokable Operations

        [OperationContract]
        ReplyData Request(RequestData requestData);

        #endregion

        #region Server Invokable Operations

        [OperationContract]
        ReplyData RecieveImAlive();
        
        [OperationContract]
        ReplyData RecieveViewChangeProposal(View proposedView);

        [OperationContract]
        void CommitViewChangeProposal(View serverView, Dictionary<string, int> messageIdTable, ConcurrentBag<object> tupleSpaceObjects);
        #endregion

        #region Puppet Master Invokable Operations
        [OperationContract]
        void Crash();

        [OperationContract]
        void Status();

        [OperationContract]
        void Freeze(string v);

        [OperationContract]
        void Unfreeze(string v);
        #endregion
    }
}
