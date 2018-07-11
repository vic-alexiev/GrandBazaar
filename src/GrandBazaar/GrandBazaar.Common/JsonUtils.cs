using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GrandBazaar.Common
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerSettings Settings;

        static JsonUtils()
        {
            Settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static string Serialize(object value, bool formatted = true)
        {
            string output = JsonConvert.SerializeObject(
                value,
                formatted ? Formatting.Indented : Formatting.None,
                Settings);

            return output;
        }

        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, Settings);
        }
    }
}
