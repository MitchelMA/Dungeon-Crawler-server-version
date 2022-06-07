using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Server.Game.Items;
using Server.Game.DataStructures;
using Shared;
using Server.Server;

namespace Server.Game
{
    internal class Scene
    {
        // generic scene info
        private string gameField;
        internal string name;
        internal readonly int[] beginPosition;
        internal int width;

        // paths to item-data
        private Dictionary<string, MonsterType> monsterData;
        private Dictionary<string, int> experienceData;
        private Dictionary<string, int> healingData;

        // items in the scene (players count too)
        private List<Player> players;
        private List<Door> doors;
        private List<Monster> monsters;
        private List<HealingBottle> healingBottles;
        private List<ExperienceBottle> experienceBottles;
        private Dictionary<string, Trap[]> traps;

        internal string GameField
        {
            get
            {
                var copy = this.gameField;
                // draw the doors
                foreach (Door door in doors)
                {
                    copy = copy.ReplaceAt(door.positionIndex, 1, "$");
                }
                // draw the monsters
                foreach (Monster monster in monsters)
                {
                    copy = copy.ReplaceAt(monster.positionIndex, 1, "@");
                }
                // draw the healing bottles
                foreach (HealingBottle heal in healingBottles)
                {
                    copy = copy.ReplaceAt(heal.positionIndex, 1, "+");
                }
                // draw the experience bottles
                foreach (ExperienceBottle experience in experienceBottles)
                {
                    copy = copy.ReplaceAt(experience.positionIndex, 1, "&");
                }
                // draw the traps
                foreach (KeyValuePair<string, Trap[]> trapGroup in traps)
                {
                    foreach (Trap trap in trapGroup.Value)
                    {
                        if (trap.activated)
                        {
                            copy = copy.ReplaceAt(trap.positionIndex, 1, "#");
                        }
                        else
                        {
                            copy = copy.ReplaceAt(trap.positionIndex, 1, "*");
                        }
                    }
                }
                // now draw the players on top of everything
                int index = 0;
                foreach (Player player in players)
                {
                    Console.WriteLine($"index: {index}; player-pos: [{player.position[0]}, {player.position[1]}]; addr: {player.socket.RemoteEndPoint}");
                    copy = copy.ReplaceAt(player.InSceneIndex, 1, "¶");
                    index++;
                }
                return copy;
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
        /// <summary>
        /// Adds a player to this scene
        /// </summary>
        /// <param name="player">The player that gets added to the list of players</param>
        /// <returns>A boolean determining if it was added or not</returns>
        internal bool AddplayerToScene(Player player)
        {
            // make sure no doubles end up in the list of players
            if (players.Contains(player))
                return false;
            players.Add(player);
            return true;
        }
        /// <summary>
        /// Removes a player from this scene
        /// </summary>
        /// <param name="player">The player that gets removed from the scene</param>
        /// <returns>A boolean determining if it was removed</returns>
        internal bool RemovePlayerFromScene(Player player)
        {
            return players.Remove(player);
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
            foreach (Player player in players)
            {
                ServerSocket.SendMessage(player.socket, GameField);
            }
            // at the end, return all the disconnected, so the server knows which one to clean up
            return disconnected;
        }

        /// <summary>
        /// Updates the status of this scene. </br>
        /// Detecting which players disconnected and removing them
        /// </summary>
        /// <returns>Returns a list containing all the players in this scene who disconnected</returns>
        internal List<Player> UpdateStatus()
        {
            List<Player> disconnected = new List<Player>();
            // first, check for disconnected
            foreach (Player player in players)
            {
                bool connected = player.socket.Poll(50, System.Net.Sockets.SelectMode.SelectRead);
                if (connected)
                {
                    Console.WriteLine("Disconnection found");
                    disconnected.Add(player);
                    continue;
                }
            }
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
