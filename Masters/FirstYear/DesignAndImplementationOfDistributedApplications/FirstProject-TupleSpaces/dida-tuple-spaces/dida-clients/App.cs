using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using dida_clients.domain_objects;
using dida_clients.domain_objects.xu_liskov_impl;
using dida_clients.domain_objects.smr_impl;
using dida_contracts.domain_objects;
using dida_contracts.data_objects;
using dida_clients.helpers;
using dida_contracts.helpers;


namespace dida_clients
{
    class App
    {
        #region Fields
        private static Client client = null;
        private static bool verbose = true;
        private static bool buildingBlock = false;
        private static int iterationsLimit = 0;
        private static List<string[]> iterationBlock = new List<string[]>();
        private static readonly string baseDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";
        private static readonly string testDirectory = $"{baseDirectory}/../../run/tests";
        private static readonly string serversDirectory = $"{baseDirectory}/servers";
        #endregion

        #region Main
        static void Main(string[] args)
        {
            string clientImplementation;
            string scriptPath;
            int clientPort = new Random().Next(1025, 65534);
            int argsCount = args.Length;

            if (argsCount != 3)
            {
                PrintUsage();
            }

            clientImplementation = args[0];
            clientPort = Int32.Parse(args[1]);
            scriptPath = args[2];

            List<ServerData> replicasList = ReadInitialServers();
            View view = new View(1, replicasList[0].ServerUId, replicasList);

            switch (clientImplementation)
            {
                case "XL":
                    XuLiskovBackgroundClient backgroundView = new XuLiskovBackgroundClient(clientPort, view);
                    client = new Client(EClientType.XLB, new XuLiskovForegroundClient(backgroundView));
                    break;
                case "SMR":
                    SMRBackgroundClient smrBackgroundClient = new SMRBackgroundClient(clientPort, view);

                    client = new Client(EClientType.SMR, new SMRForegroundClient(smrBackgroundClient));
                    break;
                default:
                    throw new ArgumentException("Invalid client type selected.");
            }

            if (argsCount > 1 && args[1].Equals("nv"))
            {
                verbose = false;
                client.SetVerbose(false);
            }

            ReadAndExecuteScript(scriptPath);
            Thread.Sleep(500000);
        }

        private static List<ServerData> ReadInitialServers()
        {
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

                Utils.Print("[*] Loading known replicas information...", verbose: true);

                foreach (ServerData server in replicasList)
                {
                    Utils.Print($" >> tcp://{server.ServerURL}/{server.ServerName}", verbose: true);
                }

                return replicasList;
            }
        }
        #endregion

        #region Script Handlin Methods
        private static void ReadAndExecuteScript(string scriptPath)
        {

            if (scriptPath.Equals("--a"))
            {
                string[] fileNamesArray = Directory.GetFiles($@"{testDirectory}", "*.txt").Select(Path.GetFileName).ToArray();
                foreach (string fileName in fileNamesArray)
                {
                    AnnounceScript(fileName);
                    ReadFile($"{testDirectory}/{fileName}");
                }
            }
            else
            {
                AnnounceScript(scriptPath);
                ReadFile($"{scriptPath}");
            }
        }

        private static void AnnounceScript(string input)
        {
            Utils.Print("###################################################", verbose: true);
            Utils.Print($"[*] Running test {input}...", verbose: true);
            Utils.Print("###################################################", verbose: true);
        }

        private static void ReadFile(string filePath)
        {
            string line;

            try
            {
                using (StreamReader fileReader = new StreamReader(@filePath))
                {
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        if (line.Equals(""))
                        {
                            continue;
                        }
                        Utils.Print($"[*] Reading command: {line}", verbose: verbose);
                        ParseLine(line);
                        Utils.Print($"[*] Done...{Environment.NewLine}", verbose: verbose);
                    }
                }
            }
            catch (Exception exc)
            {
                Utils.Print($"[x] Error during script processing: {exc.Message}{Environment.NewLine}{Environment.NewLine}{exc.StackTrace}", verbose: true);
            }
        }

        private static string FormatLine(string line)
        {
            Utils.Print($"[*] Formating...", verbose: verbose);
            StringBuilder stringBuilder = new StringBuilder();
            char[] temp = line.ToCharArray();
            bool objectParameters = false;
            bool inQuotes = false;
            int lineLenght = temp.Length;
            int index = 0;

            while (index < lineLenght)
            {
                // Marks next substring as parameters for object contructor
                if (temp[index].Equals('(') && !objectParameters)
                {
                    objectParameters = true;
                    stringBuilder.Append(temp[index]);
                }
                // Marks end of parameters for object contructor.
                else if (temp[index].Equals(')') && objectParameters)
                {
                    objectParameters = false;
                    stringBuilder.Append(temp[index]);
                }
                // If we see a comma that is not within an object constructor, escape it for split
                else if (temp[index].Equals(',') && !objectParameters)
                {
                    stringBuilder.Append($"\\{temp[index]}");
                }
                // One or two actions occur.
                else if (temp[index].Equals('"'))
                {
                    // First we determine if quote is opening or terminating a string.
                    inQuotes = !inQuotes;
                    // Then if that string is not an object parameter we we escape the quote so that is splited later.
                    if (!objectParameters)
                    {
                        stringBuilder.Append($"\\{temp[index]}");
                    }
                }
                // If a space is not in a quote, than its a space-seperator, so we trade that char with a ':' for easier splitting of the command.
                else if (temp[index].Equals(' ') && !inQuotes)
                {
                    stringBuilder.Append(':');
                }
                else
                {
                    stringBuilder.Append(temp[index]);
                }

                index++;
            }
            return stringBuilder.ToString();
        }

        private static void ParseLine(string line)
        {
            Utils.Print($"[*] Parsing...", verbose: verbose);

            line = FormatLine(line);

            string[] separatingChars = { ":", "<", "\\\"", "\\,", ">" };
            string[] command = line.Split(separatingChars, StringSplitOptions.RemoveEmptyEntries);

            if (buildingBlock && !command[0].Equals("end-repeat"))
            {
                iterationBlock.Add(command);
            }
            else
            {
                ExecuteCommand(command);
            }
        }

        private static void ExecuteCommand(string[] command)
        {
            switch (command[0])
            {
                case "add":
                    client.Write(command);
                    break;
                case "read":
                    client.Read(command);
                    break;
                case "take":
                    client.Take(command);
                    break;
                case "wait":
                    Thread.Sleep(Int32.Parse(command[1]));
                    break;
                case "begin-repeat":
                    buildingBlock = true;
                    iterationsLimit = Int32.Parse(command[1]);
                    break;
                case "end-repeat":
                    buildingBlock = false;
                    for (int iterations = 0; iterations < iterationsLimit; iterations++)
                    {
                        foreach (string[] repeatableCommand in iterationBlock)
                        {
                            Utils.Print($"[*] Looping command: {repeatableCommand[0]}, Pass: {iterations + 1}", verbose: verbose);
                            ExecuteCommand(repeatableCommand);
                            Utils.Print($"[*] Done...{Environment.NewLine}", verbose: verbose);
                        }
                    }
                    iterationsLimit = 0;
                    break;
                default:
                    Utils.Print("[x] Error during script processing. Hitted default case...", verbose: true);
                    break;
            }
        }
        #endregion

        #region Other App Methods

        private static void PrintUsage()
        {
            Console.WriteLine($"Usage: .\\dida-clients.exe XL|SMR <port> <path_to_script>");
            Environment.Exit(0);
        }

        #endregion
    }
}
