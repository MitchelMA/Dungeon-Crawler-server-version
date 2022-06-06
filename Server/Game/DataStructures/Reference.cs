using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class Reference
    {
        public Include[] Includes { get; set; }
    }

    internal class Include
    {
        [JsonPropertyName("level-path")]
        public string LevelPath { get; set; }

        [JsonPropertyName("level-data")]
        public string LevelData { get; set; }
    }
}
