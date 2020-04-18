using System;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    /// <summary>
    /// Interface for creating new commands. 
    /// </summary>
    public interface ICommand
    {
        void Execute(TcpReverseConnection connection, string[] commandArguments);

        Dictionary<string, string> Help(bool includeSyntax); 
    }
}
