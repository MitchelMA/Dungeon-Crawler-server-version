using System;

namespace Server.Game.Items
{
    /// <summary>
    /// Abstract item class.<br/>
    /// This class gets used for the creation of new items
    /// </summary>
    internal abstract class AItem
    {
        protected int[] position = new int[2];
        protected int positionIndex;

        internal int[] Position { get => new int[2] { position[0], position[1] }; }
        internal int PositionIndex { get => positionIndex; }
        protected abstract void Interact(Player player);

        internal static void CheckForPlayer(Player player, int x, int y)
        {
            throw new NotImplementedException();
        }

        internal AItem(int[] position, int sceneWidth)
        {
            // DO NOT TAKE OVER THE MEMORY ADDRES
            // INSTEAD COPY, LIKE HERE BELOW
            this.position[0] = position[0];
            this.position[1] = position[1];
            positionIndex = this.position[1] * sceneWidth + this.position[1] + this.position[0];
        }
    }
}
