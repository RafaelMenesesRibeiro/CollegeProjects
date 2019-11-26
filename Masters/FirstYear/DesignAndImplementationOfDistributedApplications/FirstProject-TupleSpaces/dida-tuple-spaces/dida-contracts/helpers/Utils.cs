using dida_contracts.data_objects;
using dida_contracts.domain_objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dida_contracts.helpers
{
    public static class Utils
    {
        public static string GenerateUniqueId()
        {
            Guid guid = Guid.NewGuid();

            string uniqueId = Convert.ToBase64String(guid.ToByteArray());

            uniqueId = uniqueId.Replace("=", "");
            uniqueId = uniqueId.Replace("+", "");
            uniqueId = uniqueId.Replace("\\", "");

            return uniqueId;
        }

        public static bool IsValidRemoteReply(Task<ReplyData> task, Type replyType)
        {
            if (task == null) return false;
            else return task.Status == TaskStatus.RanToCompletion && task.IsCanceled == false && task.Result.GetType().Equals(replyType);
        }

        public static bool EveryoneHasAccepted(int acceptCount, int replicasCount)
        {
            return acceptCount == replicasCount;
        }

        public static bool MajorityHasAccepted(int acceptCount, int replicasCount)
        {
            if (replicasCount == 0) return false;
            return (acceptCount / replicasCount) >= 0.5;
        }

        public static void Print(string message, bool verbose = true)
        {
            if (verbose)
            {
                Console.WriteLine(message);
            }
        }

        public static bool IsOtherViewBetter(View myView, View otherView)
        {
            return otherView.ViewId > myView.ViewId;
        }

        public static bool IsOtherViewWorse(View myView, View otherView)
        {
            return otherView.ViewId < myView.ViewId;
        }
    }
}
