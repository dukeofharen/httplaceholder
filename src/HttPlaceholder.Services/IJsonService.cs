namespace HttPlaceholder.Services
{
    public interface IJsonService
    {
        T DeserializeObject<T>(string value);

        string SerializeObject(object value);
    }
}
