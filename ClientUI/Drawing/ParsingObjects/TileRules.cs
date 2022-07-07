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
    }
}
