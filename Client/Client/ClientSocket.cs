using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Shared;
using Shared.Security;

namespace Client.Client
{
    internal class ClientSocket
    {
        internal Socket client;
        private readonly IPEndPoint remoteEp;
        private const short header = 64;
        private bool startedInput = false;
        private bool StopInput = false;

        // data security class for keys
        DataSecurity dataSecurity;

        internal ClientSocket(string host, int port)
        {
            // setup for the dataSecurity class
            dataSecurity = new DataSecurity();

            // setup for the client socket
            IPAddress iPAddress = IPAddress.Parse(host);
            remoteEp = new IPEndPoint(iPAddress, port);

            client = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        private bool TryConnection()
        {
            try
            {
                return client.Poll(10000, SelectMode.SelectRead);
            }
            catch
            {
                return false;
            }

        }

        internal void Start()
        {
            client.Connect(remoteEp);
            SetupPlayer();
            InitiateInput();

            Console.WriteLine($"Connected to server: {remoteEp.Address}:{remoteEp.Port}");
            while (true)
            {
                try
                {
                    string data = GetMessage();
                    if (data != null)
                    {
                        Console.Clear();
                        // now we can decrypt the message from the server
                        string decrypted = dataSecurity.DecryptAES(data);
                        Console.WriteLine(decrypted);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Er ging iets mis met de verbinding");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    client.Shutdown(SocketShutdown.Both);
                    StopInput = true;
                    break;
                }
                //if (TryConnection())
                //{
                //    Console.WriteLine("Disconnected from the server: Polling took too long");
                //    StopInput = true;
                //    break;
                //}
            }
        }

        /// <summary>
        /// This method starts the input-loop
        /// if the input-loop is running, it won't start twice
        /// </summary>
        private void InitiateInput()
        {
            if (startedInput)
            {
                return;
            }

            startedInput = true;

            Thread inputLoop = new Thread(InputLoop);
            inputLoop.Start();
        }

        private void InputLoop()
        {
            // setup for data security


            while (true)
            {
                if (StopInput)
                {
                    StopInput = false;
                    startedInput = false;
                    break;
                }

                // wait for user input
                ConsoleKey input = Console.ReadKey().Key;
                switch (input)
                {
                    case ConsoleKey.UpArrow:
                        SendMessage(dataSecurity.EncryptAES(Enum.GetName(typeof(Input), Input.up)));
                        break;
                    case ConsoleKey.RightArrow:
                        SendMessage(dataSecurity.EncryptAES(Enum.GetName(typeof(Input), Input.right)));
                        break;
                    case ConsoleKey.DownArrow:
                        SendMessage(dataSecurity.EncryptAES(Enum.GetName(typeof(Input), Input.down)));
                        break;
                    case ConsoleKey.LeftArrow:
                        SendMessage(dataSecurity.EncryptAES(Enum.GetName(typeof(Input), Input.left)));
                        break;
                    case ConsoleKey.Q:
                        client.Dispose();
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.Escape:
                        client.Dispose();
                        Environment.Exit(0);
                        break;

                    default:
                        SendMessage(dataSecurity.EncryptAES(Enum.GetName(typeof(Input), Input.ignore)));
                        break;
                }
                // sleep at least this amount
                // otherwise the server could get overloaded and in turn no client can poll the server, disconnecting all of them
                Thread.Sleep(50);
            }
        }

        private void SetupPlayer()
        {
            // first send the public key
            SendMessage(dataSecurity.PublicXmlKey);

            // then we should get the encrypted key and iv
            string encryptedKey = GetMessage();
            string encryptedIV = GetMessage();

            // now we can decrypt the keys
            byte[] decryptedKey = Convert.FromBase64String(dataSecurity.DecryptRSA(encryptedKey));
            byte[] decryptedIV = Convert.FromBase64String(dataSecurity.DecryptRSA(encryptedIV));

            // now set-up the symmetric key
            dataSecurity.SetAES(decryptedKey, decryptedIV);
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
