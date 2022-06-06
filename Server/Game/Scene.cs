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

namespace Server.Game
{
    internal class Scene
    {
        // generic scene info
        private string gameField;
        internal string name;
        private int[] beginPosition;
        private int width;

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
                    copy.ReplaceAt(door.positionIndex, 1, "$");
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

            beginPosition = structure.BeginPosition;
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
    }
}
