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
using Server.Game.Items;
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
            foreach (string name in gameReference.Levels)
            {
                Scene scene = new Scene(gameReference, name);
                scenes.Add(scene.Name, scene);
            }
            //Console.WriteLine(scenes.First().Value.GameField);
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
            while (true)
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
            Player player = new Player(playerStructure.Hp, first.BeginPosition, playerStructure.Damage, playerStructure.XpNeededNext, first, client);
            // add the player to this scene and to the global game
            first.AddplayerToScene(player);
            AddPlayer(player);
            UpdateStatus(player.Scene);

            // update the this scene
            UpdatePlayers(player.Scene);
            return player;
        }

        private void GameLoop(Player player)
        {
            while (true)
            {
                UpdateStatus(player.Scene);
                if (!players.Contains(player))
                {
                    UpdatePlayers(player.Scene);
                    break;
                }
                // wait for user input
                try
                {
                    // try to parse the input data to the input Enum
                    string data = GetMessage(player.Socket);
                    Input input = (Input)Enum.Parse(typeof(Input), data);
                    Console.WriteLine(input);
                    switch (input)
                    {
                        case Input.right:
                            CheckMove(1, 0, player);
                            //player.Move(1, 0);
                            break;
                        case Input.down:
                            CheckMove(0, 1, player);
                            //player.Move(0, 1);
                            break;
                        case Input.left:
                            CheckMove(-1, 0, player);
                            //player.Move(-1, 0);
                            break;
                        case Input.up:
                            CheckMove(0, -1, player);
                            //player.Move(0, -1);
                            break;
                        case Input.quit:
                            player.Socket.Dispose();
                            break;
                        case Input.ignore:
                            continue;
                        default:
                            break;
                    }
                    UpdatePlayers(player.Scene);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Kon input niet parsen naar Enum typeof Input");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Er ging iets mis tijdens de verbinding:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    // and update all the other players
                    UpdatePlayers(player.Scene);
                    // break this loop to end the connection on the client's side
                    break;
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("Er ging iets mis tijdens de verbinding:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    // and update all the other players
                    UpdatePlayers(player.Scene);
                    // break this loop to end the connection on the client's side
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Er ging iets mis:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    
                }

            }

        }

        internal static string GetMessage(Socket client)
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

        internal static void SendMessage(Socket client, string message)
        {
            string messageLength;

            int len = Encoding.UTF8.GetByteCount(message);
            messageLength = len.ToString();
            for (int i = len.ToString().Length; i < header; i++)
            {
                messageLength += " ";
            }

            // send the length of the message and the message respectively
            client.Send(Encoding.UTF8.GetBytes(messageLength));
            client.Send(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Updates the players in the scene.<br/>
        /// This method also removes all the disconnected players. <br/>
        /// Because of this, this is the safe way to update all the players in a specific scene.
        /// </summary>
        /// <param name="scene">The scene's players you want to update</param>
        private void UpdatePlayers(Scene scene)
        {
            List<Player> disconnected = scene.Update();
            Console.WriteLine("Disconnected count: " + disconnected.Count);

            foreach (Player disconnect in disconnected)
            {
                players.Remove(disconnect);
                try
                {
                    disconnect.Socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to disconnect socket: socket was most likely already disconnected");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
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
            foreach (Player disconnect in disconnected)
            {
                players.Remove(disconnect);
                try
                {
                    disconnect.Socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to disconnect socket: socket was most likely already disconnected");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        /// <summary>
        /// Add an instance of a Player class to the list of players on the server
        /// </summary>
        /// <param name="player">The player you want to add to the server</param>
        /// <returns>A boolean determining if the player was added or not:<br/>
        /// <b>True</b> - when the player was succesfully added to the list of players. <br/>
        /// <b>False</b> - when the player wasn't added, this could be, because the player was already on the server
        /// </returns>
        private bool AddPlayer(Player player)
        {
            if (players.Contains(player))
            {
                return false;
            }
            players.Add(player);
            return true;
        }

        private void CheckMove(int x, int y, Player player)
        {
            Scene playerScene = player.Scene;
            int[] playerPos = player.Position;
            int nextPos = (playerPos[1] + y) * playerScene.Width + playerPos[1] + y + playerPos[0] + x;
            switch (playerScene.GameField(player)[nextPos])
            {
                case '─':
                case '│':
                case '┌':
                case '┐':
                case '┘':
                case '└':
                case '#':
                case '?':
                    break;

                // item cases
                // Door-case
                case '$':
                    Door.CheckForPlayer(player, scenes, x, y);
                    break;
                // Monster case
                case '@':
                    Monster.CheckForPlayer(player, x, y);
                    break;
                // HealingBottle case
                case '+':
                    HealingBottle.CheckForPlayer(player, x, y);
                    break;
                // ExperienceBottle case
                case '&':
                    ExperienceBottle.CheckForPlayer(player, x, y);
                    break;
                // Trap-activator
                case '*':
                    Trap.CheckForPlayer(player, x, y);
                    player.Move(x * 2, y * 2);
                    break;
                default:
                    player.Move(x, y);
                    break;
            }
        }
    }
}
