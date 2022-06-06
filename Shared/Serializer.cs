using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Shared
{
    public static class Serializer
    {
        public static T Deserialize<T>(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<T>(jsonString);
        }
        public static void Serialize<T>(this T toSerialize, string filePath)
        {
            string parsed = JsonSerializer.Serialize(toSerialize, new JsonSerializerOptions { WriteIndented = true });

            using FileStream writer = File.OpenWrite(filePath);
            writer.Write(Encoding.UTF8.GetBytes(parsed));
            writer.Close();
        }
    }
}
