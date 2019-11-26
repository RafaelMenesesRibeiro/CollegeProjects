using dida_contracts.data_objects;
using dida_contracts.domain_objects;
using dida_servers.domain_objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dida_servers
{
    class App
    {
        static void Main(string[] args)
        {
            if (args.Count() != 5 && args.Count() != 6)
            {
                Console.WriteLine($"Usage: .\\dida-servers XL|SMR port name min_delay max_delay");
                return;
            }
            bool verbose = true;
            if (args.Count() == 6) verbose = false;

            if (args[0].Equals("XL"))
            {
                Console.WriteLine($"[*] Server: XuLiskovServer running at tcp://localhost:{args[1]}/{args[2]} with delays between {args[3]} and {args[4]} ms");
                XuLiskovServer xu = new XuLiskovServer(Int32.Parse(args[1]), args[2], Int32.Parse(args[3]), Int32.Parse(args[4]), ReadInitialServers(), verbose);
            }
            else if (args[0].Equals("SMR"))
            {
                Console.WriteLine($"[*] Server: SMRServer running at tcp://localhost:{args[1]}/{args[2]} with delays between {args[3]} and {args[4]} ms");
                SMRServer smr = new SMRServer(Int32.Parse(args[1]), args[2], Int32.Parse(args[3]), Int32.Parse(args[4]), ReadInitialServers(), verbose);
            }
            else
            {
                Console.WriteLine($"Usage: .\\dida-servers [XL|SMR] port name min_delay max_delay");
            }

            Console.ReadLine();
        }

        private static View ReadInitialServers()
        {
            string serversDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/servers/";
            string filePath = $"{serversDirectory}/knownReplicas.json";

            Directory.CreateDirectory(serversDirectory);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[x] File {filePath} not found. Exiting...");
                Environment.Exit(-1);
            }

            using (StreamReader streamReader = new StreamReader(@filePath))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                List<ServerData> replicasList;
                JsonSerializer serializer = new JsonSerializer();
                replicasList = serializer.Deserialize<List<ServerData>>(jsonReader);

                Console.WriteLine("[*] Loading known replicas information...");
                foreach (ServerData server in replicasList)
                {
                    Console.WriteLine($" >> tcp://{server.ServerURL}/{server.ServerName}");
                }

                ServerData leaderData = replicasList[0];
                string leaderName = leaderData.ServerName;
                Console.WriteLine($"[*] LEADER NAME: {leaderName}");
                return new View(1, leaderData.ServerUId, replicasList);
            }
        }
    }
}
