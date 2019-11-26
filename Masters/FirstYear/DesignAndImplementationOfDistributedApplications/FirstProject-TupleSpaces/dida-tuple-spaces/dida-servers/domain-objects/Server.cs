using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using dida_contracts.data_objects;
using dida_contracts.domain_objects;
using dida_contracts.helpers;
using dida_contracts.data_objects.reply_data_types;
using dida_servers.helpers;
using System.Threading.Tasks;

namespace dida_servers.domain_objects
{
    public abstract class Server
    {
        #region Fields and Properties

        private readonly object __acceptedViewLock = new object();
        private const int NO_VIEW = -1;
        private const string NO_LEADER = "no_leader";
        protected int currentReq = 0;

        protected readonly int minDelay;
        protected readonly int maxDelay;

        protected Dictionary<string, int> mIdTable; // Dictionary<RequestData, PreviousReply> 
        protected TcpChannel channel; // TCP Channels need to stop listening on ViewChange, likely, making it protected.
        protected ViewManager viewManager;
        protected ConcurrentBag<object> tupleSpace;
        private View acceptedView = new View(NO_VIEW, NO_LEADER, new List<ServerData>());
        private bool isWaitingForCommit = false;
        protected bool freezed = false;

        public ServerData ServerData { get; }
        public ServerService serverService;

        public bool Verbose { get; set; } = true;

        #endregion

        #region Constructors

        public Server(int serverPort, string name, int minDelay, int maxDelay, View view, bool verbose)
        {
            Verbose = verbose;
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
            mIdTable = new Dictionary<string, int>();
            tupleSpace = new ConcurrentBag<object>();
            ServerData = new ServerData(name, $"tcp://localhost:{serverPort}", Utils.GenerateUniqueId());

            serverService = new ServerService(this);

            channel = new TcpChannel(serverPort);
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(serverService, name, typeof(ServerService));
            viewManager = new ViewManager(view, this);
        }

        #endregion

        #region Request Handling
        internal abstract ReplyData HandleRequest(RequestData requestData, int requestNum);

        internal abstract void ReceiveTotalOrder(TotalOrderData totalorderData);

        public abstract void ReceiveTakeTuple(RequestData takeCapsuleData);

        public abstract List<DIDATuple> SendMatchingTuples(DIDATuple didaTuple);
        #endregion

        #region View Change
        internal ReplyData RecieveImAlive()
        {
            Utils.Print("[o] Recieved ImAlive Event...");
            return new ImAliveReply(ServerData, viewManager.GetView());
        }

        internal void CommitViewChangeProposal(View serverView, Dictionary<string, int> messageIdTable, ConcurrentBag<object> tupleSpaceObjects)
        {
            if (!isWaitingForCommit || acceptedView == null) return;

            int proposedViewId = serverView.ViewId;
            // If a commit for a View that has been surpassed arrives, ignore it.
            lock (__acceptedViewLock)
            {
                if (proposedViewId != acceptedView.ViewId) return;
            }
    
            isWaitingForCommit = false;
            this.mIdTable = messageIdTable;
            this.tupleSpace = tupleSpaceObjects;
        }
        
        #region Underling Behaviour
        internal ReplyData RecieveViewChangeProposal(View proposedView)
        {
            Utils.Print("[o] Received View Change Proposal");
            View view = viewManager.GetView();
            int proposedViewId = proposedView.ViewId;

            Task irrelTask;
            if (ServerData.ServerUId.Equals(proposedView.ManagerUId) && view.ViewId < proposedView.ViewId)
            {
                viewManager.DisallowSelfPromotion();
                isWaitingForCommit = true;
                irrelTask = WaitForViewChangeCommitTimeout(proposedViewId, 4000);
                return new AcceptViewProposal(ServerData, view, mIdTable, tupleSpace);
            }
            
            if (view.ViewId >= proposedViewId) return new RejectViewProposal(ServerData, view);

            lock (__acceptedViewLock)
            {
                int acceptedViewId = acceptedView.ViewId;
                if (acceptedViewId >= proposedView.ViewId) return new RejectViewProposal(ServerData, view);
                acceptedView = proposedView;
            }

            viewManager.DisallowSelfPromotion();
            isWaitingForCommit = true;
            // Starts a timer to timeout in case a Commit for this particular proposed View doesn't arrive in time.
            // Ignores return value to stop warning.
            irrelTask = WaitForViewChangeCommitTimeout(proposedViewId, 4000);
            return new AcceptViewProposal(ServerData, view, mIdTable, tupleSpace);
        }

        private async Task WaitForViewChangeCommitTimeout(int accViewId, int timeout)
        {
            int startAcceptedViewId = accViewId;
            await Task.Delay(timeout);

            if (!isWaitingForCommit) return;

            lock (__acceptedViewLock)
            {
                int currentAccpetedViewId = acceptedView.ViewId;
                if (startAcceptedViewId != currentAccpetedViewId) return; // A better view was accepted while waiting for a commit for another view. Do nothing else.
            }

            isWaitingForCommit = false;

            List<ServerData> knownLivingServers = new List<ServerData>();
            viewManager.TryViewChange(knownLivingServers);
        }
        #endregion

        #region Puppet Master Methods
        internal void Unfreeze()
        {
            Console.WriteLine("[*] Server: Unfreeze");
            freezed = false;
        }

        internal void Freeze()
        {
            Console.WriteLine("[*] Server: Freeze");
            freezed = true;
        }

        internal void Status()
        {
            Console.WriteLine("[*] Server: Status");
            Console.WriteLine($"[*] @ {ServerData.ServerURL}/{ServerData.ServerName}");
            Console.WriteLine($"[*] Tuplespace count: {tupleSpace.Count}");
            Console.WriteLine(acceptedView.ToString());
        }

        internal void Crash()
        {
            Environment.Exit(0);
        }
        #endregion

        #endregion

        #region Leader Election
        // WARNING: SHOULD THIS REALLY BE HERE, XULISKOV HAS NO LEADER -> Consider moving to SMRServer
        internal void DetectedLeaderFailed() => throw new NotImplementedException();

        #endregion

        #region Helper Methods

        protected bool CheckOldMsg(RequestData requestData)
        {
            if (!mIdTable.ContainsKey(requestData.ClientId))
                mIdTable.Add(requestData.ClientId, -1);

            int mostRecentMsgId = mIdTable[requestData.ClientId];
            return requestData.RequestId <= mostRecentMsgId;
        }

        protected bool CheckOldView(RequestData requestData)
        {
            return requestData.ViewId < viewManager.GetView().ViewId;
        }

        #endregion
    }
}
