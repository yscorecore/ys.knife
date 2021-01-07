using System.Text.Json;

namespace YS.Knife.Data
{
    public static class Json
    {
        private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
        };

        private static JsonSerializerOptions JsonOptionsWithIndented =
            new JsonSerializerOptions(JsonOptions) { WriteIndented = true };

        public static string Serialize(object obj, bool withIndented = false)
        {
            return JsonSerializer.Serialize(obj, withIndented ? JsonOptionsWithIndented : JsonOptions);
        }

        public static T DeSerialize<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content);
        }
        public static string ToJsonString(this object obj, bool withIndented = false)
        {
            return Serialize(obj, withIndented);
        }

        public static string Dump(this object obj, bool withIndented = false) => obj.ToJsonString(withIndented);
        public static T FromFile<T>(string filePath)
        {
            return JsonSerializer.Deserialize<T>(System.IO.File.ReadAllText(filePath));
        }

        public static void DumpToFile(this object obj, string filePath, bool withIndented = true)
        {
            System.IO.File.WriteAllText(filePath, obj.Dump(withIndented));
        }

    }
}
