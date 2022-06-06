﻿using System;
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
            // add the player to this scene and to the global game
            first.AddplayerToScene(player);
            AddPlayer(player);
            UpdateStatus(player.scene);
            try
            {
                if (player == players[1])
                {
                    players[0].Move(2, 0);
                }
            }
            catch { }

            // update the this scene
            UpdatePlayers(player.scene);
            return player;
        }

        private void GameLoop(Player player)
        {
            Console.WriteLine(players.Count);
            //Scene currentScene = player.scene;
            //SendMessage(player.socket, currentScene.GameField);
        }

        internal static string GetMessage(Socket client)
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

        internal static void SendMessage(Socket client, string message)
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

        /// <summary>
        /// Updates the players in the scene.</br>
        /// This method also removes all the disconnected players. </br>
        /// Because of this, this is the safe way to update all the players in a specific scene.
        /// </summary>
        /// <param name="scene">The scene's players you want to update</param>
        private void UpdatePlayers(Scene scene)
        {
            List<Player> disconnected = scene.Update();
            Console.WriteLine("Disconnected count: " + disconnected.Count);
            
            foreach(Player disconnect in disconnected)
            {
                players.Remove(disconnect);
            }
        }
        /// <summary>
        /// Does what UpdatePlayers() does, except that it doesn't send the players the GameField
        /// </summary>
        /// <param name="scene">The scene whose status you want to update</param>
        private void UpdateStatus(Scene scene)
        {
            List<Player> disconnected = scene.UpdateStatus();
            Console.WriteLine("Disconnected count: " + disconnected.Count);
            foreach(Player disconnect in disconnected)
            {
                players.Remove(disconnect);
            }
        }
        private bool AddPlayer(Player player)
        {
            if(players.Contains(player))
            {
                return false;
            }
            players.Add(player);
            return true;
        }
    }
}