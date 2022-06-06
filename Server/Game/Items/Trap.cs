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

        internal Trap(int[] position, string groupName, int sceneWidth) : base(position, sceneWidth)
        {
            this.groupName = groupName;
        }

        protected override void Interact()
        {
            throw new NotImplementedException();
        }
    }
}
