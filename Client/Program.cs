using System;
using Client.Client;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Wat is het IPAdress?: ");
            string IPAddress = Console.ReadLine();
            int port;
            while (true)
            {
                try
                {
                    Console.WriteLine("En de port?: ");
                    port = int.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Dat was geen correcte port, de port moet een getal zijn");
                }
            }

            ClientSocket client = new ClientSocket(IPAddress, port);
            client.Start();
        }
    }
}
