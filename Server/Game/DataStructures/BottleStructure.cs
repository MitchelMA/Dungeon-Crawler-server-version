using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class BottleStructure
    {
        [JsonPropertyName("size")]
        public string Size { get; set; }
        [JsonPropertyName("position")]
        public int[] Position { get; set; }
    }
}
