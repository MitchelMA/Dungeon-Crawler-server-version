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
                TileSrcLocation tmp = new TileSrcLocation(kvp.Value);
                nameTiles.Add(kvp.Key, tmp);
            }

            // now copy all the chars
            foreach (KeyValuePair<char, TileSrcLocation> kvp in copy.charTiles)
            {
                TileSrcLocation tmp = new TileSrcLocation(kvp.Value);
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
            return new Tile(placement, sprite);
        }

        internal Tile Parse(string name, int stringIndex, int stringWidth, int tileSize)
        {
            // get the src location from the namelist
            TileSrcLocation srcLocation = nameTiles[name];

            // convert the string-index to a x and y coordinate
            int x = stringIndex % stringWidth * tileSize;
            int y = stringIndex / stringWidth * tileSize;

            // copy the data to Rectangles
            Rectangle placement = new Rectangle(x, y, tileSize, tileSize);
            Rectangle sprite = new Rectangle(srcLocation.position.X, srcLocation.position.Y, srcLocation.widthHeight.X, srcLocation.widthHeight.Y);

            // return a new instance of a tile
            return new Tile(placement, sprite);
        }

        internal List<Tile> ParseText(string text, int textWidth, int tileSize)
        {
            List<Tile> tiles = new List<Tile>();
            // presets for the types of tiles
            Tile floorTile = Parse("floor", 0, 1, tileSize);
            Tile monsterTile = Parse("monster", 0, 1, tileSize);
            Tile playerTile = Parse("player", 0, 1, tileSize);
            Tile otherPlayerTile = Parse("otherp", 0, 1, tileSize);
            Tile wallTile = Parse("wallH", 0, 1, tileSize);
            Tile cornerTile = Parse("cornerTR", 0, 1, tileSize);
            Tile doorTile = Parse("door", 0, 1, tileSize);
            Tile healTile = Parse("healpot", 0, 1, tileSize);
            Tile xpTile = Parse("xppot", 0, 1, tileSize);

            // now turn the text int a list of tiles
            for (int i = 0; i < text.Length; i++)
            {
                Tile t = null;
                // optimizing the code itself
                if (text[i] == ' ')
                {
                    continue;
                }
                // now copy the pre-parsed tiles
                switch (text[i])
                {
                    // floor-tile
                    case '·':
                    case '*':
                        t = new Tile(floorTile);
                        break;
                    // monster-til
                    case '@':
                        t = new Tile(monsterTile);
                        break;
                    // player-tile
                    case '¶':
                        t = new Tile(playerTile);
                        break;
                    // other-player-tile
                    case '?':
                        t = new Tile(otherPlayerTile);
                        break;
                    // walls
                    case '│':
                    case '─':
                    case '#':
                        t = new Tile(wallTile);
                        break;
                    // corner tiles
                    case '┐':
                    case '┌':
                    case '└':
                    case '┘':
                        t = new Tile(cornerTile);
                        break;
                    // door tiles
                    case '$':
                        t = new Tile(doorTile);
                        break;
                    // healing pots
                    case '+':
                        t = new Tile(healTile);
                        break;
                    // xp pots
                    case '&':
                        t = new Tile(xpTile);
                        break;
                }

                if (t == null)
                {
                    continue;
                }

                // set the correct cords of the pre-parsed copy
                int x = i % textWidth;
                int y = i / textWidth;

                t.placement.X = x * tileSize;
                t.placement.Y = y * tileSize;
                tiles.Add(t);

            }
            return tiles;
        }
    }
}
