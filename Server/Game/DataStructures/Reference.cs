using System.Text.Json.Serialization;

namespace Server.Game.DataStructures
{
    internal class Reference
    {
        [JsonPropertyName("level-data-path")]
        public string LevelDataPath { get; set; }
        [JsonPropertyName("level-path")]
        public string LevelPath { get; set; }
        [JsonPropertyName("item-data-path")]
        public string ItemDataPath { get; set; }
        [JsonPropertyName("levels")]
        public string[] Levels { get; set; }
    }
}
