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

        internal string[] Load(string pathName)
        {
            // reads the CurrentTileMap.txt and gets the name of the file
            string[] files = File.ReadAllLines(Path.Combine(pathName, @".\TileMaps.txt"));

            foreach (string file in files)
            {
                // open the filenames that were include by the file from the filePath
                string[] lines = File.ReadAllLines(Path.Combine(pathName, file) + ".csv");
                // create new instances for the dictionaries that will get used by the parsers
                Dictionary<char, TileSrcLocation> chars = new Dictionary<char, TileSrcLocation>();

                // parse the .csv file to a dictionary
                foreach (string line in lines)
                {
                    // split the lines
                    string[] lineParts = line.Trim().Split(',');
                    // create a new instance of a TileSrcLocation
                    TileSrcLocation srcLoc = new TileSrcLocation(
                        new Point(int.Parse(lineParts[1].Trim()), int.Parse(lineParts[2].Trim())),
                        new Point(int.Parse(lineParts[3].Trim()), int.Parse(lineParts[4].Trim())));

                    // add this TileSrcLocation to both the dictionaries
                    chars.Add(lineParts[0].Trim()[0], srcLoc);
                }
                // create a new parser with the data of the dictionaries
                ParseToTile tmp = new ParseToTile(chars);
                parsers.Add(file, tmp);
            }
            return files;
        }

        internal ParseToTile Load(string[] parseText)
        {
            Dictionary<char, TileSrcLocation> chars = new Dictionary<char, TileSrcLocation>();
            foreach(string line in parseText)
            {
                string[] lineParts = line.Trim().Split(',');
            }

            return new ParseToTile(chars);
        }
        // creates a new instance of a perser with a blueprint
        internal ParseToTile Create(string name)
        {
            return new ParseToTile(parsers[name]);
        }
    }

}
