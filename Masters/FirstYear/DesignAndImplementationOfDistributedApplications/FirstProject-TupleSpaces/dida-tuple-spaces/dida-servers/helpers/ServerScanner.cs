using dida_contracts.data_objects;
using dida_contracts.data_objects.reply_data_types;
using dida_contracts.domain_objects;
using dida_contracts.helpers;
using dida_contracts.web_services;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Net.Sockets;

namespace dida_servers.helpers
{
    public class ServerScanner : IDisposable
    {
        #region Fields and Properties
        private static readonly int IM_ALIVE_REQUEST_ID = -1;
        private static readonly int timeout = 15000;
        private static readonly double period = 45000;
        private View viewState;
        private ViewManager viewManager;
        private System.Timers.Timer timer;
        #endregion

        #region Constructors

        public ServerScanner(ViewManager serverViewManager)
        {
            viewManager = serverViewManager;
            viewState = serverViewManager.GetView();
            timer = new System.Timers.Timer(period);
            timer.Elapsed += ImAlive;
            timer.AutoReset = true;
            timer.Enabled = true;  // start the timer implicitly
        }

        #endregion

        #region Inversion of Control Methods

        private void SetGroupMember(List<ServerData> knownLivingReplicas)
        {

        }

        #endregion

        #region Server Scanning Methods

        private void ImAlive(object sender, ElapsedEventArgs e)
        {
            Utils.Print("Sending ImAlive");

            StopScanning();

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;
            tokenSource.CancelAfter(timeout);
            cancellationToken = tokenSource.Token;

            Task<ReplyData>[] tasksArray = InitiateServerCalls(viewState, cancellationToken);

            try
            {
                Task.WaitAll(tasksArray, cancellationToken);
            }
            catch (OperationCanceledException) { ; } // Swallow Canceled Operations We Don't Care About That At This Point of the Code

            List<ServerData> knownLivingServers = new List<ServerData>();

            bool detectedFaultingServer = false;
            bool detectedBetterView = false;

            for (int tidx = 0; tidx < tasksArray.Length; tidx++)
            {
                Task<ReplyData> task = tasksArray[tidx];

                if (Utils.IsValidRemoteReply(task, typeof(ImAliveReply)))
                {
                    ImAliveReply calleeReply = (ImAliveReply)task.Result;
                    detectedFaultingServer = Utils.IsOtherViewWorse(viewState, calleeReply.ServerView);
                    detectedBetterView = Utils.IsOtherViewBetter(viewState, calleeReply.ServerView);   
                    knownLivingServers.Add(calleeReply.ServerData);
                }
                else
                {
                    // We if operation was canceled (we don't know the reason) or we have a NoReply datatype returned to us, we assume callee faulted.
                    detectedFaultingServer = true;
                }
            }

            if (ShouldUpdateView(detectedFaultingServer, detectedBetterView))
            {
                TryUpdateViewState(knownLivingServers);
            }

            StartScanning();
        }
 
        #endregion

        #region Remoting methods

        private Task<ReplyData>[] InitiateServerCalls(View serverView, CancellationToken cancellationToken)
        {
            List<ServerData> replicasList = serverView.ReplicasList;
            Task<ReplyData>[] tasksArray = new Task<ReplyData>[replicasList.Count];

            for (int ridx = 0; ridx < replicasList.Count; ridx++)
            {
                Thread.Sleep(50);
                ServerData serverData = replicasList[ridx];
                tasksArray[ridx] = Task.Run(() => CallServer(serverData, serverView, cancellationToken), cancellationToken);
            }
     
            return tasksArray;
        }

        private ReplyData CallServer(ServerData serverData, View serverView, CancellationToken cancellationToken)
        {
            IServerService server_proxy;

            try
            {
                server_proxy = (IServerService)Activator.GetObject(typeof(IServerService), $"tcp://{serverData.ServerURL}/{serverData.ServerName}");
                return server_proxy.RecieveImAlive();
            } catch (RemotingException)
            {
                return new NoReply(serverData, IM_ALIVE_REQUEST_ID);
            } catch (SocketException)
            {
                return new NoReply(serverData, IM_ALIVE_REQUEST_ID);
            }


        }

        public void TryUpdateViewState(List<ServerData> livingServers)
        {
            Utils.Print(" [o] Asking manager to update it's view...");
            viewState = viewManager.TryViewChange(livingServers);
        }

        #endregion

        #region IDisposable Implementation and Helpers

        public void StartScanning()
        {
            timer.Start();
        }

        private void StopScanning()
        {
            timer.Stop();
        }

        private bool ShouldUpdateView(bool detectedFaultingServer, bool detectedBetterView)
        {
            return detectedFaultingServer || detectedBetterView;
        }

        public void Dispose()
        {
            timer.Dispose();
        }
        #endregion
    }
}
