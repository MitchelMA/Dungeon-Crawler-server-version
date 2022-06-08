using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class Trap : AItem
    {
        private string groupName;
        private bool activated;

        internal string GroupName { get => groupName; }
        internal bool Activated { get => activated; }

        internal Trap(int[] position, string groupName, int sceneWidth) : base(position, sceneWidth)
        {
            this.groupName = groupName;
            activated = false;
        }

        protected override void Interact(Player player)
        {
            foreach(Trap trap in player.Scene.Traps[groupName])
            {
                player.ActivateTrap(trap);
            }
        }

        new internal static void CheckForPlayer(Player player, int x, int y)
        {
            player.Move(x, y);
            int playerPosIndex = player.InSceneIndex;
            player.Move(-x, -y);
            foreach(Trap[] trapArr in player.Scene.Traps.Values)
            {
                foreach(Trap trap in trapArr)
                {
                    if(trap.positionIndex == playerPosIndex)
                    {
                        trap.Interact(player);
                        break;
                    }
                }
            }
        }
    }
}
