using System.IO;
using System.Text;
using System.Text.Json;

namespace Shared
{
    /// <summary>
    /// Static class to serialize and deserialize json files to objects
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Deserializes jsontext to an object
        /// </summary>
        /// <typeparam name="T">The type of the object that the jsontext gets serialized into</typeparam>
        /// <param name="filePath">Path of the file from which the jsontext will be read</param>
        /// <returns>The object of type `T` serialized from the file</returns>
        public static T Deserialize<T>(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<T>(jsonString);
        }
        /// <summary>
        /// Serializes an object into jsontext which it then writes into a file
        /// </summary>
        /// <typeparam name="T">The type of the object that gets serialized into jsonText</typeparam>
        /// <param name="toSerialize">The object of type `T` that gets serialed into jsontext</param>
        /// <param name="filePath">The file that the jsontext gets written into</param>
        public static void Serialize<T>(this T toSerialize, string filePath)
        {
            string parsed = JsonSerializer.Serialize(toSerialize, new JsonSerializerOptions { WriteIndented = true });

            using FileStream writer = File.OpenWrite(filePath);
            writer.Write(Encoding.UTF8.GetBytes(parsed));
            writer.Close();
        }
    }
}
