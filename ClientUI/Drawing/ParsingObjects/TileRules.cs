using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ClientUI.Drawing.ParsingObjects
{
    internal class TileRules
    {
        internal Dictionary<string, LevelObject> LevelObjects { get; set; }

        internal TileRules(string path)
        {
            string exeDir = PathHelper.ExeDir;
            this.LevelObjects = Serializer.Deserialize<Dictionary<string, LevelObject>>(Path.Combine(exeDir, path));
        }

        internal TileRules(TileRules copy)
        {
            this.LevelObjects = new Dictionary<string, LevelObject>();
            foreach(KeyValuePair<string, LevelObject> pair in copy.LevelObjects)
            {
                this.LevelObjects.Add(pair.Key, CopyLevelObject(pair.Value));
            }
        }

        private static BlockDataObject CopyDataObject(BlockDataObject copy)
        {
            BlockDataObject tmp = new BlockDataObject { };
            tmp.Position = new int[copy.Position.Length];
            tmp.Position[0] = copy.Position[0];
            tmp.Position[1] = copy.Position[1];

            tmp.BlockRules = new string[copy.BlockRules.Length][];
            for(int i = 0; i < copy.BlockRules.Length; i++)
            {
                tmp.BlockRules[i] = new string[copy.BlockRules[i].Length];
                tmp.BlockRules[i][0] = copy.BlockRules[i][0];
                tmp.BlockRules[i][1] = copy.BlockRules[i][1];
            }

            return tmp;
        }

        private static LevelObject CopyLevelObject(LevelObject copy)
        {
            LevelObject tmp = new LevelObject { };
            tmp.ForcedTheme = copy.ForcedTheme;

            tmp.Data = new BlockDataObject[copy.Data.Length];
            for(int i = 0; i < copy.Data.Length; i++)
            {
                tmp.Data[i] = CopyDataObject(copy.Data[i]);
            }

            return tmp;
        }
    }
}
