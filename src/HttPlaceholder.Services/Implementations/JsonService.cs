using Newtonsoft.Json;

namespace HttPlaceholder.Services.Implementations
{
    internal class JsonService : IJsonService
    {
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
