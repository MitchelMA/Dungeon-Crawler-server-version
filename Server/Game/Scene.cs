using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using Server.Game.Items;
using Server.Game.DataStructures;
using Shared;
using Shared.Security;
using Server.Server;

namespace Server.Game
{
    internal class Scene
    {
        // generic scene info
        private string gameField;
        private string name;
        private readonly int[] beginPosition;
        private int width;

        // paths to item-data
        private Dictionary<string, MonsterType> monsterData;
        private Dictionary<string, int> experienceData;
        private Dictionary<string, int> healingData;

        // items in the scene (players count too)
        private List<Player> players;
        private Mutex playerListMutex = new Mutex();
        private List<Door> doors;
        private List<Monster> monsters;
        private List<HealingBottle> healingBottles;
        private List<ExperienceBottle> experienceBottles;
        private Dictionary<string, Trap[]> traps;

        // internal Properties of scene info
        internal string GameField(Player player)
        {
            var copy = this.gameField;
            // draw the doors
            foreach (Door door in doors)
            {
                copy = copy.ReplaceAt(door.PositionIndex, 1, "$");
            }
            // draw the monsters
            foreach (Monster monster in monsters)
            {
                copy = copy.ReplaceAt(monster.PositionIndex, 1, "@");
            }
            // draw the healing bottles
            foreach (HealingBottle heal in healingBottles)
            {
                copy = copy.ReplaceAt(heal.PositionIndex, 1, "+");
            }
            // draw the experience bottles
            foreach (ExperienceBottle experience in experienceBottles)
            {
                copy = copy.ReplaceAt(experience.PositionIndex, 1, "&");
            }
            // draw the traps
            foreach (KeyValuePair<string, Trap[]> trapGroup in traps)
            {
                foreach (Trap trap in trapGroup.Value)
                {
                    if (player.ActivatedTraps.Contains(trap))
                    {
                        copy = copy.ReplaceAt(trap.PositionIndex, 1, "#");
                    }
                    else
                    {
                        copy = copy.ReplaceAt(trap.PositionIndex, 1, "*");
                    }
                }
            }
            // now draw the players on top of everything
            Console.WriteLine($"player-pos: [{player.Position[0]}, {player.Position[1]}]; addr: {player.Socket.RemoteEndPoint}");
            foreach (Player p in players)
            {
                if (p == player)
                {
                    copy = copy.ReplaceAt(p.InSceneIndex, 1, "¶");
                }
                else
                {
                    copy = copy.ReplaceAt(p.InSceneIndex, 1, "?");
                }
            }
            return copy;

        }
        internal string Name { get => name; }
        internal int[] BeginPosition { get => new int[2] { beginPosition[0], beginPosition[1] }; }
        internal int Width { get => width; }

        // internal Properties of items in the scene
        internal List<Player> Players { get => new List<Player>(players); }
        internal List<Door> Doors { get => new List<Door>(doors); }
        internal List<Monster> Monsters { get => new List<Monster>(monsters); }
        internal List<HealingBottle> HealingBottles { get => new List<HealingBottle>(healingBottles); }
        internal List<ExperienceBottle> ExperienceBottles { get => new List<ExperienceBottle>(experienceBottles); }
        internal Dictionary<string, Trap[]> Traps
        {
            get
            {
                Dictionary<string, Trap[]> tmp = new();
                foreach (KeyValuePair<string, Trap[]> pair in traps)
                {
                    List<Trap> traptmp = new();
                    foreach (Trap trap in pair.Value)
                    {
                        traptmp.Add(trap);
                    }
                    tmp.Add(pair.Key, traptmp.ToArray());
                }
                return tmp;
            }
        }

        internal Scene(Reference reference, string dataName)
        {
            // create an empty list for the players in this scene
            players = new List<Player>();
            doors = new List<Door>();
            monsters = new List<Monster>();
            healingBottles = new List<HealingBottle>();
            experienceBottles = new List<ExperienceBottle>();
            traps = new Dictionary<string, Trap[]>();

            // get the structure containing all the data for this scene
            string structurePath = Path.Combine(@"game-data\", reference.LevelDataPath, dataName + ".json");
            SceneStructure structure = Serializer.Deserialize<SceneStructure>(structurePath);

            // now get the paths to item-data
            DirectoryInfo itemDataDir = new DirectoryInfo(Path.Combine(@".\game-data\", reference.ItemDataPath));
            monsterData = Serializer.Deserialize<Dictionary<string, MonsterType>>(Path.Combine(itemDataDir.FullName, "Monster.json"));
            experienceData = Serializer.Deserialize<Dictionary<string, int>>(Path.Combine(itemDataDir.FullName, "Experience.json"));
            healingData = Serializer.Deserialize<Dictionary<string, int>>(Path.Combine(itemDataDir.FullName, "Healing.json"));

            this.name = structure.Name;
            // set the begin position
            beginPosition = structure.BeginPosition;
            // set the width
            width = structure.Width;
            // set the doors
            if (structure.Doors != null)
            {
                foreach (DoorStructure doorStructure in structure.Doors)
                {
                    this.doors.Add(new Door(doorStructure.Position, doorStructure.DestPosition, doorStructure.DestName, width));
                }
            }
            // set the monsters
            if (structure.Monsters != null)
            {
                foreach (MonsterStructure monsterStructure in structure.Monsters)
                {
                    this.monsters.Add(new Monster(monsterStructure.Position, monsterStructure.Difficulty, monsterData, width));
                }
            }
            // set the healing bottles
            if (structure.HealingBottles != null)
            {
                foreach (BottleStructure bottleStructure in structure.HealingBottles)
                {
                    this.healingBottles.Add(new HealingBottle(bottleStructure.Position, bottleStructure.Size, healingData, width));
                }
            }

            // set the experience bottles
            if (structure.ExperienceBottles != null)
            {
                foreach (BottleStructure bottleStructure in structure.ExperienceBottles)
                {
                    this.experienceBottles.Add(new ExperienceBottle(bottleStructure.Position, bottleStructure.Size, experienceData, width));
                }
            }
            // set the traps
            if (structure.Traps != null)
            {
                foreach (KeyValuePair<string, int[][]> trapStructure in structure.Traps)
                {
                    int groupSize = trapStructure.Value.Length;
                    Trap[] trapArr = new Trap[groupSize];

                    for (int i = 0; i < groupSize; i++)
                    {
                        trapArr[i] = new Trap(trapStructure.Value[i], trapStructure.Key, width);
                    }
                    this.traps.Add(trapStructure.Key, trapArr);
                }
            }
            // set the value of the gameField
            using FileStream f = File.OpenRead(Path.Combine(@".\game-data\", reference.LevelPath, dataName + ".txt"));
            byte[] bytes = new byte[f.Length];
            f.Read(bytes);
            gameField = Encoding.UTF8.GetString(bytes);
        }

        internal bool RemoveMonster(Monster monster)
        {
            return monsters.Remove(monster);
        }
        internal bool RemoveExperience(ExperienceBottle bottle)
        {
            return experienceBottles.Remove(bottle);
        }
        internal bool RemoveHealing(HealingBottle bottle)
        {
            return healingBottles.Remove(bottle);
        }
        /// <summary>
        /// Adds a player to this scene
        /// </summary>
        /// <param name="player">The player that gets added to the list of players</param>
        /// <returns>A boolean determining if it was added or not: <br/>
        /// <b>True</b> - when the player was added <br/>
        /// <b>False</b> - when the player wasn't added, this could be because <br/>
        /// the player was already in this scene
        /// </returns>
        internal bool AddplayerToScene(Player player)
        {
            // make sure no doubles end up in the list of players
            if (players.Contains(player))
                return false;
            // use the mutex to be able to safely add the player
            playerListMutex.WaitOne();
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Entered protected area to add player to scene");
            Console.ForegroundColor = ServerSocket.standColor;
            // add the player to the list in the protected area
            players.Add(player);
            // now release the mutex so it can be used again
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Releasing mutex after adding player to scene");
            Console.ForegroundColor = ServerSocket.standColor;
            playerListMutex.ReleaseMutex();
            return true;
        }
        /// <summary>
        /// Removes a player from this scene
        /// </summary>
        /// <param name="player">The player that gets removed from the scene</param>
        /// <returns>A boolean determining if it was removed:<br/>
        /// <b>True</b> - when the player was succesfully removed <br/>
        /// <b>False</b> - when the player wasn't removed</returns>
        internal bool RemovePlayerFromScene(Player player)
        {
            // use the mutex to enter a protected area
            playerListMutex.WaitOne();
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Entered protected area to remove player from scene");
            Console.ForegroundColor = ServerSocket.standColor;
            // now remove the player from the list of players in the protected area
            bool removed = players.Remove(player);
            // now release the mutex to leave the protected area
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Releasing mutex after removing player from scene");
            Console.ForegroundColor = ServerSocket.standColor;
            playerListMutex.ReleaseMutex();
            return removed;
        }

        /// <summary>
        /// Sends the GameField to all the players in this scene
        /// </summary>
        /// <returns>A list of players who got disconnected</returns>
        internal List<Player> Update()
        {
            // first, get the list of disconnected players
            // these have already been removed from this scene
            List<Player> disconnected = UpdateStatus();
            // after knowing which players got disconnected,
            // only update the connected players
            // wait on the mutex to safely loop through all the players in the scene
            playerListMutex.WaitOne();
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Entered protected area to loop through all the players in the scene");
            Console.ForegroundColor = ServerSocket.standColor;
            foreach (Player player in players)
            {
                // don't send any messages to disconnected players
                try
                {
                    string message = "";
                    message += $"Position: {player.Position[0]}, {player.Position[1]}";
                    message += $"{Environment.NewLine}Scene: {player.Scene.Name}";
                    message += $"{Environment.NewLine}Level: {player.CurrentLvl}";
                    message += $"{Environment.NewLine}Damage: {player.Damage[0]} - {player.Damage[1]}";
                    message += $"{Environment.NewLine}HP: {player.CurrentHp}/{player.MaxHp}";
                    message += $"{Environment.NewLine}XP: {player.CurrentXp}/{player.XpNecUp * player.CurrentLvl}{Environment.NewLine}";
                    message += GameField(player);
                    // first encrypt the message with the symmetric key
                    string encryptedM = ServerSocket.DataSecurity.EncryptAES(message);
                    ServerSocket.SendMessage(player.Socket, encryptedM);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ServerSocket.errColor;
                    Console.WriteLine("Failed to update player");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = ServerSocket.standColor;
                }
            }

            // at the end of the loop, release the mutex to exit the protected area
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Releasing mutex after looping through players in the scene");
            Console.ForegroundColor = ServerSocket.standColor;
            playerListMutex.ReleaseMutex();
            // at the end, return all the disconnected, so the server knows which one to clean up
            return disconnected;
        }

        /// <summary>
        /// Updates the status of this scene. <br/>
        /// Detecting which players disconnected and removing them
        /// </summary>
        /// <returns>Returns a list containing all the players in this scene who disconnected</returns>
        internal List<Player> UpdateStatus()
        {
            List<Player> disconnected = new List<Player>();
            // first, check for disconnected
            // use the mutex so we know for sure that the list of players cannot be changed while in this loop
            playerListMutex.WaitOne();
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Entered protected area to loop through all the players in the scene");
            Console.ForegroundColor = ServerSocket.standColor;
            foreach (Player player in players)
            {
                try
                {
                    bool connected = !(player.Socket.Poll(1, System.Net.Sockets.SelectMode.SelectRead) && player.Socket.Available == 0);
                    if (!connected)
                    {
                        Console.ForegroundColor = ServerSocket.disconnColor;
                        Console.WriteLine("Disconnection found");
                        Console.ForegroundColor = ServerSocket.standColor;
                        disconnected.Add(player);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ServerSocket.errColor;
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = ServerSocket.disconnColor;
                    Console.WriteLine("Disconnection found");
                    Console.ForegroundColor = ServerSocket.standColor;
                    disconnected.Add(player);
                }
            }
            // after this loop, we can exit the protected area by releasing the mutex
            Console.ForegroundColor = ServerSocket.mutexInfoColor;
            Console.WriteLine("Releasing mutex after looping through all the players in the scene");
            Console.ForegroundColor = ServerSocket.standColor;
            playerListMutex.ReleaseMutex();
            // then remove all the disconnected
            foreach (Player disconnect in disconnected)
            {
                Console.WriteLine(RemovePlayerFromScene(disconnect));
            }
            Console.WriteLine("In scene: " + players.Count);
            return disconnected;
        }
    }
}
