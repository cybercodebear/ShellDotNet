using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellThing
{
    class SystemInfoCommand : ICommand
    {
        TcpReverseConnection connection;

        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {
            this.connection = connection;
            
            throw new NotImplementedException();
        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            throw new NotImplementedException();
        }
    }
}
