using System;
using System.Net;
using Server.Game.DataStructures;
using Shared;
using Server.Server;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Use local connection? [y/n]");
            string input = Console.ReadLine().Trim();
            string host;
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            if (input.ToLower() == "y")
            {
                host = "127.0.0.1";
            }
            else
            {
                Console.WriteLine("toepasselijke IPAdressen zijn als volgt: ");
                for (int i = 0; i < addresses.Length; i++)
                {
                    Console.WriteLine($"{i + 1}: {addresses[i]}");
                }
                while(true)
                {
                    try
                    {
                        string inp = Console.ReadLine();
                        int inpI = int.Parse(inp);
                        host = addresses[inpI-1].ToString();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Dat was geen geldige input!");
                    }
                }
            }
            int port;
            while (true)
            {
                try
                {
                    Console.WriteLine("Op welke port wil je het hosten?");
                    port = int.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.WriteLine("Dat was geen correcte port, de port moet een getal zijn");
                }
            }
            // create a socket
            ServerSocket socket = new ServerSocket(host, port);
            socket.Start();
        }
    }
}
