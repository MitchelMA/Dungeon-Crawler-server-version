using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class ExperienceBottle : AItem
    {
        internal string size;

        internal ExperienceBottle(int[] position, string size, Dictionary<string, int> dataTable, int sceneWidth) : base(position, sceneWidth)
        {
            this.size = size;
        }

        protected override void Interact(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
