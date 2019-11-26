using System;
using dida_contracts.exceptions;
using dida_contracts.web_services;
using dida_contracts.helpers;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Collections.Generic;
using dida_contracts.domain_objects;
using dida_contracts.data_objects;
using System.Threading.Tasks;
using dida_contracts.data_objects.reply_data_types;

namespace dida_clients.domain_objects
{
    public abstract class BackgroundClient : MarshalByRefObject, IClientService
    {
        #region Fields

        private const int NO_VIEW = -1;
        private const string NO_LEADER = "no_leader";

        protected View backgroundClientView;

        protected TcpChannel tcpChannel;
        protected ObjRef internalRef;

        protected string ClientPort { get; }
        protected string ClientId { get; }

        #endregion
        
        #region Properties

        public bool Verbose { get; set; } = true;

        #endregion

        public BackgroundClient(int port, View view = null)
        {
            if (view == null)
            {
                backgroundClientView = new View(NO_VIEW, NO_LEADER, new List<ServerData>());
            } else
            {
                backgroundClientView = view;
            }

            ClientPort = port.ToString();
            ClientId = Utils.GenerateUniqueId();

            Hashtable channelProperties = new Hashtable() {
                { "name", ClientId },
                { "port", ClientPort }
            };

            tcpChannel = new TcpChannel(channelProperties, null, null);
            ChannelServices.RegisterChannel(tcpChannel, false);

            internalRef = RemotingServices.Marshal(this, ClientId, typeof(BackgroundClient));
        }

        #region View Change Methods

        protected bool TryUpdateView(Task<ReplyData> task)
        {
            if (Utils.IsValidRemoteReply(task, typeof(ViewProposal)))
            {
                backgroundClientView = ((ViewProposal)task.Result).ServerView;
                return true;
            }
            return false;
        }

        #endregion

        #region Methods not invokable through remote calls
        public virtual void Read(DIDATuple template)
        {
            throw new NotImplementedException("Base BackgroundClient does not implement a read. Use a typed background client method or implement the called method.");
        }

        public virtual void Write(DIDATuple tuple)
        {
            throw new NotImplementedException("Base BackgroundClient does not implement a write. Use a typed background client method or implement the called method.");
        }

        public virtual void Take(DIDATuple template)
        {
            throw new NotImplementedException("Base BackgroundClient does not implement a take. Use a typed background client method or implement the called method.");
        }
        #endregion

        #region IClientService Implementation

        public virtual void recieveNewView() { throw new NotImplementedException(); }
        public virtual void haltRequests() { throw new NotImplementedException(); }
        public abstract void ReceiveAnswer(ServerData serverData, ReplyData replyData);

        #endregion
    }
}
