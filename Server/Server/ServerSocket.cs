using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Console.WriteLine(scenes.First().Value.GameField);
        }

        internal void Start()
        {
            // load in all the scenes and corresponding data
            GameSetup();

            listener.Bind(endPoint);

            listener.Listen();

            ListenForClients();
        }

        private void ListenForClients()
        {
            Console.WriteLine($"[Listening]: Server started listening on: {endPoint.Address}:{endPoint.Port}");
            while(true)
            {
                Socket client = listener.Accept();
                Console.WriteLine($"[New Connection]: {client.RemoteEndPoint}");
                Player player = SetupPlayer(client);

                Thread loop = new Thread(() => GameLoop(player));
                loop.Start();
            }
        }

        private Player SetupPlayer(Socket client)
        {
            PlayerStructure playerStructure = Serializer.Deserialize<PlayerStructure>(Path.Combine(@".\game-data\", gameReference.ItemDataPath, @"player.json"));
            Scene first = scenes.First().Value;
            Player player = new Player(playerStructure.Hp, first.beginPosition, playerStructure.Damage, playerStructure.XpNeededNext, first, client);
            first.AddplayerToScene(player);
            return player;
        }

        private void GameLoop(Player player)
        {
            Scene currentScene = player.scene;
            SendMessage(player.socket, currentScene.GameField);
        }

        private static string GetMessage(Socket client)
        {
            string data = null;
            byte[] bytes = new byte[header];

            int bytesRec = client.Receive(bytes);
            if(bytesRec > 0)
            {
                int messageSize = Convert.ToInt32(Encoding.UTF8.GetString(bytes, 0, bytesRec));
                bytes = new byte[messageSize];

                bytesRec = client.Receive(bytes);
                data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }

            return data;
        }

        private static void SendMessage(Socket client, string message)
        {
            string messageLength;

            int len = Encoding.UTF8.GetByteCount(message);
            messageLength = len.ToString();
            for(int i = len.ToString().Length; i < header; i++)
            {
                messageLength += " ";
            }

            Console.WriteLine(Encoding.UTF8.GetByteCount(message));

            // send the length of the message and the message respectively
            client.Send(Encoding.UTF8.GetBytes(messageLength));
            client.Send(Encoding.UTF8.GetBytes(message));
        }
    }
}
