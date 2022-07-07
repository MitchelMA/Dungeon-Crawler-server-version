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

        /// <summary>
        /// Adds another parser to this one.
        /// It will overwrite values in this parser if it conflicts with current values
        /// </summary>
        /// <param name="other"></param>
        internal void Combine(ParseToTile other)
        {
            foreach (KeyValuePair<char, TileSrcLocation> kvp in other.charTiles)
            {
                // overwrite the old value if it already existed
                if (charTiles.TryGetValue(kvp.Key, out _))
                {
                    charTiles.Remove(kvp.Key);
                    charTiles.Add(kvp.Key, kvp.Value);
                }
                // if not, add it
                else
                {
                    charTiles.Add(kvp.Key, kvp.Value);
                }
            }

            // now rearrange the charIndex and tileIndex array's
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

        internal static List<Tile> ParseText(string text, int textWidth, int tileSize, TileRules extraRules, ParseToTileFactory referenceFactory, ParseToTile referenceParser, string sceneName)
        {
            List<Tile> tiles = new List<Tile>();

            // create a new instance of a parser that is a copy of the referenceParser 
            ParseToTile parser = new ParseToTile(referenceParser);
            // and for the specified rules per block per scene
            BlockDataObject[] currentDataObjects = null;

            // check if the current scene even has a forced-theme
            bool hasRules = extraRules.LevelObjects.TryGetValue(sceneName, out LevelObject currentSceneRules);
            // if it does, setup the parser accordingly
            if (hasRules)
            {
                // get the data for per-cord parsing
                currentDataObjects = currentSceneRules.Data;
                try
                {
                    // try to get a parser for the forced theme
                    parser = referenceFactory.Create(currentSceneRules.ForcedTheme);
                }
                catch { };
                // create a parser to inject parsing-rules per scene
                ParseToTile injected = referenceFactory.Load(currentSceneRules.Injection);
                parser.Combine(injected);
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

                // check if the scene has extra parsing-rules
                if (!(currentDataObjects != null && hasRules))
                {
                    goto skipOne;
                }

                foreach (BlockDataObject data in currentDataObjects)
                {
                    // check if cords are the same
                    if (!(data.Position[0] == x && data.Position[1] == y))
                    {
                        continue;
                    }

                    for (int j = 0; j < data.BlockRules.Length; j++)
                    {
                        // check if the defined characters for the rule match up
                        if (data.BlockRules[j][0][0] != text[i])
                        {
                            continue;
                        }

                        // a parsing-rule can done in two ways:
                        // giving a rulesheet name:  ["?", "Red"]
                        // or giving tile-source locations: ["?", "00,00,16,16"]
                        string newTheme = data.BlockRules[j][1];
                        // check if a tile-source location was given
                        if (newTheme.Contains(','))
                        {
                            try
                            {
                                string[] splitted = newTheme.Split(',');
                                // replace the old value with the new src-location
                                char key = data.BlockRules[j][0][0];
                                // get the new source-location
                                Point xy = new Point(int.Parse(splitted[0]), int.Parse(splitted[1]));
                                Point wh = new Point(int.Parse(splitted[2]), int.Parse(splitted[3]));
                                TileSrcLocation newSrc = new TileSrcLocation(xy, wh);
                                // remove the old value and add the new one
                                parser.charTiles.Remove(key);
                                parser.charTiles.Add(key, newSrc);
                                // now create a new instance
                                // this has to be done because the constructor does something with private arrays
                                // this is necessary, else it won't work
                                parser = new ParseToTile(parser);
                            }
                            catch { };
                        }
                        // else we try to take it as a name
                        else
                        {
                            try
                            {
                                parser = referenceFactory.Create(newTheme);
                            }
                            catch { };
                        }
                    }
                }

            skipOne:
                // current character
                char cc = text[i];

                // current character-index
                int ccI = Array.IndexOf(parser.charIndex, cc);
                if (ccI == -1)
                {
                    continue;
                }

                // current tile
                Tile ccT = new Tile(parser.tileIndex[ccI]);

                // now correct the placement
                ccT.placement.X = x * tileSize;
                ccT.placement.Y = y * tileSize;
                ccT.placement.Width = tileSize;
                ccT.placement.Height = tileSize;

                // and add the current tile to the list of tiles
                tiles.Add(ccT);

                if (hasRules && currentSceneRules.ForcedTheme != null)
                {
                    // reset the parser back to the forcedTheme
                    // if this fails, we assume it was the referenceParsers
                    try
                    {
                        parser = referenceFactory.Create(currentSceneRules.ForcedTheme);
                    }
                    catch
                    {
                        parser = new ParseToTile(referenceParser);
                    };
                    
                    // alwasy at the injection
                    // this cannot give a null-exception
                    // since the Load will always return a parsers, with no capability to parse if necessary
                    ParseToTile injected = referenceFactory.Load(currentSceneRules.Injection);
                    parser.Combine(injected);
                }
            }
            return tiles;
        }
    }
}
