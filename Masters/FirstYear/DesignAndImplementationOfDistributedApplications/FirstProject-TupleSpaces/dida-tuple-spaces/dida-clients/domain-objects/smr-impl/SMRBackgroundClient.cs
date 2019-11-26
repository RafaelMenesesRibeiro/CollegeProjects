using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using dida_clients.data_objects;
using dida_contracts.data_objects;
using dida_contracts.data_objects.reply_data_types;
using dida_contracts.domain_objects;
using dida_contracts.helpers;
using dida_contracts.web_services;

namespace dida_clients.domain_objects.smr_impl
{
    class SMRBackgroundClient : BackgroundClient
    {
        private int requestCounter;
        private int responseCounter;
        private List<SMRResponse> responseList;
        ManualResetEvent allResponsesEvent = new ManualResetEvent(false);


        #region Constructors
        public SMRBackgroundClient(int port, View view = null) : base(port, view)
        {
            Console.WriteLine(" [*] Constructing Background Client of Type SMR");
            requestCounter = -1;
            responseList = new List<SMRResponse>();
        }
        #endregion

        #region Server methods
        private void CallServers(DIDATuple tuple, EOperationType requestOperation)
        {
            Utils.Print("[*] Requesting servers...", verbose: Verbose);
            RequestData requestData = new RequestData(++requestCounter, backgroundClientView.ViewId, ClientId, $"tcp://localhost:{ClientPort}", tuple, requestOperation);
            List<ServerData> replicasList = backgroundClientView.ReplicasList;
            allResponsesEvent.Reset();

            int replicasCount = replicasList.Count;
            var tasks = new Task<ReplyData>[replicasCount];
            for (int i = 0; i < replicasCount; i++)
            {
                ServerData serverData = replicasList[i];
                tasks[i] = Task.Run(() => ExecuteRemoteOperation(serverData, requestData));
            }
            Task.WaitAny(tasks);
            HandleResponses(requestData);
        }
        
        private ReplyData ExecuteRemoteOperation(ServerData serverData, RequestData requestData)
        {
            string serverProxyURL = $"tcp://{serverData.ServerURL}/{serverData.ServerName}";
            try
            {
                IServerService serverProxy = (IServerService)Activator.GetObject(typeof(IServerService), serverProxyURL);
                return serverProxy.Request(requestData);
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

        public override void ReceiveAnswer(ServerData serverData, ReplyData replyData)
        {
            Utils.Print(" [*] Received server answer...", verbose: Verbose);
            if (responseCounter > 0)
            {
                SMRResponse response = new SMRResponse(serverData, replyData);
                responseList.Add(response);
                responseCounter--;

                if (responseCounter == 0) allResponsesEvent.Set(); // Signals all the responses arrived.
            }
            else Utils.Print(" [*] Ignored server answer.", verbose: Verbose);
        }

        public void HandleResponses(RequestData requestData)
        {
            Utils.Print(" [*] Blocked waiting for signal.", verbose: Verbose);
            allResponsesEvent.WaitOne(); // Waits for the signal of when all responses arrive.
            Utils.Print(" [*] Received all answers.", verbose: Verbose);

            foreach (SMRResponse resp in responseList)
            {
                string serverId = resp.serverData.ServerName;
                ReplyData answer = resp.replyData;
                Utils.Print($"  [*] Server {serverId} answered {answer.ToString()}.", verbose: Verbose);

                if (answer.GetType() == typeof(TupleReply)) {
                    TupleReply tr = (TupleReply)answer;
                    Utils.Print($"    [*] Tuple Reply: {tr.Tuple.ToString()}.", verbose: Verbose);
                }
            }
            responseList.Clear();
            Utils.Print(" >> Success");
        }
        #endregion
        
        #region Tuple Space methods
        public override void Write(DIDATuple tuple)
        {
            Utils.Print(" [*] Waiting for response from server for <WRITE> operation...");
            int replicasCount = backgroundClientView.ReplicasList.Count;
            responseCounter = replicasCount; // The SMR Server Sequencer doesn't answer, but is in the view.
            CallServers(tuple, EOperationType.Write);
        }
        public override void Read(DIDATuple tuple)
        {
            Utils.Print(" [*] Waiting for response from server for <READ> operation...");
            responseCounter = 1; // The SMR Server Sequencer doesn't answer, but is in the view.
            CallServers(tuple, EOperationType.Read);
        }
        public override void Take(DIDATuple tuple)
        {
            Utils.Print(" [*] Waiting for response from server for <TAKE> operation...");
            int replicasCount = backgroundClientView.ReplicasList.Count;
            responseCounter = replicasCount; // The SMR Server Sequencer doesn't answer, but is in the view.
            CallServers(tuple, EOperationType.Take);
        }
        #endregion
    }
}
