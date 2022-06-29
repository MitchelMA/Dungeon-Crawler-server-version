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

        private char[] charIndex;
        private Tile[] tileIndex;

        internal ParseToTile(Dictionary<char, TileSrcLocation> charTiles, Dictionary<string, TileSrcLocation> nameTiles)
        {
            this.charTiles = charTiles;
            this.nameTiles = nameTiles;

            // create the char and tile index corresponding to the dictionaries
            // this is usefull for faster parsing a bigger string
            charIndex = charTiles.Keys.ToArray();
            tileIndex = new Tile[charIndex.Length];
            for(int i = 0; i < charIndex.Length; i++)
            {
                tileIndex[i] = Parse(charIndex[i], 0, 1, 16); 
            }
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

            // copy the charIndex
            charIndex = new char[copy.charIndex.Length];
            for(int i = 0; i < copy.charIndex.Length; i++)
            {
                // this copies the value rather than the address
                charIndex[i] = copy.charIndex[i];
            }

            // copy the tileIndex
            tileIndex = new Tile[charIndex.Length];
            for(int i = 0; i < copy.tileIndex.Length; i++)
            {
                // this uses the copy-constructor of the tile to create a copy rather than copy the address
                tileIndex[i] = new Tile(copy.tileIndex[i]);
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
            Tile flagTile = Parse("flag", 0, 1, tileSize);

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
                    // flag
                    case '!':
                        t = new Tile(flagTile);
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

        internal List<Tile> ParseTextNew(string text, int textWidth, int tileSize)
        {
            List<Tile> tiles = new List<Tile>();
            
            for(int i = 0; i < text.Length; i++)
            {
                if(text[i] == ' ')
                {
                    continue;
                }
                // current character
                char cc = text[i];

                // current character-index
                int ccI = Array.IndexOf(charIndex, cc);
                if(ccI == -1)
                {
                    continue;
                }
                // current tile
                Tile ccT = new Tile(tileIndex[ccI]);

                // convert the string-index to an x and y coördinate
                int x = i % textWidth;
                int y = i / textWidth;

                // now correct the placement
                ccT.placement.X = x * tileSize;
                ccT.placement.Y = y * tileSize;
                ccT.placement.Width = tileSize;
                ccT.placement.Height = tileSize;

                // and add the current tile to the list of tiles
                tiles.Add(ccT);
            }
            return tiles;
        }
    }
}
