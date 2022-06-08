using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class Trap : AItem
    {
        internal string groupName;
        internal bool activated;

        internal Trap(int[] position, string groupName, int sceneWidth) : base(position, sceneWidth)
        {
            this.groupName = groupName;
            activated = false;
        }

        protected override void Interact(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
