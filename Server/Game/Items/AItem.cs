using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Items
{
    /// <summary>
    /// Abstract item class.<br/>
    /// This class gets used for the creation of new items
    /// </summary>
    internal abstract class AItem
    {
        internal int[] position = new int[2];
        internal int positionIndex;
        protected abstract void Interact();
        
        internal AItem(int[] position, int sceneWidth)
        {
            this.position = position;
            positionIndex = this.position[1] * sceneWidth + this.position[1] + this.position[0];
        }
    }
}
