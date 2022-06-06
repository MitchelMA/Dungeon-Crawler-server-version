using System;
using Server.Game.DataStructures;
using Shared;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // try to parse the reference.json
            Reference myRef = Serializer.Deserialize<Reference>(@"game-data\reference.json");
            // log the reference
            Console.WriteLine($"Reference: \nlevel-data-path: {myRef.LevelDataPath}");
            Console.WriteLine($"level-path: {myRef.LevelPath}");
            Console.WriteLine($"item-data-path: {myRef.ItemDataPath}");
            foreach(string levelName in myRef.Levels)
            {
                Console.WriteLine($"level-name: {levelName}");
            }
            Console.WriteLine();

            // try to parse a scene
            SceneStructure mySceneStruct = Serializer.Deserialize<SceneStructure>(@"game-data\level-data\tutorial.json");
            // log the healing-bottles
            foreach (BottleStructure bottle in mySceneStruct.HealingBottles)
            {
                Console.WriteLine($"Size: {bottle.Size}");
                Console.WriteLine($"Position: ({bottle.Position[0]}, {bottle.Position[1]})\n");
            }
        }
    }
}
