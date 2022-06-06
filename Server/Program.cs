using System;
using Server.Game.DataStructures;
using Shared;
using Server.Server;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // create a socket
            ServerSocket socket = new ServerSocket("localhost", 5500, false);
            socket.Start();
        }
    }
}
