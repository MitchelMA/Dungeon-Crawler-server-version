using System;
using Server.Game.DataStructures;
using Shared;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // try to parse a scene
            SceneStructure mySceneStruct = Serializer.Deserialize<SceneStructure>(@"game-data\level-data\tutorial.json");
            Console.WriteLine(mySceneStruct.Name);
        }
    }
}
