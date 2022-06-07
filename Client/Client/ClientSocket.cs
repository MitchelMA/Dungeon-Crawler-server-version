using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client.Client
{
    internal class ClientSocket
    {
        internal Socket client;
        private readonly IPEndPoint remoteEp;
        private const short header = 64;
        private bool startedInput = false;
        private bool StopInput = false;

        internal ClientSocket(string host, int port, bool ipv4)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            IPAddress iPAddress = hostEntry.AddressList[Convert.ToInt32(ipv4)];
            remoteEp = new IPEndPoint(iPAddress, port);

            client = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        internal void Start()
        {
            client.Connect(remoteEp);

            Console.WriteLine($"Connected to server: {remoteEp.Address}:{remoteEp.Port}");
            while (true)
            {
                if (!client.Connected)
                {
                    Console.WriteLine("Disconnected from the server");
                    StopInput = true;
                    break;
                }
                try
                {
                    Console.WriteLine(client.Connected);
                    string data = GetMessage();
                    if(data != null)
                    {
                        Console.WriteLine(data);
                        // after the first message, 
                        // the input loop may start
                        InitiateInput();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Er ging iets mis met de verbinding");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    StopInput = true;
                    break;
                }
            }
        }

        /// <summary>
        /// This method starts the input-loop
        /// if the input-loop is running, it won't start twice
        /// </summary>
        private void InitiateInput()
        {
            if(startedInput)
            {
                return;
            }

            startedInput = true;

            Thread inputLoop = new Thread(InputLoop);
            inputLoop.Start();
        }

        private void InputLoop()
        {
            while(true)
            {
                if (StopInput)
                {
                    StopInput = false;
                    startedInput = false;
                    break;
                }

                // wait for user input
                string input = Console.ReadLine();
                SendMessage(input);
            }
        }

        internal string GetMessage()
        {
            string data = null;
            byte[] bytes = new byte[header];

            int bytesRec = client.Receive(bytes);
            if (bytesRec > 0)
            {
                int messageSize = Convert.ToInt32(Encoding.UTF8.GetString(bytes, 0, bytesRec));
                bytes = new byte[messageSize];

                bytesRec = client.Receive(bytes);
                data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }

            return data;
        }

        internal void SendMessage(string message)
        {
            int len = Encoding.UTF8.GetByteCount(message);
            string messageLength = len.ToString();
            for (int i = len.ToString().Length; i < header; i++)
            {
                messageLength += " ";
            }

            client.Send(Encoding.UTF8.GetBytes(messageLength));
            client.Send(Encoding.UTF8.GetBytes(message));
        }
    }
}
