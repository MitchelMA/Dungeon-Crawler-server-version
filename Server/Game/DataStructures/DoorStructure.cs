using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class DoorStructure
    {
        [JsonPropertyName("position")]
        public int[] Position { get; set; }
        [JsonPropertyName("dest-position")]
        public int[] DestPosition { get; set; }
        [JsonPropertyName("dest-name")]
        public string DestName { get; set; }
    }
}
