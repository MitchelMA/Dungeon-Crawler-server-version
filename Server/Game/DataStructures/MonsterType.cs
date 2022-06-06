using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class MonsterType
    {
        [JsonPropertyName("hp")]
        public int Hp { get; set; }
        [JsonPropertyName("damage")]
        public int[] Damage { get; set; }
    }
}
