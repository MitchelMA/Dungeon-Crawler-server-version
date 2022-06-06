using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    internal class Door : AItem
    {
        internal int[] destPos;
        internal string destName;

        internal Door(int[] position, int[] destPos, string destName, int sceneWidth) : base(position, sceneWidth)
        {
            this.destPos = destPos;
            this.destName = destName;
        }

        protected override void Interact()
        {
            throw new NotImplementedException();
        }
    }
}
