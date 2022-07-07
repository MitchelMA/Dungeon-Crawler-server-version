using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClientUI.Drawing.ParsingObjects
{
    internal class LevelObject
    {
        [JsonPropertyName("forced-theme")]
        public string ForcedTheme { get; set; }
        [JsonPropertyName("data")]
        public DataObject[] Data { get; set; }
    }
}
