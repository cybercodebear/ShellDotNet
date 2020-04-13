using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ShellThing
{
    // Delegate definition used for events to receive notification
    // of text read from stdout or stderr of a running process
    public delegate void StringReadEventHandler(string text);

    class ProcessIoManager
    {
        private Process runningProcess;

        private Thread stdoutThread;
        private Thread stderrThread;

        private StringBuilder streamBuffer;

        public bool ContinueWriteToStdIn { get;  set; }

        private TcpReverseConnection activeConnection;

        public Process RunningProcess 
        { 
            get {  return runningProcess; } 
        }

        public event StringReadEventHandler StdoutTextRead;

        public event StringReadEventHandler StderrTextRead;

        public ProcessIoManager(Process process, TcpReverseConnection connection)
        {
            if (process == null)
            {
                throw new NullReferenceException("ProcessIoManager unable to set running porocess");
            }

            activeConnection = connection;
            runningProcess = process;
            streamBuffer = new StringBuilder(256);
            ContinueWriteToStdIn = true;
        }

        public void StartProcessOutputRead()
        {
            // Make sure there aren't previous threads running
            StopMonitoringProcessOutput();

            // Start the thread to read stdout in the background
            stdoutThread = new Thread(new ThreadStart(ReadStandardOutputThreadMethod));
            stdoutThread.IsBackground = true;
            stdoutThread.Start();

            stderrThread = new Thread(new ThreadStart(ReadStandardErrorThreadMethod));
            stderrThread.IsBackground = true;
            stderrThread.Start();
        }

        private void ReadStream(int firstCharRead, StreamReader streamReader, bool isstdout)
        {

            // Lock this to prevent multiple threads (Stdout and Stderr) attempting to read from the same stream simultaneously
            // If threads were reading from the same stream simultaneously, output text would be jumbled with characters from each stream
            lock (this)
            {
                // Single character read from either stdout or stderr
                int ch;

                streamBuffer.Length = 0;

                streamBuffer.Append((char)firstCharRead);

                // While there are more characters to be read...
                while (streamReader.Peek() > -1)
                {
                    ch = streamReader.Read();

                    streamBuffer.Append((char)ch);

                    // Send text one line at a time
                    if (ch == '\n')
                    {
                        NotifyAndFlushBufferText(streamBuffer, isstdout);
                    }
                }

                NotifyAndFlushBufferText(streamBuffer, isstdout);
            }
        }

        private void NotifyAndFlushBufferText(StringBuilder textBuffer, bool isstdout)
        {
            if (textBuffer.Length > 0)
            {
                if (isstdout == true && StdoutTextRead != null)
                {
                    // Send notification of text read from stdout
                    StdoutTextRead(textBuffer.ToString());
                }
                else if (isstdout == false && StderrTextRead == null)
                {
                    StderrTextRead(textBuffer.ToString());
                }

                // Clear the text buffer
                textBuffer.Length = 0;
            }
        }

        private void ReadStandardOutputThreadMethod()
        {
            try
            {
                int character;

                // Read() will block until something is available
                while ((character = runningProcess.StandardOutput.Read()) > -1)
                {
                    ReadStream(character, runningProcess.StandardOutput, true);
                }
            }
            catch (Exception e)
            {
                activeConnection.SendData($"ProcessIoManager.ReadStandardOutputThreadMethod() - Exception {e.Message}");
            }
        }

        private void ReadStandardErrorThreadMethod()
        {
            try
            {
                int ch;

                while (runningProcess != null && (ch = runningProcess.StandardError.Read()) > -1)
                {
                    ReadStream(ch, runningProcess.StandardError, false);
                }
            }
            catch (Exception e)
            {
                activeConnection.SendData($"ProcessIoManager.ReadStandardErrorThreadMethod Exception {e.Message}");
            }
        }

        public void WriteStdIn()
        {
            string inputText = activeConnection.ReceiveData();

            if (runningProcess != null && runningProcess.HasExited == false && !string.IsNullOrEmpty(inputText))
            {
                runningProcess.StandardInput.WriteLine(inputText);
            }
        }

        public void StopMonitoringProcessOutput()
        {
            // Stop the stdout reader thread
            try
            {
                if (stdoutThread != null)
                {
                    stdoutThread.Abort();
                }
            }
            catch (ThreadAbortException e)
            {
                activeConnection.SendData($"ProcessIoManager.StopMonitoringProcessOutput() exception: {e.Message}");
            }

            // Stop the stderr reader thread
            try
            {
                if (stderrThread != null)
                {
                    stderrThread.Abort();
                }
            }
            catch (ThreadAbortException e)
            {
                activeConnection.SendData($"ProcessIoManager.StopMonitoringProcessOutput() exception: {e.Message}");
            }
        }
    }
}
