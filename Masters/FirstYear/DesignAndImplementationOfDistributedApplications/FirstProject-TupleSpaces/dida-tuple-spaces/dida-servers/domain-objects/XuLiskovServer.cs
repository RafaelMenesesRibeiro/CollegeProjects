using dida_contracts.helpers;
using dida_contracts.data_objects;
using dida_contracts.data_objects.reply_data_types;
using dida_contracts.domain_objects;
using System.Collections.Generic;
using System;

namespace dida_servers.domain_objects
{
    class XuLiskovServer : Server
    {
        public XuLiskovServer(int channelPort, string serverName, int minDelay, int maxDelay, View view, bool verbose) : 
            base(channelPort, serverName, minDelay, maxDelay, view, verbose)
        {  }

        internal override ReplyData HandleRequest(RequestData requestData, int requestNum)
        {
            Random random = new Random();
            int delayValue = random.Next(minDelay, maxDelay);
            System.Threading.Thread.Sleep(delayValue);

            while(freezed || requestNum < currentReq)
            {
                System.Threading.Thread.Sleep(200);
            }

            currentReq++;

            if (CheckOldMsg(requestData)) return new DiscardedMsgReply(requestData.RequestId);
            if (CheckOldView(requestData)) return new ViewProposal(viewManager.GetView(), ServerData);

            switch (requestData.Operation)
            {
                case EOperationType.Read:
                    System.Console.WriteLine("[*] XuLiskov Server: Read operation");
                    mIdTable[requestData.ClientId] = requestData.RequestId;
                    return Read(requestData);
                case EOperationType.Take:
                    System.Console.WriteLine("[*] XuLiskov Server: Take operation");
                    mIdTable[requestData.ClientId] = requestData.RequestId;
                    return Take(requestData);
                case EOperationType.Write:
                    System.Console.WriteLine("[*] XuLiskov Server: Write operation");
                    mIdTable[requestData.ClientId] = requestData.RequestId;
                    return Write(requestData);
                case EOperationType.Lock:
                    System.Console.WriteLine("[*] XuLiskov Server: Lock operation");
                    mIdTable[requestData.ClientId] = requestData.RequestId;
                    return Lock(requestData);
                case EOperationType.Unlock:
                    System.Console.WriteLine("[*] XuLiskov Server: Unlock operation");
                    mIdTable[requestData.ClientId] = requestData.RequestId;
                    return Unlock(requestData);
                default:
                    System.Console.WriteLine("[*] XuLiskov Server: Invalid operation");
                    return null;
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
                DIDATuple tuple = (DIDATuple) ((List<object>) entry)[0];
                if (tuple.Matches(requestData.TupleData)) {
                    return new TupleReply(requestData.RequestId, tuple);
                }
            }

            return new NoTupleFoundReply(requestData.RequestId);
        }

        internal ReplyData Take(RequestData requestData)
        {
            foreach (var entry in tupleSpace)
            {
                DIDATuple tuple = (DIDATuple)((List<object>)entry)[0];
                string workerLock = (string)((List<object>)entry)[1];

                if (workerLock == null) continue;

                // If locked by client
                if (workerLock.Equals(requestData.ClientId))
                {
                    // If it is the tuple to remove, removes it from the tupleSpace, else removes the lock
                    if (tuple.Matches(requestData.TupleData))
                    {
                        object toRemove = entry;
                        tupleSpace.TryTake(out toRemove);
                        return new AckReply(requestData.RequestId);
                    }
                    else
                    {
                        object toRemoveLock = entry;
                        if (tupleSpace.TryPeek(out toRemoveLock))
                            ((List<object>)toRemoveLock)[1] = null;
                    }
                }
            }

            return new AckReply(requestData.RequestId);
        }

        internal ReplyData Lock(RequestData requestData)
        {
            List<DIDATuple> tupleSetToSend = new List<DIDATuple>();
            bool refused = false;


            foreach (var entry in tupleSpace)
            {
                DIDATuple tuple = (DIDATuple)((List<object>)entry)[0];
                string workerLock = (string)((List<object>)entry)[1];

                // If unlocked and it matches the template it locks the tuple
                if (tuple.Matches(requestData.TupleData))
                {
                    if (workerLock != null)
                    {
                        refused = true;
                        continue;
                    }

                    object toLock = entry;
                    if (tupleSpace.TryPeek(out toLock))
                    {
                        ((List<object>)toLock)[1] = requestData.ClientId;
                        tupleSetToSend.Add(tuple);
                    }
                }
            }

            if (refused) return new LockRefusedReply(requestData.RequestId);
            return new TupleSetReply(requestData.RequestId, tupleSetToSend);
        }

        internal ReplyData Unlock(RequestData requestData)
        {
            foreach (var entry in tupleSpace)
            {
                DIDATuple tuple = (DIDATuple)((List<object>)entry)[0];
                string workerLock = (string)((List<object>)entry)[1];

                // If locked by client and it is the tuple to unlock, removes the lock
                if (workerLock.Equals(requestData.ClientId) && tuple.Matches(requestData.TupleData))
                { 
                    object toRemoveLock = entry;
                    if (tupleSpace.TryPeek(out toRemoveLock))
                        ((List<object>)toRemoveLock)[1] = null;
                }
            }

            return new AckReply(requestData.RequestId);
        }
        
        // Does nothing, only here because it was needed for SMR and the interface is shared.
        internal override void ReceiveTotalOrder(TotalOrderData totalorderData) => throw new System.NotImplementedException();
        // Does nothing, only here because it was needed for SMR and the interface is shared.
        public override void ReceiveTakeTuple(RequestData takeCapsuleData) => throw new System.NotImplementedException();
        // Does nothing, only here because it was needed for SMR and the interface is shared.
        public override List<DIDATuple> SendMatchingTuples(DIDATuple didaTuple) => throw new System.NotImplementedException();
    }
}
