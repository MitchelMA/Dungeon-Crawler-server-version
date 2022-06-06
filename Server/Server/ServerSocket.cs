using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Server.Game;
using Server.Game.DataStructures;
using Shared;

namespace Server.Server
{
    internal class ServerSocket
    {
        private readonly Socket listener;
        private List<Player> players;
        private Dictionary<string, Scene> scenes;
        private readonly IPEndPoint endPoint;

        private const short header = 64;

        // info about the game, for instance: the reference.json
        private Reference gameReference;

        internal ServerSocket(string host, int port, bool ipv4)
        {
            players = new List<Player>();
            scenes = new Dictionary<string, Scene>();
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            IPAddress ipAddress = hostEntry.AddressList[Convert.ToInt32(ipv4)];
            endPoint = new IPEndPoint(ipAddress, port);

            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // now load the reference and the game files
            GameSetup();
        }

        /// <summary>
        /// Method to load in all the game-data and create all the scenes from the reference.json
        /// </summary>
        private void GameSetup()
        {
            gameReference = Serializer.Deserialize<Reference>(@"game-data\reference.json");
            foreach(string name in gameReference.Levels)
            {
                Scene scene = new Scene(gameReference, name);
                scenes.Add(scene.name, scene);
            }
            foreach(KeyValuePair<String, Scene> pair in scenes)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }
    }
}
