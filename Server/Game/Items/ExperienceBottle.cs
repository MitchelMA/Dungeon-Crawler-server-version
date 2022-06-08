using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class ExperienceBottle : AItem
    {
        private string size;
        private int xpUp;

        internal int XpUp { get => xpUp; }
        internal string Size { get => size; }

        internal ExperienceBottle(int[] position, string size, Dictionary<string, int> dataTable, int sceneWidth) : base(position, sceneWidth)
        {
            this.size = size;
            xpUp = dataTable[size];
        }

        protected override void Interact(Player player)
        {
            player.XpUp(xpUp);
            player.Scene.RemoveExperience(this);
        }

        new internal static void CheckForPlayer(Player player, int x, int y)
        {
            player.Move(x, y);
            int playerPosIndex = player.InSceneIndex;
            player.Move(-x, -y);
            foreach(ExperienceBottle bottle in player.Scene.ExperienceBottles)
            {
                if(bottle.positionIndex == playerPosIndex)
                {
                    bottle.Interact(player);
                    break;
                }
            }
        }
    }
}
