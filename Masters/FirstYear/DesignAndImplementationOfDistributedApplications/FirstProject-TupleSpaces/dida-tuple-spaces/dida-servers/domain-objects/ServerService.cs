using System;
using dida_contracts.helpers;
using dida_contracts.exceptions;
using dida_contracts.web_services;
using dida_contracts.data_objects;
using dida_contracts.domain_objects;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace dida_servers.domain_objects
{
    public class ServerService : MarshalByRefObject, IServerService, ISMRServerService
    {
        #region Fields and Constructors
        protected readonly Server server;
        private int requestNum = 0;

        public ServerService(Server server)
        {
            this.server = server;
        }
        #endregion

        #region General Tuple Space Contracts Implementation
        public ReplyData Request(RequestData requestData)
        {
            return server.HandleRequest(requestData, requestNum++);
        }
        #endregion

        #region SMR Leader Election Contracts Implementation
        public void ReceiveTotalOrder(TotalOrderData totalorderData) => server.ReceiveTotalOrder(totalorderData);
        #endregion

        #region View Change Contracts Implementation
        public ReplyData RecieveImAlive()
        {
            return server.RecieveImAlive();
        }

        public void ReceiveTakeTuple(RequestData takeCapsuleData) => server.ReceiveTakeTuple(takeCapsuleData);

        public List<DIDATuple> SendMatchingTuples(DIDATuple didaTuple) => server.SendMatchingTuples(didaTuple);

        public ReplyData RecieveViewChangeProposal(View proposedView)
        {
            return server.RecieveViewChangeProposal(proposedView);
        }

        public void CommitViewChangeProposal(View serverView, Dictionary<string, int> messageIdTable, ConcurrentBag<object> tupleSpaceObjects)
        {
            server.CommitViewChangeProposal(serverView, messageIdTable, tupleSpaceObjects);
        }
        #endregion

        #region PuppetMaster Implementation

        public void Crash()
        {
            server.Crash();
        }

        public void Status()
        {
            server.Status();
        }

        public void Freeze(string v)
        {
            server.Freeze();
        }

        public void Unfreeze(string v)
        {
            server.Unfreeze();
        }

        #endregion

    }
}