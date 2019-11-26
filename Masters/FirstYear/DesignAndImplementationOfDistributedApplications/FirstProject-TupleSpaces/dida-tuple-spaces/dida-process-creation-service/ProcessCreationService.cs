using dida_contracts.web_services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace dida_process_creation_service
{
    class ProcessCreationService : MarshalByRefObject, IProcessCreationService
    {

        private static readonly int PCSPORT = 10000;
        private static readonly string PCSNAME = "PCS";
        private TcpChannel channel = new TcpChannel(PCSPORT);

        static void Main(string[] args)
        {
            ProcessCreationService pcs = new ProcessCreationService();
            ChannelServices.RegisterChannel(pcs.channel, false);
            RemotingServices.Marshal(pcs, PCSNAME, typeof(ProcessCreationService));
            Console.ReadKey();
        }

        public void Server(string name, int port, string urlName, int minDelay, int maxDelay, string mode)
        {
            string args = $"{mode} {port} {urlName} {minDelay} {maxDelay}";
            System.Diagnostics.Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}/dida-servers.exe", args);
        }

        public void Client(string name, int port, string urlName, string scriptPath, string mode)
        {
            string args = $"{mode} {port} {scriptPath}";
            System.Diagnostics.Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}/dida-clients.exe", args);
        }
    }
}
