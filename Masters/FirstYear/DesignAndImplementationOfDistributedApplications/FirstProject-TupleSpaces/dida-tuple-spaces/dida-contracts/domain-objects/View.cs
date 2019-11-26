using System;
using System.Collections.Generic;
using dida_contracts.data_objects;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace dida_contracts.domain_objects
{
    [Serializable]
    public class View : ISerializable
    {
        #region Fields and Properties

        public List<ServerData> ReplicasList
        {
            get;
            set;
        }
        public int ViewId {
            get;
            set;
        }
        public string ManagerUId
        {
            get;
            set;
        }

        #endregion

        #region Constructor
        public View(int viewId, string serverUid, List<ServerData> replicasList)
        {
            ViewId = viewId;
            ManagerUId = serverUid;
            ReplicasList = replicasList;
        }
        #endregion

        #region Class Methods
        public void Add(ServerData serverData)
        {
            ReplicasList.Add(serverData);
        }

        public void Remove(ServerData serverData)
        {
            ReplicasList.Remove(serverData);
        }

        public ServerData Find(ServerData serverData)
        {
            return ReplicasList.Find(server => string.Equals(server.ServerName, serverData.ServerName));
        }

        public ServerData Find(string serverUId)
        {
            return ReplicasList.Find(server => string.Equals(server.ServerUId, serverUId));
        }

        public ServerData FindByName(string serverName)
        {
            return ReplicasList.Find(server => string.Equals(server.ServerName, serverName));
        }

        public void AddReplica(ServerData newReplica)
        {
            ReplicasList.Add(newReplica);
        }

        public void AddReplicas(List<ServerData> replicasList)
        {
            foreach (ServerData replica in replicasList)
            {
                ReplicasList.Add(replica);
            }
        }
        #endregion

        #region Helpers

        public void IncrementViewId()
        {
            ViewId++;
        }

        public void PromoteViewManager(ServerData serverData)
        {
            ManagerUId = serverData.ServerUId;
        }

        public bool Equals(View otherView)
        {
            if (otherView == null) { return false; }

            int otherViewId = otherView.ViewId;
            string otherManagerUId = otherView.ManagerUId;
            List<ServerData> otherReplicasList = otherView.ReplicasList;
            if (ViewId == otherViewId && ManagerUId == otherManagerUId && ReplicasList.Equals(otherReplicasList)) return true;
            return false;
        }

        public override string ToString()
        {
            string repString = "";
            foreach(ServerData server in ReplicasList)
            {
                repString += $"[*][*] {server.ToString()}\n";
            }
            if (ReplicasList.Count == 0) repString = "[*][*] No servers in this view";

            return $"[*] View id: {ViewId}\n[*] # Servers in View: {ReplicasList.Count}\n[*] Servers in View:\n{repString}";
        }

        #endregion

        #region Serialization Methods
        public View(SerializationInfo info, StreamingContext context)
        {
            ViewId = (int)info.GetValue("ViewId", typeof(int));
            ManagerUId = (string)info.GetValue("ManagerUId", typeof(string));
            ReplicasList = (List<ServerData>)info.GetValue("ReplicasList", typeof(List<ServerData>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ViewId", ViewId);
            info.AddValue("ReplicasList", ReplicasList);
            info.AddValue("ManagerUId", ManagerUId);
        }
        #endregion

    }
}

