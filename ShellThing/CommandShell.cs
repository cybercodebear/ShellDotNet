using System;
using System.Diagnostics;
using System.Threading;

namespace ShellThing
{
    public delegate void ProcessTerminatedEventHandler(object source, EventArgs e);

    public class CommandShell
    {
        public Process Process { get; }

        public event ProcessTerminatedEventHandler ProcessTerminated;

        private Thread processMonitorThread;

        public CommandShell(string shellType)
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

        private void StartProcessStatusMonitor()
        {
            // Make sure there aren't previous threads running
            StopProcessStatusMonitor();

            // If the process has exited, trigger the OnProcessTerminated(); event to notify ProcessIoManager
            while (true)
            {
                if (Process == null || Process.HasExited == true)
                {
                    OnProcessTerminated();
                }
            }
        }

        private void StopProcessStatusMonitor()
        {
            // Abort the process monitor thread if the process has been terminated
            if (processMonitorThread.IsAlive)
            {
                processMonitorThread.Abort();
            }
        }

        public void StartProcess()
        {

            Process.Start();

            // Start a thread to monitor for process termination so that I/O can be redirected to the normal shell
            processMonitorThread = new Thread(StartProcessStatusMonitor);
            processMonitorThread.IsBackground = true;
            processMonitorThread.Start();
        }

        public void TerminateProcess()
        {
            // processMonitorThread should detect that the process has exited and trigger OnProcessTerminated();
            Process.Kill();
            OnProcessTerminated();
        }

        public virtual void OnProcessTerminated()
        {
            StopProcessStatusMonitor();

            if (ProcessTerminated != null)
            {
                ProcessTerminated(this, EventArgs.Empty);
            }
        }

        //TODO: Add ability to 'background' process
    }
}
