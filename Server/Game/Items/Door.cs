using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class Door : AItem
    {
        private int[] destPos = new int[2];
        private string destName;

        internal int[] DestPos { get => new int[2] { destPos[0], destPos[1] }; }
        internal string DestName { get => destName; }

        internal Door(int[] position, int[] destPos, string destName, int sceneWidth) : base(position, sceneWidth)
        {
            this.destPos[0] = destPos[0];
            this.destPos[1] = destPos[1];
            this.destName = destName;
        }

        protected override void Interact(Player player)
        {
            throw new NotImplementedException();
        }
        private void Interact(Player player, Dictionary<string, Scene> scenes)
        {
            Scene oldScene = player.Scene;
            Scene newScene = scenes[destName];
            player.Transfer(oldScene, newScene);
            player.MoveTo(destPos[0], destPos[1]);
        }

        internal static void CheckForPlayer(Player player, Dictionary<string, Scene> scenes, int x, int y)
        {
            player.Move(x, y);
            int playerPosIndex = player.InSceneIndex;
            player.Move(-x, -y);
            foreach(Door door in player.Scene.Doors)
            {
                if(playerPosIndex == door.positionIndex)
                {
                    Console.WriteLine($"Found door at {door.positionIndex}");
                    door.Interact(player, scenes);
                    break;
                }
            }
        }
    }
}
