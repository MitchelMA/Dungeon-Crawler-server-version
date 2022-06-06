using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class SceneStructure
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("begin-position")]
        public int[] BeginPosition { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("doors")]
        public DoorStructure[] Doors { get; set; }
        [JsonPropertyName("monsters")]
        public MonsterStructure[] Monsters { get; set; }
        [JsonPropertyName("healing-bottles")]
        public HealingBottleStructure[] HealingBottles { get; set; }
        [JsonPropertyName("experience-bottles")]
        public ExperienceBottleStructure[] ExperienceBottles { get; set; }
        [JsonPropertyName("traps")]
        public Dictionary<string, int[][]> Traps { get; set; }
    }
}
