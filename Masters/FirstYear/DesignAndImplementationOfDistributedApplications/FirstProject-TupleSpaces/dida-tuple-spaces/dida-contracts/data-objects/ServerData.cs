using System;
using System.Runtime.Serialization;
using dida_contracts.domain_objects;

namespace dida_contracts.data_objects
{
    [Serializable]
    public class ServerData : ISerializable, IComparable
    {
        #region Fields
        public string ServerName { get; }
        public string ServerURL { get; }
        public string ServerUId { get; }
        #endregion

        #region Constructors
        public ServerData(string serverId, string serverURL, string uid)
        {
            ServerName = serverId;
            ServerURL = serverURL;
            ServerUId = uid;
        }
        #endregion

        #region Serialization Methods
        public ServerData(SerializationInfo info, StreamingContext context)
        {
            ServerName = (string)info.GetValue("ServerId", typeof(string));
            ServerURL = (string)info.GetValue("ServerURL", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ServerId", ServerName);
            info.AddValue("ServerURL", ServerURL);
        }
        #endregion

        #region Overrided Methods

        public override bool Equals(object obj)
        {
            var item = obj as ServerData;

            if (item == null)
            {
                return false;
            }
            else if (ServerName.Equals(item.ServerName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{ServerUId} @ {ServerURL}/{ServerName}";
        }

        public override int GetHashCode()
        {
            return ServerName.GetHashCode();

        }

        public static bool operator <(ServerData left, ServerData right)
        {
            return (Compare(left, right) < 0);
        }

        public static bool operator >(ServerData left, ServerData right)
        {
            return (Compare(left, right) > 0);
        }

        public static int Compare(ServerData left, ServerData right)
        {
            if (left.Equals(right)) return 0;
            else if (left.Equals(null)) return -1;
            else if (right.Equals(null)) return 1;
            return -string.Compare(left.ServerName, right.ServerName, StringComparison.OrdinalIgnoreCase);
            
        }

        public int CompareTo(object obj)
        {
            if (obj.Equals(null)) return 1;
            ServerData rigth = obj as ServerData;
            if (rigth.Equals(null)) throw new ArgumentException("A RatingInformation object is required for comparison.", "obj");
            return this.CompareTo(rigth);
        }

        public int CompareTo(ServerData rigth)
        {
            if (object.ReferenceEquals(rigth, null)) return 1;
            return string.Compare(this.ServerName, rigth.ServerName, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}