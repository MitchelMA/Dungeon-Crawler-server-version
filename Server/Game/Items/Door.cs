using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class Door : AItem
    {
        internal int[] destPos = new int[2];
        internal string destName;

        internal Door(int[] position, int[] destPos, string destName, int sceneWidth) : base(position, sceneWidth)
        {
            this.destPos[0] = destPos[0];
            this.destPos[1] = destPos[1];
            this.destName = destName;
        }

        protected override void Interact()
        {
            throw new NotImplementedException();
        }
    }
}
