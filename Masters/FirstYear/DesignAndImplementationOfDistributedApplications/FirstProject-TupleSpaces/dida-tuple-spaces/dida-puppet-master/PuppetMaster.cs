using dida_contracts.web_services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace dida_puppet_master
{
    class PuppetMaster
    {
        private Dictionary<string, string> name2URL = new Dictionary<string, string>();
        private readonly int PCSPORT = 10000;
        private readonly string PCSNAME = "PCS";
        private string mode;

        static void Main(string[] args)
        {
            PuppetMaster pm = new PuppetMaster();

            if (args.Count() < 1)
            {
                PrintUsage();
            }

            if (args[0].Equals("XL") || args[0].Equals("SMR"))
            {
                pm.mode = args[0];
            } else {
                PrintUsage();
            }

            if (args.Count() > 1)
            {
                pm.ReadScript(args[1], args.Contains("-s"));
            }

            while (true)
            {
                try
                {
                    pm.ExecuteCommand(Console.ReadLine());
                } catch (Exception exc)
                {
                    Console.WriteLine($"[x] Error during input processing: {exc.Message}");
                }
            }
        }

        private void ExecuteCommand(string command)
        {
            if (command.Equals("Quit"))
            {
                Quit();
            } else if (command.StartsWith("Server"))
            {
                Server(command);
            } else if (command.StartsWith("Client"))
            {
                Client(command);
            } else if (command.Equals("Status"))
            {
                Status();
            } else if (command.StartsWith("Crash"))
            {
                Crash(command);
            } else if (command.StartsWith("Freeze"))
            {
                Freeze(command);
            } else if (command.StartsWith("Unfreeze"))
            {
                Unfreeze(command);
            } else if (command.StartsWith("Wait"))
            {
                Wait(command);
            } else
            {
                throw new Exception($"Unknown command '{command}'");
            }
        }

        private static void Quit()
        {
            Environment.Exit(0);
        }

        private void Server(string command)
        {
            string[] args = command.Split(' ');
            if (args.Count() != 5) throw new Exception("Wrong format for Server command");
            try
            {
                string URL = args[2];
                string machineIP = ExtractIP(URL);
                string serverPort = ExtractPort(URL);
                string serverName = ExtractName(URL);

                Console.WriteLine(URL);
                Console.WriteLine(machineIP);
                Console.WriteLine(serverPort);
                Console.WriteLine(serverName);

                string pcsURL = $"tcp://{machineIP}:{PCSPORT}/{PCSNAME}";
                IProcessCreationService pcs = (IProcessCreationService)Activator.GetObject(typeof(IProcessCreationService), pcsURL);
                pcs.Server(args[1], Int32.Parse(serverPort), serverName, Int32.Parse(args[3]), Int32.Parse(args[4]), mode);

                name2URL.Add(args[1], URL);
            } catch (Exception exc)
            {
                Console.WriteLine($"[x] Error during server creation: {exc.Message}");
            }
        }

        private void Client(string command)
        {
            string[] args = command.Split(' ');
            if (args.Count() != 4) throw new Exception("Wrong format for Client command");
            try
            {
                string URL = args[2];
                string machineIP = ExtractIP(URL);
                string clientPort = ExtractPort(URL);
                string clientName = ExtractName(URL);

                Console.WriteLine(URL);
                Console.WriteLine(machineIP);
                Console.WriteLine(clientPort);
                Console.WriteLine(clientName);

                string pcsURL = $"tcp://{machineIP}:{PCSPORT}/{PCSNAME}";
                IProcessCreationService pcs = (IProcessCreationService)Activator.GetObject(typeof(IProcessCreationService), pcsURL);
                pcs.Client(args[1], Int32.Parse(clientPort), clientName, args[3], mode);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"[x] Error during client creation: {exc.Message}");
            }
        }

        private void Status()
        {
            foreach (KeyValuePair<string, string> entry in name2URL)
            {
                IServerService server = (IServerService)Activator.GetObject(typeof(IServerService), entry.Value);
                server.Status();
            }
        }

        private void Crash(string command)
        {
            string[] args = command.Split(' ');
            if (args.Count() != 2) throw new Exception("Wrong format for Wait command");
            string serverURL = name2URL[args[1]];

            IServerService sever = (IServerService)Activator.GetObject(typeof(IServerService), serverURL);
            name2URL.Remove(args[1]);
            sever.Crash();
        }

        private void Freeze(string command)
        {
            string[] args = command.Split(' ');
            if (args.Count() != 2) throw new Exception("Wrong format for Freeze command");
            string serverURL = name2URL[args[1]];

            IServerService sever = (IServerService)Activator.GetObject(typeof(IServerService), serverURL);
            sever.Freeze(args[1]);
        }

        private void Unfreeze(string command)
        {
            string[] args = command.Split(' ');
            if (args.Count() != 2) throw new Exception("Wrong format for Unfreeze command");
            string serverURL = name2URL[args[1]];

            IServerService sever = (IServerService)Activator.GetObject(typeof(IServerService), serverURL);
            sever.Unfreeze(args[1]);
        }

        private void Wait(string command)
        {
            string[] args = command.Split(' ');
            if (args.Count() != 2) throw new Exception("Wrong format for Wait command");
            Thread.Sleep(Int32.Parse(args[1]));
        }

        private void ReadScript(string filePath, bool stepByStep)
        {
            string line;
            try
            {
                using (StreamReader fileReader = new StreamReader(@filePath))
                {
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        if (line.Equals("")) continue;
                        Console.WriteLine($"[*] Reading command: {line}");
                        ExecuteCommand(line);
                        Console.WriteLine($"[*] Done...{Environment.NewLine}");
                        if (stepByStep)
                        {
                            Console.WriteLine($"[*] Press <Enter> to go to the next command in the file");
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"[x] Error during script processing: {exc.Message}");
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine($"Usage: .\\dida-puppet-master.exe XL|SMR [<path_to_script> [-s]]");
            Environment.Exit(0);
        }

        private static string ExtractIP(string URL)
        {
            return URL.Split('/')[2].Split(':')[0]; 
        }

        private static string ExtractPort(string URL)
        {
            return URL.Split('/')[2].Split(':')[1];
        }

        private static string ExtractName(string URL)
        {
            return URL.Split('/')[3];
        }
    }
}
