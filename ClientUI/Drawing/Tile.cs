namespace ClientUI.Drawing
{
    internal class Tile
    {
        internal Rectangle placement;
        internal Rectangle sprite;

        internal Tile(Rectangle placement, Rectangle sprite)
        {
            this.placement = placement;
            this.sprite = sprite;
        }

        internal Tile(Tile copy)
        {
            placement = new Rectangle(copy.placement.X, copy.placement.Y, copy.placement.Width, copy.placement.Height);
            sprite = new Rectangle(copy.sprite.X, copy.sprite.Y, copy.sprite.Width, copy.sprite.Height);
        }
    }
}
