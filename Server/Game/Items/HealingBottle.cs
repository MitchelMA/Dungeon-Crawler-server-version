using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class HealingBottle : AItem 
    {
        private string size;
        private int hpUp;

        internal string Size { get => size; }
        internal int HpUp { get => hpUp; }

        internal HealingBottle(int[] position, string size, Dictionary<string, int> dataTable, int sceneWidth) : base(position, sceneWidth)
        {
            this.size = size;
            hpUp = dataTable[this.size];
        }

        protected override void Interact(Player player)
        {
            // don't consume the healing bottle when the hp of the specified player is already full
            if(player.CurrentHp == player.MaxHp)
            {
                return;
            }
            player.Heal(hpUp);
            player.Scene.RemoveHealing(this);
        }

        new internal static void CheckForPlayer(Player player, int x, int y)
        {
            player.Move(x, y);
            int playerPosIndex = player.InSceneIndex;
            player.Move(-x, -y);

            foreach(HealingBottle bottle in player.Scene.HealingBottles)
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
