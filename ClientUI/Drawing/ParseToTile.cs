using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Drawing
{
    internal class ParseToTile
    {
        internal Dictionary<char, TileSrcLocation> charTiles = new Dictionary<char, TileSrcLocation>();
        internal Dictionary<string, TileSrcLocation> nameTiles = new Dictionary<string, TileSrcLocation>();

        internal ParseToTile(Dictionary<char, TileSrcLocation> charTiles, Dictionary<string, TileSrcLocation> nameTiles)
        {
            this.charTiles = charTiles;
            this.nameTiles = nameTiles;
        }

        internal ParseToTile(ParseToTile copy)
        {
            // copy all the names
            foreach (KeyValuePair<string, TileSrcLocation> kvp in copy.nameTiles)
            {
                TileSrcLocation tmp = new TileSrcLocation
                {
                    position = new Point(kvp.Value.position.X, kvp.Value.position.Y),
                    widthHeight = new Point(kvp.Value.widthHeight.X, kvp.Value.widthHeight.Y),
                };

                nameTiles.Add(kvp.Key, tmp);
            }

            // now copy all the chars
            foreach (KeyValuePair<char, TileSrcLocation> kvp in copy.charTiles)
            {
                TileSrcLocation tmp = new TileSrcLocation
                {
                    position = new Point(kvp.Value.position.X, kvp.Value.position.Y),
                    widthHeight = new Point(kvp.Value.widthHeight.X, kvp.Value.widthHeight.Y),
                };

                charTiles.Add(kvp.Key, tmp);
            }
        }

        internal Tile Parse(char character, int stringIndex, int stringWidth, int tileSize)
        {
            // get the src location from the charlist
            TileSrcLocation srcLocation = charTiles[character];
            // convert the string-index to a x and y coordinate
            int x = stringIndex % stringWidth;
            int y = stringIndex / stringWidth;
            // copy the data to Rectangles
            Rectangle placement = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
            Rectangle sprite = new Rectangle(srcLocation.position.X, srcLocation.position.Y, srcLocation.widthHeight.X, srcLocation.widthHeight.Y);

            // return a new instance of a tile
            return new Tile
            {
                placement = placement,
                sprite = sprite,
            };
        }

        internal Tile Parse(string name, int stringIndex, int stringWidth, int tileSize)
        {
            // get the src location from the namelist
            TileSrcLocation srcLocation = nameTiles[name];
            // convert the string-index to a x and y coordinate
            int offset = (stringIndex % stringWidth * (tileSize / srcLocation.widthHeight.X));
            //int x = stringIndex % stringWidth * tileSize - (stringIndex % stringWidth * (tileSize / srcLocation.widthHeight.X));
            int x = stringIndex % stringWidth * tileSize;
            int y = stringIndex / stringWidth * tileSize;
            // copy the data to Rectangles
            Rectangle placement = new Rectangle(x, y, tileSize, tileSize);
            Rectangle sprite = new Rectangle(srcLocation.position.X, srcLocation.position.Y, srcLocation.widthHeight.X, srcLocation.widthHeight.Y);

            // return a new instance of a tile
            return new Tile
            {
                placement = placement,
                sprite = sprite,
            };
        }
    }
}
