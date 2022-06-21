using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Server.Game;
using Server.Game.Items;
using Server.Game.DataStructures;
using Shared;
using Shared.Security;

namespace Server.Server
{
    internal class ServerSocket
    {

        private readonly Socket listener;
        private List<Player> players;
        private Dictionary<string, Scene> scenes;
        private readonly IPEndPoint endPoint;

        private const short header = 64;

        // console log colours
        internal static readonly ConsoleColor standColor = ConsoleColor.White;
        internal static readonly ConsoleColor errColor = ConsoleColor.Red;
        internal static readonly ConsoleColor connColor = ConsoleColor.Green;
        internal static readonly ConsoleColor disconnColor = ConsoleColor.Blue;

        // info about the game, for instance: the reference.json
        private Reference gameReference;

        // data security sectup
        private static DataSecurity dataSecurity = new DataSecurity();

        internal static DataSecurity DataSecurity => dataSecurity;

        internal ServerSocket(string host, int port)
        {

            players = new List<Player>();
            scenes = new Dictionary<string, Scene>();
            IPAddress ipAddress = IPAddress.Parse(host);
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Listening]: Server started listening on: {endPoint.Address}:{endPoint.Port}");
            Console.ForegroundColor = standColor;
            while (true)
            {
                Socket client = listener.Accept();
                Console.ForegroundColor = connColor;
                Console.WriteLine($"[New Connection]: {client.RemoteEndPoint}");
                Console.ForegroundColor = standColor;
                Player player;
                try
                {
                    player = SetupPlayer(client);
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = errColor;
                    Console.WriteLine("Failed to establish a save connection");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = standColor;
                    client.Dispose();
                    continue;
                }

                Console.ForegroundColor = connColor;
                Console.WriteLine($"[Save Connection]: A save connection has been established with {client.RemoteEndPoint}");
                Console.ForegroundColor = standColor;

                Thread loop = new Thread(() => GameLoop(player));
                loop.Start();
            }
        }

        private Player SetupPlayer(Socket client)
        {
            // at first setup of the client with the public key exchange
            string pubKey = GetMessage(client);

            // then we can send our symmetric key encrypted with the client's public key
            string enctypedKey = dataSecurity.EncryptRSA(pubKey, Convert.ToBase64String(dataSecurity.Aes.Key));
            string encryptedIV = dataSecurity.EncryptRSA(pubKey, Convert.ToBase64String(dataSecurity.Aes.IV));

            SendMessage(client, enctypedKey);
            SendMessage(client, encryptedIV);

            PlayerStructure playerStructure = Serializer.Deserialize<PlayerStructure>(Path.Combine(@".\game-data\", gameReference.ItemDataPath, @"player.json"));
            Scene first = scenes.First().Value;
            Player player = new Player(playerStructure.Hp, first.BeginPosition, playerStructure.Damage, playerStructure.XpNeededNext, first, client, pubKey);
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
                    // decrypt the input
                    string decryptedD = dataSecurity.DecryptAES(data);
                    Input input = (Input)Enum.Parse(typeof(Input), decryptedD);
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
                    Console.ForegroundColor = errColor;
                    Console.WriteLine("Kon input niet parsen naar Enum typeof Input");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = standColor;
                }
                catch (SocketException e)
                {
                    Console.ForegroundColor = errColor;
                    Console.WriteLine("Er ging iets mis tijdens de verbinding:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = standColor;
                    // and update all the other players
                    UpdatePlayers(player.Scene);
                    // break this loop to end the connection on the client's side
                    break;
                }
                catch (ObjectDisposedException e)
                {
                    Console.ForegroundColor = errColor;
                    Console.WriteLine("Er ging iets mis tijdens de verbinding:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = standColor;
                    // and update all the other players
                    UpdatePlayers(player.Scene);
                    // break this loop to end the connection on the client's side
                    break;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = errColor;
                    Console.WriteLine("Er ging iets mis:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = standColor;
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
                    Console.ForegroundColor = errColor;
                    Console.WriteLine("Failed to disconnect socket: socket was most likely already disconnected");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = standColor;
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
                    Console.ForegroundColor = errColor;
                    Console.WriteLine("Failed to disconnect socket: socket was most likely already disconnected");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = standColor;
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
