using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class MonsterStructure
    {
        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; }
        [JsonPropertyName("position")]
        public int[] Position { get; set; }
    }
}
