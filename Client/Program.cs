using System;
using Client.Client;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientSocket client = new ClientSocket("localhost", 5500, false);
            client.Start();
        }
    }
}
