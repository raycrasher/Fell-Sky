using Newtonsoft.Json;
using System.IO;

namespace FellSky
{
    public interface IPersistable
    {
    }

    public static class Persistence
    {
        public static void SaveToFile(this IPersistable obj, string filename)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(obj, JsonSettings));            
        }

        public static T LoadFromFile<T>(string filename)
            where T :IPersistable
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filename), JsonSettings);
        }

        public static void SerializeToFile(this object obj, string filename)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(obj, JsonSettings));
        }

        public static T DeserializeFromFile<T>(string filename)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filename), JsonSettings);
        }

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            Formatting = Formatting.Indented
        };
    }
}
