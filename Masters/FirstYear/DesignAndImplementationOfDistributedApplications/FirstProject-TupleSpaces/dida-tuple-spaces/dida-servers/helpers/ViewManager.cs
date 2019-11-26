using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using dida_contracts.data_objects;
using dida_contracts.data_objects.reply_data_types;
using dida_contracts.domain_objects;
using dida_contracts.helpers;
using dida_contracts.web_services;
using dida_servers.domain_objects;

namespace dida_servers.helpers
{
    public class ViewManager
    {
        private static readonly int timeout = 2000;
        private Server managerOwner;
        private ServerScanner serverScanner;
        private View serverView;
        private bool selfPromotionIsAllowed = true;
        private object __selfPromotionAllowedBoolLocker = new object();

        public ViewManager(View view, Server owningServer, bool forcefulJoin = false)
        {
            managerOwner = owningServer;
            serverView = view;
            serverScanner = new ServerScanner(this);

            if (forcefulJoin)
            {
                serverScanner.TryUpdateViewState(view.ReplicasList);
            }
        }

        public List<ServerData> GetKnownReplicas()
        {
            return serverView.ReplicasList;
        }

        public View GetView()
        {
            return serverView;
        }

        public View TryViewChange(List<ServerData> knownLivingServers)
        {
            // TODO >> When servers accept a commit message they should end all their requests, alt incomming requests and wait for a commit.
            // TODO >> After commit is done they should resume the requests
            int cancelled = 0;
            AllowSelfPromotion();

            Utils.Print(" [o] Proposing new View to known living servers...");

            #region Initialization

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(timeout);
            CancellationToken cancellationToken = tokenSource.Token;

            // Generate the new View.
            serverView.IncrementViewId();
            serverView.PromoteViewManager(managerOwner.ServerData);
            serverView.ReplicasList = knownLivingServers;

            List<AcceptViewProposal> underlingsList = new List<AcceptViewProposal>();
            Task<ReplyData>[] tasksArray = InitiateServerCalls(serverView, cancellationToken);

            try
            {
                Task.WaitAll(tasksArray, cancellationToken);
            }
            catch (OperationCanceledException) { cancelled++; }

            bool aServerExplicitlyRefused = false;

            for (int tidx = 0; tidx < tasksArray.Length; tidx++)
            {
                Task<ReplyData> task = tasksArray[tidx];

                if (Utils.IsValidRemoteReply(task, typeof(AcceptViewProposal)))
                {
                    underlingsList.Add((AcceptViewProposal)task.Result);
                }
                else if (Utils.IsValidRemoteReply(task, typeof(RejectViewProposal)))
                {
                    aServerExplicitlyRefused = true;
                }
                else
                {
                    knownLivingServers.Remove(((NoReply)task.Result).ServerData);
                }
            }

            #endregion

            if (Utils.MajorityHasAccepted(underlingsList.Count, tasksArray.Length - cancelled))
            {
                AcceptViewProposal bestProposal = SelectBestInitialState(underlingsList);
                TryCommitView(underlingsList, bestProposal);
            }
            else if (!Utils.MajorityHasAccepted(underlingsList.Count, tasksArray.Length) || aServerExplicitlyRefused)
            {
                return TryViewChange(knownLivingServers);     
            }

            return this.serverView;
        }

        private void TryCommitView(List<AcceptViewProposal> underlingsList, AcceptViewProposal bestState)
        {
            if (IsSelfPromotionAllowed())
            {
                Utils.Print($" [o] Commiting View Change...");
                Task<ReplyData>[] tasksArray = InitiateServerCalls(underlingsList);
                // We don't care about those replicas after informing them they should commit. If they fail, someone will detect it sooner or later.
            }
            AllowSelfPromotion();
        }

        #region Remote Calls Handling
        public ReplyData RecieveViewChangeProposal(View proposedView)
        {
            Utils.Print($" [o] Received View Change Proposal.");
            // TODO - Move Server's ReceiveViewChangeProposal to here.
            if (Utils.IsOtherViewBetter(serverView, proposedView))
            {
                throw new NotImplementedException();
            }
            else
            {
                return new RejectViewProposal(managerOwner.ServerData, serverView);
            }
        }
        #endregion

        #region Remoting methods

        private Task<ReplyData>[] InitiateServerCalls(List<AcceptViewProposal> underlingsList)
        {
            Task<ReplyData>[] tasksArray = new Task<ReplyData>[underlingsList.Count];

            for (int ridx = 0; ridx < underlingsList.Count; ridx++)
            {
                ServerData destServer = underlingsList[ridx].ServerData;
                Dictionary<string, int> messageIdTable = underlingsList[ridx].MessageIdTable;
                ConcurrentBag<object> tupleSpaceObjects = underlingsList[ridx].TupleSpaceObjects;

                Task.Run(() => CallServer(destServer, this.serverView, messageIdTable, tupleSpaceObjects));
            }

            return tasksArray;
        }

        private Task<ReplyData>[] InitiateServerCalls(View serverView, CancellationToken token)
        {
            List<ServerData> knownLivingReplicas = serverView.ReplicasList;
            Task<ReplyData>[] tasksArray = new Task<ReplyData>[knownLivingReplicas.Count];

            for (int ridx = 0; ridx < knownLivingReplicas.Count; ridx++)
            {
                ServerData destServer = knownLivingReplicas[ridx];
                tasksArray[ridx] = Task.Run( () => CallServer(destServer, this.serverView, token), token);
            }

            return tasksArray;
        }

        private ReplyData CallServer(ServerData destServer, View proposedView, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            IServerService server_proxy;
            try
            {
                server_proxy = (IServerService)Activator.GetObject(typeof(IServerService), $"tcp://{destServer.ServerURL}/{destServer.ServerName}");
                return server_proxy.RecieveViewChangeProposal(proposedView);
            }
            catch (RemotingException)
            {
                return new NoReply(destServer , - 1);
            }
            catch (SocketException)
            {
                return new NoReply(destServer, - 2);
            }
        }

        private void CallServer(ServerData destServer, View viewToCommit, Dictionary<string, int> msgTable, ConcurrentBag<object> objects)
        {
            IServerService server_proxy;
            try
            {
                server_proxy = (IServerService)Activator.GetObject(typeof(IServerService), $"tcp://{destServer.ServerURL}/{destServer.ServerName}");
                server_proxy.CommitViewChangeProposal(viewToCommit, msgTable, objects);
            }
            catch (RemotingException)
            {
                return; // Don't care about failed servers after telling them to commit. We will know they failed with pigs.
            }
            catch (SocketException)
            {
                return;
            }
        }

        #endregion

        #region Helper Methods

        private AcceptViewProposal SelectBestInitialState(List<AcceptViewProposal> underlingsList)
        {
            AcceptViewProposal bestProposal = underlingsList[0];
            foreach (AcceptViewProposal underlingProposal in underlingsList)
            {
                View currentUnderlingView = underlingProposal.ServerView;
                if (Utils.IsOtherViewBetter(bestProposal.ServerView, currentUnderlingView))
                {
                    bestProposal = underlingProposal;
                }
            }
            return bestProposal;
        }

        public void DisallowSelfPromotion()
        {
            lock (__selfPromotionAllowedBoolLocker)
            {
                selfPromotionIsAllowed = false;
            }
        }

        private bool IsSelfPromotionAllowed()
        {
            lock(__selfPromotionAllowedBoolLocker)
            {
                return selfPromotionIsAllowed;
            }
        }

        private void AllowSelfPromotion()
        {
            lock(__selfPromotionAllowedBoolLocker)
            {
                selfPromotionIsAllowed = true;
            }
        }
        
        #endregion
    }
}
