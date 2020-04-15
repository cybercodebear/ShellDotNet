using System;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    /// <summary>
    /// Prints help for all or a particular set of commands stored in the CommandInvoker's 'commands' hashtable. 
    /// </summary>
    class HelpCommand : ICommand
    {
        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {

        }
    }
}
