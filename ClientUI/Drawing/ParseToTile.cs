using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientUI.Drawing.ParsingObjects;

namespace ClientUI.Drawing
{
    internal class ParseToTile
    {
        internal Dictionary<char, TileSrcLocation> charTiles = new Dictionary<char, TileSrcLocation>();

        private char[] charIndex;
        private Tile[] tileIndex;

        internal ParseToTile(Dictionary<char, TileSrcLocation> charTiles)
        {
            this.charTiles = charTiles;

            // create the char and tile index corresponding to the dictionaries
            // this is usefull for faster parsing a bigger string
            charIndex = charTiles.Keys.ToArray();
            tileIndex = new Tile[charIndex.Length];
            for (int i = 0; i < charIndex.Length; i++)
            {
                tileIndex[i] = Parse(charIndex[i], 0, 1, 16);
            }
        }

        internal ParseToTile(ParseToTile copy)
        {
            // now copy all the chars
            foreach (KeyValuePair<char, TileSrcLocation> kvp in copy.charTiles)
            {
                TileSrcLocation tmp = new TileSrcLocation(kvp.Value);
                charTiles.Add(kvp.Key, tmp);
            }

            // create the char and tile index corresponding to the dictionaries
            // this is usefull for faster parsing a bigger string
            charIndex = charTiles.Keys.ToArray();
            tileIndex = new Tile[charIndex.Length];
            for (int i = 0; i < charIndex.Length; i++)
            {
                tileIndex[i] = Parse(charIndex[i], 0, 1, 16);
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

        internal static List<Tile> ParseTextNew(string text, int textWidth, int tileSize, TileRules extraRules, ParseToTileFactory referenceFactory, ParseToTile referenceParser, string sceneName)
        {
            List<Tile> tiles = new List<Tile>();

            // create a second temporary parser to accomadate for the forced rules
            // and for the specified rules per block per scene
            ParseToTile secondary = null;
            BlockDataObject[] currentDataObjects = null;

            // check if the current scene even has a forced-theme
            bool hasRules = extraRules.LevelObjects.TryGetValue(sceneName, out LevelObject currentSceneRules);
            bool hasForced = false;

            if (hasRules && currentSceneRules.ForcedTheme != null)
            {
                currentDataObjects = currentSceneRules.Data;
                try
                {
                    secondary = referenceFactory.Create(currentSceneRules.ForcedTheme);
                    hasForced = true;
                }
                catch { };
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    continue;
                }

                // convert the string-index to an x and y coördinate
                int x = i % textWidth;
                int y = i / textWidth;

                if (hasRules)
                {
                    foreach (BlockDataObject data in currentDataObjects)
                    {
                        if (!(data.Position[0] == x && data.Position[1] == y))
                        {
                            continue;
                        }

                        for (int j = 0; j < data.BlockRules.Length; j++)
                        {
                            if (data.BlockRules[j][0][0] != text[i])
                            {
                                continue;
                            }

                            string newTheme = data.BlockRules[j][1];
                            if(newTheme.Contains(','))
                            {
                                string[] splitted = newTheme.Split(',');
                                // replace the old value with the new src-location
                                char key = data.BlockRules[j][0][0];
                                Point xy = new Point(int.Parse(splitted[0]), int.Parse(splitted[1]));
                                Point wh = new Point(int.Parse(splitted[2]), int.Parse(splitted[3]));
                                TileSrcLocation newSrc = new TileSrcLocation(xy, wh);
                                secondary.charTiles.Remove(key);
                                secondary.charTiles.Add(key, newSrc);
                                secondary = new ParseToTile(secondary);
                            }
                            else
                            {
                                try
                                {
                                    secondary = referenceFactory.Create(newTheme);
                                }
                                catch { };
                            }
                        }
                    }
                }
                // current character
                char cc = text[i];

                // current character-index
                int ccI = Array.IndexOf(referenceParser.charIndex, cc);
                if (secondary != null && hasRules)
                {
                    ccI = Array.IndexOf(secondary.charIndex, cc);
                }
                if (ccI == -1)
                {
                    continue;
                }
                // current tile
                Tile ccT = new Tile(referenceParser.tileIndex[ccI]);
                if (secondary != null && hasRules)
                {
                    ccT = new Tile(secondary.tileIndex[ccI]);
                }

                // now correct the placement
                ccT.placement.X = x * tileSize;
                ccT.placement.Y = y * tileSize;
                ccT.placement.Width = tileSize;
                ccT.placement.Height = tileSize;

                // and add the current tile to the list of tiles
                tiles.Add(ccT);

                if (hasRules && currentSceneRules.ForcedTheme != null)
                {
                    try
                    {
                        secondary = referenceFactory.Create(currentSceneRules.ForcedTheme);
                        hasForced = true;
                    }
                    catch
                    {
                        secondary = new ParseToTile(referenceParser);
                        hasForced = false;
                    };
                }
            }
            return tiles;
        }
    }
}
