using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class PlayerStructure
    {
        [JsonPropertyName("hp")]
        public int Hp { get; set; }
        [JsonPropertyName("damage")]
        public int[] Damage { get; set; }
        [JsonPropertyName("xp-needed-next")]
        public int XpNeededNext { get; set; }
    }
}
