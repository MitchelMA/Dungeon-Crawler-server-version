using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClientUI.Drawing.ParsingObjects
{
    internal class DataObject
    {
        [JsonPropertyName("position")]
        public int[] Position { get; set; }
        [JsonPropertyName("block-rules")]
        public string[][] BlockRules { get; set; }
    }
}
