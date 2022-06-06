using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class ExperienceBottleStructure
    {
        [JsonPropertyName("size")]
        public string Size { get; set; }
        [JsonPropertyName("position")]
        public int[] Position { get; set; }
    }
}
