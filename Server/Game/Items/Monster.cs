using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Game.DataStructures;

namespace Server.Game.Items
{
    internal class Monster : AItem
    {
        internal string difficulty;

        internal Monster(int[] position, string difficulty, Dictionary<string, MonsterType> dataTable, int sceneWidth) : base(position, sceneWidth)
        {
            this.difficulty = difficulty;
        }

        protected override void Interact(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
