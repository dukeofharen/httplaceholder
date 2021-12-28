using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Models.HAR;

public class PostData
{
    [JsonProperty("mimeType")]
    public string MimeType { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("params")]
    public PostDataItem[] Params { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}

public class PostDataItem
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("fileName")]
    public string FileName { get; set; }

    [JsonProperty("contentType")]
    public string ContentType { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }
}
