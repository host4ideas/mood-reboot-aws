using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;

namespace NugetMoodReboot.Helpers
{
    public class HelperJsonSession
    {
        public static T DeserializeObject<T>(string data)
        {
            T objeto = JsonConvert.DeserializeObject<T>(data)!;
            return objeto;
        }

        public static string SerializeObject(object obj)
        {
            string data = JsonConvert.SerializeObject(obj);
            return data;
        }
    }
}
