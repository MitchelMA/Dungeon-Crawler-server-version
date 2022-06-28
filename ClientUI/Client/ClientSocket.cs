using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Shared;
using Shared.Security;

namespace ClientUI.Client
{
    internal class ClientSocket
    {
        internal Socket client;
        private readonly IPEndPoint remoteEp;
        private const short header = 64;


        // data security class for keys
        DataSecurity dataSecurity;

        // reference to the form
        ClientUIForm form;

        internal ClientSocket(string host, int port, ClientUIForm form)
        {
            dataSecurity = new DataSecurity();
            
            // form of the programme
            this.form = form;

            // setup for the client socket
            IPAddress iPAddress = IPAddress.Parse(host);
            remoteEp = new IPEndPoint(iPAddress, port);

            client = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        internal void Connect()
        {
            client.Connect(remoteEp);
            SetupPlayer();

            while(true)
            {
                try
                {
                    string data = GetMessage();
                    if (data != null)
                    {
                        string decrypted = dataSecurity.DecryptAES(data);
                        form.SetupField(decrypted); 
                        //break;
                    }
                }
                catch
                {
                    client.Shutdown(SocketShutdown.Both);
                    break;
                }
            }
        }

        internal void SetupPlayer()
        {
            // first send the public key
            SendMessage(dataSecurity.PublicXmlKey);

            // then we should get the encrypted key end iv
            string encryptedKey = GetMessage();
            string encryptedIV = GetMessage();

            // now we can decrypt the key and iv
            byte[] decryptedKey = Convert.FromBase64String(dataSecurity.DecryptRSA(encryptedKey));
            byte[] decryptedIV = Convert.FromBase64String(dataSecurity.DecryptRSA(encryptedIV));

            // now setup the symmetric key
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

        internal void Close()
        {
            client.Disconnect(false);
        }
    }
}
