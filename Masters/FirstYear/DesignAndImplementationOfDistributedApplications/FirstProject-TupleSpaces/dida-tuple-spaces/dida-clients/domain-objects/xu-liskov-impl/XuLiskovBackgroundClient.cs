using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using dida_clients.helpers;
using dida_contracts.data_objects;
using dida_contracts.data_objects.reply_data_types;
using dida_contracts.domain_objects;
using dida_contracts.helpers;
using dida_contracts.web_services;
using Newtonsoft.Json;

namespace dida_clients.domain_objects.xu_liskov_impl
{
    public class XuLiskovBackgroundClient : BackgroundClient
    {
        #region Fields and Constructors

        private static readonly int timeout = 15000;
        private static Random randomGenerator = new Random();
        private int requestCounter;

        public XuLiskovBackgroundClient(int port, View view = null) : base(port, view)
        {
            Utils.Print(" [*] Constructing Background Client of Type XuLiskov", verbose: false);
            requestCounter = 0;
        }

        #endregion

        #region General Methods

        public void ConsumeRequest(DIDATuple tuple, EXuLiskovOperation operation)
        {
            Utils.Print(" [*] Recieved new request...", verbose: Verbose);
            EOperationType requestOperation = GetPrimitiveOperationType(operation);
            RequestData requestData = new RequestData(++requestCounter, backgroundClientView.ViewId, ClientId, $"tcp://localhost:{ClientPort}", tuple, requestOperation);
            XuLiskovRequest request = new XuLiskovRequest(requestData, operation);
            ExecuteConsumption(request, operation);
        }

        private void ExecuteConsumption(XuLiskovRequest request, EXuLiskovOperation operation)
        {
            if (request == null) { return; }

            Utils.Print($" [*] Consuming operation of type: {operation}...", verbose: Verbose);

            if (operation == EXuLiskovOperation.Write)
            {
                Write(request);
            }
            else if (operation == EXuLiskovOperation.Read)
            {
                Read(request);
            }
            else
            {
                Take(request, operation);
            }
        }

        private Task<ReplyData>[] InitiateServerCalls(List<ServerData> replicasList, XuLiskovRequest request, CancellationToken cancellationToken)
        {
            int replicasCount = replicasList.Count;
            Task<ReplyData>[] tasksArray = new Task<ReplyData>[replicasCount];

            for (int ridx = 0; ridx < replicasCount; ridx++)
            {
                Thread.Sleep(50);
                ServerData serverData = replicasList[ridx];
                RequestData requestData = request.RequestData;
                tasksArray[ridx] = Task.Run(() => CallServer(serverData, requestData, cancellationToken), cancellationToken);
            }

            return tasksArray;
        }

        private ReplyData CallServer(ServerData serverData, RequestData requestData, CancellationToken token)
        {
            IServerService server_proxy;
            token.ThrowIfCancellationRequested();

            Utils.Print($" [*] Asking {serverData.ServerName} to execute request...");

            try
            {
                server_proxy = (IServerService)Activator.GetObject(typeof(IServerService), $"tcp://{serverData.ServerURL}/{serverData.ServerName}");
                return server_proxy.Request(requestData);
            }
            catch (RemotingException)
            {
                return new NoReply(serverData, -1);
            }
            catch (SocketException)
            {
                return new NoReply(serverData, -2);
            }      
        }

        #endregion

        #region Tuple Space methods

        public void Write(XuLiskovRequest request)
        {
            int tasksCount;

            List<ServerData> replicasList = backgroundClientView.ReplicasList;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(timeout);
            CancellationToken cancellationToken = tokenSource.Token;

            Utils.Print($" [*] Waiting servers for <WRITE> Acknowledge...");

            Task<ReplyData>[] tasksArray = InitiateServerCalls(replicasList, request, cancellationToken);

            try
            {
                Task.WaitAll(tasksArray, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                RepeatOperation(EOperationType.Write, request, reason: "failed to recieve at least one Acknowledge");
                return;
            }

            tasksCount = replicasList.Count;
            for (int tidx = 0; tidx < tasksCount; tidx++)
            {
                TryUpdateView(tasksArray[tidx]);

                if (!Utils.IsValidRemoteReply(tasksArray[tidx], typeof(AckReply)))
                {
                    RepeatOperation(EOperationType.Write, request, reason: "failed to recieve at least one Acknowledge");
                }
            }

            Utils.Print(" >> Success");
        }

        public void Read(XuLiskovRequest request)
        {
            int tidx;
            int tasksCount;

            List<ServerData> replicasList = backgroundClientView.ReplicasList;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(timeout);
            CancellationToken cancellationToken = tokenSource.Token;

            Utils.Print($" [*] Waiting servers for <READ> Acknowledge...");

            Task<ReplyData>[] tasksArray = InitiateServerCalls(replicasList, request, cancellationToken);

            try
            {
                tasksCount = replicasList.Count;
                while (tasksCount > 0)
                {
                    tidx = Task.WaitAny(tasksArray, cancellationToken);

                    TryUpdateView(tasksArray[tidx]);

                    if (Utils.IsValidRemoteReply(tasksArray[tidx], typeof(TupleReply)))
                    {          
                        Utils.Print($" >> Success");
                        Utils.Print($" >>> {JsonConvert.SerializeObject(((TupleReply)tasksArray[tidx].Result).Tuple, Formatting.Indented)}");
                        return;
                    }

                    tasksArray[tidx] = new Task<ReplyData>(NoAction);

                    tasksCount--;
                    if (tasksCount == 0)
                    {
                        RepeatOperation(EOperationType.Read, request, reason: "no match found");
                        return;
                    }      
                }
            }
            catch (OperationCanceledException)
            {
                RepeatOperation(EOperationType.Read, request, "operation reached timeout");
                return;
            }
        }

        public void Take(XuLiskovRequest request, EXuLiskovOperation operation)
        {
            List<ServerData> replicasList = backgroundClientView.ReplicasList;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(timeout);
            CancellationToken cancellationToken = tokenSource.Token;

            Utils.Print($" [*] Waiting servers for <TAKE - PHASE 1> Acknowledge...");

            Task<ReplyData>[] tasksArray = InitiateServerCalls(replicasList, request, cancellationToken);

            if (operation == EXuLiskovOperation.TakeOne)
            {
                TakeOne(tasksArray, request, cancellationToken);
            }
            else if (operation == EXuLiskovOperation.TakeTwo)
            {
                TakeTwo(tasksArray, request, cancellationToken);
            }
            else if (operation == EXuLiskovOperation.Unlock)
            {
                TakeUnlock(tasksArray, request, cancellationToken);
            }
            else
            {
                Utils.Print($" [x] Error: Unable to identify <TAKE> phase...");
            }
        }

        private void TakeOne(Task<ReplyData>[] tasksArray, XuLiskovRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<DIDATuple> tmpSet = new List<DIDATuple>();

            try
            {
                Task.WaitAll(tasksArray, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                RepeatOperation(EOperationType.Take, request, reason: "one or more servers timed out");
                return;
            }

            int goodResponseCount = 0;
            int badResponseCount = 0;
            int tasksCount = tasksArray.Length;

            for (int tidx = 0; tidx < tasksCount; tidx++)
            {
                if (TryUpdateView(tasksArray[tidx]))
                {
                    RepeatOperation(EOperationType.Take, request, reason: "client in wrong view");
                }

                if (!Utils.IsValidRemoteReply(tasksArray[tidx], typeof(TupleSetReply)))
                {
                    badResponseCount++;
                }
                else
                {
                    ReplyData replyData = tasksArray[tidx].Result;
                    IEnumerable<DIDATuple> resultingSet = ((TupleSetReply)replyData).TupleSet;
                    List<DIDATuple> intersectSet = new List<DIDATuple>();
                    if (tidx == 0)
                    {
                        tmpSet = resultingSet;
                    }
                    else
                    {
                        foreach (DIDATuple tupleRes in resultingSet)
                        {
                            foreach (DIDATuple tupleTmpSet in tmpSet)
                            {
                                if (tupleTmpSet.Equals(tupleRes))
                                {
                                    intersectSet.Add(tupleTmpSet);
                                }
                            } 
                        }
                        tmpSet = intersectSet;
                    }
                    goodResponseCount++;
                }
            }

            DIDATuple[] setIntersection = Enumerable.ToArray(tmpSet);
            int setIntersectionCount = setIntersection.Length;
            bool majorityAccepted = Utils.MajorityHasAccepted(goodResponseCount, tasksCount);
            bool everyoneAccepted = Utils.EveryoneHasAccepted(goodResponseCount, tasksCount);

            if ((majorityAccepted && !everyoneAccepted) || (everyoneAccepted && setIntersectionCount == 0))
            {
                RepeatOperation(EOperationType.Take, request, reason: "at least a server failed to send a response set");
                return;
            }
            else if (!majorityAccepted)
            {
                Utils.Print($" >> failed locking on <TAKE-PHASE1>, initiating <TAKE-UNLOCK>...");

                request.Operation = EXuLiskovOperation.Unlock;
                Take(request, EXuLiskovOperation.Unlock);

                Thread.Sleep(randomGenerator.Next(500, 1000));

                request.Operation = EXuLiskovOperation.TakeOne;
                RepeatOperation(EOperationType.Take, request, reason: "only a minority of servers accepted <TAKE-ONE> request, retrying soon");
                return;
            }
            else
            {
                Utils.Print($" >> Success on <TAKE-PHASE1>, initiating <TAKE-PHASE2>...");
                DIDATuple selectedTuple = setIntersection[randomGenerator.Next(setIntersectionCount)];
                Utils.Print(" >> Locked:");
                Utils.Print(JsonConvert.SerializeObject(selectedTuple, Formatting.Indented));
                RequestData requestData = new RequestData(++requestCounter, backgroundClientView.ViewId, ClientId, $"tcp://localhost:{ClientPort}", selectedTuple, EOperationType.Take);
                XuLiskovRequest requestTake = new XuLiskovRequest(requestData, EXuLiskovOperation.TakeTwo);

                Take(requestTake, EXuLiskovOperation.TakeTwo);
            }
        }

        private void TakeTwo(Task<ReplyData>[] tasksArray, XuLiskovRequest request, CancellationToken cancellationToken)
        {
            TakeWaitAll(tasksArray, request, cancellationToken, isTakeTwo: true);
        }

        private void TakeUnlock(Task<ReplyData>[] tasksArray, XuLiskovRequest request, CancellationToken cancellationToken)
        {
            TakeWaitAll(tasksArray, request, cancellationToken, isTakeTwo: false);
        }

        private void TakeWaitAll(Task<ReplyData>[] tasksArray, XuLiskovRequest request, CancellationToken cancellationToken, bool isTakeTwo = true)
        {
            int tasksCount = tasksArray.Length;

            try
            {
                Task.WaitAll(tasksArray, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                RepeatOperation(EOperationType.Take, request, reason: "one or more servers timed out");
                return;
            }

            for (int tidx = 0; tidx < tasksCount; tidx++)
            {
                ReplyData replyData = tasksArray[tidx].Result;

                if (!Utils.IsValidRemoteReply(tasksArray[tidx], typeof(AckReply)))
                {
                    TryUpdateView(tasksArray[tidx]);
                    RepeatOperation(EOperationType.Take, request, $"at least a server failed to acknowledge <TAKE-{(isTakeTwo ? "PHASE2> commit" : "UNLOCK> request")}");
                    return;
                }
            }

            Utils.Print($" >> Success on <TAKE-{(isTakeTwo ? "PHASE2" : "UNLOCK> ")}>...");
        }

        #endregion

        #region Helpers and sometimes MARTELADA

        private EOperationType GetPrimitiveOperationType(EXuLiskovOperation operation)
        {
            if (operation == EXuLiskovOperation.Write)
            {
                return EOperationType.Write;
            }
            else if (operation == EXuLiskovOperation.Read)
            {
                return EOperationType.Read;
            }
            else if (operation == EXuLiskovOperation.TakeOne)
            {
                return EOperationType.Lock;
            }
            else if (operation == EXuLiskovOperation.TakeTwo)
            {
                return EOperationType.Take;
            } else
            {
                return EOperationType.Unlock;
            }
        }

        private void RepeatOperation(EOperationType operationType, XuLiskovRequest request, string reason = "reason not specified")
        {
            if (operationType == EOperationType.Write)
            {
                Utils.Print($" >> Retrying <WRITE> operation, {reason}...");
                Write(request);
            }
            else if (operationType == EOperationType.Read)
            {
                Utils.Print($" >> Retrying <READ> operation, {reason}...");
                Read(request);
            }
            else
            {
                Utils.Print($" >> Retrying <TAKE> operation, {reason}...");
                EXuLiskovOperation concreteOperation = request.Operation;
                Take(request, concreteOperation);
            }
        }

        private ReplyData NoAction() { return null; }

        public override void ReceiveAnswer(ServerData serverData, ReplyData replyData) {; }

        #endregion
    }
}
