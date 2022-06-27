using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Drawing
{
    internal class ParseToTileFactory
    {
        private Dictionary<string, ParseToTile> parsers = new Dictionary<string, ParseToTile>();
        internal string Load(string pathName)
        {
            string file = File.ReadAllText(Path.Combine(pathName, @".\CurrentTileMap.txt"));

            // open the filenames that were include by the file from the filePath
            string[] lines = File.ReadAllLines(Path.Combine(pathName, file) + ".csv");
            Dictionary<string, TileSrcLocation> names = new Dictionary<string, TileSrcLocation>();
            Dictionary<char, TileSrcLocation> chars = new Dictionary<char, TileSrcLocation>();
            foreach (string line in lines)
            {
                string[] lineParts = line.Trim().Split(';');
                TileSrcLocation srcLoc = new TileSrcLocation
                {
                    position = new Point(int.Parse(lineParts[2].Trim()), int.Parse(lineParts[3].Trim())),
                    widthHeight = new Point(int.Parse(lineParts[4].Trim()), int.Parse(lineParts[5].Trim())),

                };

                names.Add(lineParts[0].Trim(), srcLoc);
                chars.Add(lineParts[1].Trim()[0], srcLoc);
            }
            ParseToTile tmp = new ParseToTile(chars, names);
            parsers.Add(file, tmp);
            
            return file;
        }
        internal ParseToTile Create(string name)
        {
            return new ParseToTile(parsers[name]);
        }
    }

}
