using System;
using System.Linq;
using System.Collections.Generic;
using dida_contracts.helpers;
using dida_contracts.data_objects;
using dida_contracts.data_objects.reply_data_types;
using dida_contracts.domain_objects;
using dida_contracts.web_services;
using dida_servers.helpers;
using dida_contracts.data_objects.election_data_types;
using System.Threading;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.Remoting;

namespace dida_servers.domain_objects
{
    class SMRServer : Server
    {
        #region Fields
        private int rg;
        private int sg;
        private Dictionary<int, RequestData> holdbackQueueSequencer;
        private Dictionary<int, RequestData> holdbackQueue;
        private Dictionary<int, TotalOrderData> deliveryQueue;
        ManualResetEvent receiveTakeTupleFromLeader = new ManualResetEvent(false);
        RequestData takeTupleFromLeader;
        #endregion

        #region Constructors
        public SMRServer(int channelPort, string serverName, int minDelay, int maxDelay, View view, bool verbose) :
            base(channelPort, serverName, minDelay, maxDelay, view, verbose)
        {
            rg = 0;
            sg = 0;
            holdbackQueueSequencer = new Dictionary<int, RequestData>();
            holdbackQueue = new Dictionary<int, RequestData>();
            deliveryQueue = new Dictionary<int, TotalOrderData>();
        }
        #endregion

        #region Request Handling
        internal override ReplyData HandleRequest(RequestData requestData, int requestNum)
        {
            Random random = new Random();
            int delayValue = random.Next(minDelay, maxDelay);
            Thread.Sleep(delayValue);

            while (freezed || requestNum < currentReq)
            {
                Thread.Sleep(200);
            }

            Utils.Print("[*] SMRServer: Handle Request.");
            if (CheckOldMsg(requestData)) return new DiscardedMsgReply(requestData.RequestId);
            if (CheckOldView(requestData)) return new ViewProposal(viewManager.GetView(), ServerData);

            holdbackQueue.Add(requestData.RequestId, requestData);
            Utils.Print(" [*] SMR Server: Added to Holdback Queue.", verbose: Verbose);
            Utils.Print($" [*] SMR Server: Request ID: {requestData.RequestId}.", verbose: Verbose);

            View view = viewManager.GetView();
            string leaderUId = view.ManagerUId;
            ServerData leaderData = view.Find(leaderUId);
            if (leaderData.Equals(ServerData)) // As the View Leader.
            {
                int requestId = requestData.RequestId;
                Utils.Print($" [*] SMR Server Sequencer: Request ID: {requestId}.", verbose: Verbose);

                //If the message received is not the following message of the previously received
                //from the same Client, the message is added to the hold back list, to ensure FIFO
                //ordering.
                int lastClientMessageId = mIdTable[requestData.ClientId];
                Utils.Print($" [*] SMR Server Sequencer: Last Client Request ID: {lastClientMessageId}.", verbose: Verbose);
                if (requestId > lastClientMessageId + 1)
                {
                    holdbackQueueSequencer.Add(requestId, requestData);
                    return null;
                }
                SendTotalOrder(requestData.ClientId, requestId);
                return null;
            }    
            CheckDeliveryQueue();
            return null;
        }

        internal override void ReceiveTotalOrder(TotalOrderData totalorderData)
        {
            int requestId = totalorderData.RequestId;

            Utils.Print(" [*] SMR Server: Received TotalOrderData.", verbose: Verbose);

            if (holdbackQueue.ContainsKey(requestId))
            {
                int totalorderId = totalorderData.TotalOrderId;

                if (totalorderId < rg) Utils.Print(" [x] SMR Server: Received unexpected TotalOrderData.");
                else if (totalorderId > rg) deliveryQueue.Add(requestId, totalorderData);
                else ExecuteRequest(requestId);
            }
            else deliveryQueue.Add(requestId, totalorderData);
        }

        public override void ReceiveTakeTuple(RequestData takeCapsuleData)
        {
            Utils.Print(" [*] SMR Server: Received Take Tuple from Leader.", verbose: Verbose);
            takeTupleFromLeader = takeCapsuleData;
            receiveTakeTupleFromLeader.Set();
        }

        internal void ExecuteRequest(int requestId)
        {
            Utils.Print(" [*] SMR Server: Executing request.", verbose: Verbose);
            RequestData requestData = holdbackQueue[requestId];
            holdbackQueue.Remove(requestId);
            ReplyData replyData = DeliverRequest(requestData);
            SendResponse(requestData, replyData);
            rg += 1;
            CheckDeliveryQueue(); //Checks if any of the requests in the delivery queue can be executed (if rg == requestId)
        }

        internal void SendResponse(RequestData requestData, ReplyData replyData)
        {
            string clientProxyURL = $"{requestData.ClientURL}/{requestData.ClientId}";
            IClientService clientProxy = (IClientService)Activator.GetObject(typeof(IClientService), clientProxyURL);
            clientProxy.ReceiveAnswer(ServerData, replyData);
            Utils.Print("[*] SMR Server: Sent Response.");
        }

        internal void CheckDeliveryQueue()
        {
            Utils.Print(" [*] SMR Server: Checking Delivery Queue.", verbose: Verbose);
            //Checks if any of the request's whose TotalOrderId came earlier can be executed now.
            foreach (int requestId in deliveryQueue.Keys)
            {
                TotalOrderData totalorderData = deliveryQueue[requestId];
                int totalorderId = totalorderData.TotalOrderId;
                //If the totalorderId of the next request to be executed is there, checks if the request is in holdback.
                if (totalorderId == rg)
                {   
                    //If both the totalorderId and the request are in the server, executes, and recursively checks if the next are.
                    if (holdbackQueue.ContainsKey(requestId)) ExecuteRequest(requestId);
                    return;
                }
            }
        }
        #endregion

        #region Operations
        internal ReplyData DeliverRequest(RequestData requestData)
        {
            mIdTable[requestData.ClientId] = requestData.RequestId;

            Random random = new Random();
            int delayValue = random.Next(minDelay, maxDelay);
            Thread.Sleep(delayValue);

            Utils.Print(" [*] SMR Server: Delivering Request", verbose: Verbose);

            switch (requestData.Operation)
            {
                case EOperationType.Read:
                    Utils.Print(" [*] Executing <READ> operation...", verbose: Verbose);
                    return Read(requestData);
                case EOperationType.Take:
                    Utils.Print(" [*] Executing <TAKE> operation...", verbose: Verbose);
                    return Take(requestData);
                case EOperationType.Write:
                    Utils.Print(" [*] Executing <WRITE> operation...", verbose: Verbose);
                    return Write(requestData);
                default:
                    return new DiscardedMsgReply(requestData.RequestId);
            }
        }

        internal ReplyData Write(RequestData requestData)
        {
            tupleSpace.Add(new List<object> { requestData.TupleData, null });
            return new AckReply(requestData.RequestId);
        }

        internal ReplyData Read(RequestData requestData)
        {
            foreach (var entry in tupleSpace)
            {
                DIDATuple tuple = (DIDATuple)((List<object>)entry)[0];
                if (tuple.Matches(requestData.TupleData))
                {
                    return new TupleReply(requestData.RequestId, tuple);
                }
            }
            return new NoTupleFoundReply(requestData.RequestId);
        }

        #region Take Operation
        internal ReplyData Take(RequestData requestData)
        {
            ViewManager viewM = viewManager;
            View view = viewM.GetView();
            ServerData leaderData = view.Find(view.ManagerUId);
            if (leaderData.Equals(ServerData)) // As the View Leader.
            {
                receiveTakeTupleFromLeader.Reset();
                List<List<DIDATuple>> serversMatchingTuples = new List<List<DIDATuple>>();
                List<ServerData> replicasList = viewManager.GetKnownReplicas();
                
                Utils.Print($" [*] SMR Server Leader: Calling {replicasList.Count()} servers.", verbose: Verbose);
                foreach (ServerData serverData in replicasList)
                {
                    if (!serverData.Equals(leaderData))
                    {
                        string serverProxyURL = $"tcp://{serverData.ServerURL}/{serverData.ServerName}";
                        ISMRServerService serverProxy = (ISMRServerService)Activator.GetObject(typeof(ISMRServerService), serverProxyURL);
                        try
                        {
                            List<DIDATuple> matchingTuples = serverProxy.SendMatchingTuples(requestData.TupleData);
                            serversMatchingTuples.Add(matchingTuples);
                        }
                        catch (SocketException)
                        {
                            viewManager.TryViewChange(viewManager.GetKnownReplicas());
                        }
                    }
                }
                serversMatchingTuples.Add(GetMatchingTuples(requestData.TupleData));
                PrintTuplesListsList(serversMatchingTuples);

                DIDATuple takeTuple = GetRandomIntersectedTuple(serversMatchingTuples);
                if (takeTuple == null) return new NoTupleFoundReply(requestData.RequestId);

                Utils.Print($" [*] SMR Server Leader: Taking {takeTuple}.", verbose: Verbose);

                // Removed tuple from all replicas.
                foreach (ServerData serverData in replicasList)
                {
                    string serverProxyURL = $"tcp://{serverData.ServerURL}/{serverData.ServerName}";
                    ISMRServerService serverProxy = (ISMRServerService)Activator.GetObject(typeof(ISMRServerService), serverProxyURL);
                    RequestData leaderTakeRequestData = new RequestData(0, 0, "", "", takeTuple, EOperationType.Take);
                    try
                    {
                        serverProxy.ReceiveTakeTuple(leaderTakeRequestData);
                    }
                    catch (SocketException)
                    {
                        viewManager.TryViewChange(viewManager.GetKnownReplicas());
                    }
                }

                object toRemove = takeTuple;
                if (tupleSpace.TryTake(out toRemove)) return new TupleReply(requestData.RequestId, takeTuple);
                else return new NoTupleFoundReply(requestData.RequestId); // Should be impossible to execute this else.
            }
            // As a follower.
            else
            {
                receiveTakeTupleFromLeader.WaitOne(); // Waits for the signal of when the Take Tuple arrives.
                // After receiving the Tuple from the Leader, for the Take Operation.
                DIDATuple takeTuple = takeTupleFromLeader.TupleData;
                Utils.Print($" [*] SMR Server: Taking {takeTuple}.", verbose: Verbose);
                if (takeTuple == null) return new NoTupleFoundReply(requestData.RequestId);
                object toRemove = takeTuple;
                if (tupleSpace.TryTake(out toRemove)) return new TupleReply(requestData.RequestId, takeTuple);
                else return new NoTupleFoundReply(requestData.RequestId); // Should be impossible to execute this else.

                // Add send Leader an Ack.
            }
        }
        private DIDATuple GetRandomIntersectedTuple(List<List<DIDATuple>> lists)
        {
            List<DIDATuple> intersectList = lists[0];

            for (int i = 1; i < lists.Count(); i++)
            {
                foreach (DIDATuple tuple in lists[i])
                {
                    if (!intersectList.Contains(tuple))
                    {
                        intersectList.Remove(tuple);
                    }
                }
            }
            if (intersectList.Count() == 0) return null;
            Random ran = new Random();
            DIDATuple takeTuple = intersectList[ran.Next(0, intersectList.Count())];
            return takeTuple;
        }

        private List<DIDATuple> GetMatchingTuples(DIDATuple didaTuple)
        {
            List<DIDATuple> matchingTuples = new List<DIDATuple>();
            foreach (var entry in tupleSpace)
            {
                DIDATuple tuple = (DIDATuple)((List<object>)entry)[0];
                if (tuple.Matches(didaTuple)) matchingTuples.Add(tuple);
            }
            return matchingTuples;
        }

        public override List<DIDATuple> SendMatchingTuples(DIDATuple didaTuple)
        {
            List<DIDATuple> matchingTuples = GetMatchingTuples(didaTuple);
            Utils.Print($" [*] Sending Matching Tuples for {didaTuple.ToString()}. Found {matchingTuples.Count()}.", verbose: Verbose);
            return matchingTuples;
        }
        #endregion
        #endregion

        #region Helpers
        private void PrintTuplesListsList(List<List<DIDATuple>> tupleList)
        {
            Utils.Print($" [*] SMR Server Leader: Printing List of Tuple Lists ({tupleList.Count()} lists).", verbose: Verbose);
            int counter1 = 1;
            foreach (List<DIDATuple> list in tupleList)
            {
                Utils.Print($" [*] SMR Server Leader: List has {list.Count()} elements", verbose: Verbose);
                int counter2 = 1;
                foreach (DIDATuple tuple in list)
                {
                    Utils.Print($" [*] SMR Server Leader: List {counter1}, Element {counter2++} -> {tuple.ToString()}.", verbose: Verbose);
                }
                counter1++;
            }
        }
        #endregion

        #region Leader Methods
        internal void SendTotalOrder(string clientId, int requestId)
        {
            bool goingNewView = false;
            mIdTable[clientId] = requestId;
            TotalOrderData totalorderData = new TotalOrderData(requestId, sg);
            sg += 1;

            List<ServerData> replicasList = viewManager.GetKnownReplicas();
            foreach (ServerData serverData in replicasList)
            {
                string serverProxyURL = $"tcp://{serverData.ServerURL}/{serverData.ServerName}";
                try
                {
                    ISMRServerService serverProxy = (ISMRServerService)Activator.GetObject(typeof(ISMRServerService), serverProxyURL);
                    serverProxy.ReceiveTotalOrder(totalorderData);
                }
                catch (RemotingException)
                {
                    goingNewView = true;
                    break;
                }
                catch (SocketException)
                {
                    goingNewView = true;
                    break;
                }
            }
            if (goingNewView)
            {
                viewManager.TryViewChange(viewManager.GetKnownReplicas());
            }
            Utils.Print("[*] SMR Server Sequencer: Sent TotalOrderData.");

            CheckHoldbackSequencerQueue(); //Checks if there are any messages than can be attributed now.
        }

        internal void CheckHoldbackSequencerQueue()
        {
            foreach (int requestId in holdbackQueueSequencer.Keys)
            {
                RequestData requestData = holdbackQueueSequencer[requestId];
                int lastClientMessageId = mIdTable[requestData.ClientId];

                if (requestId == lastClientMessageId + 1)
                {
                    holdbackQueueSequencer.Remove(requestId);
                    SendTotalOrder(requestData.ClientId, requestId);
                    return;
                }
            }
        }
        #endregion
    }
}
