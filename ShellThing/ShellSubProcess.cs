using System;
using System.Diagnostics;
using System.Threading;

namespace ShellThing
{
    public class ShellSubProcess
    {
        public Process Process { get; }

        public ShellSubProcess(string shellType)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = shellType;
            startInfo.UseShellExecute = false;
            startInfo.ErrorDialog = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            Process = new Process();
            Process.StartInfo = startInfo;
        }

        public void StartProcess()
        {

            Process.Start();
        }

        public void TerminateProcess()
        {
            Process.Kill();
        }

        //TODO: Add ability to 'background' process
    }
}
