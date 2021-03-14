using System.Collections.Generic;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing all possible response parameters for a stub.
    /// </summary>
    public class StubResponseModel
    {
        /// <summary>
        /// Gets or sets whether dynamic mode is on.
        /// </summary>
        [YamlMember(Alias = "enableDynamicMode")]
        [JsonProperty("enableDynamicMode")]
        public bool? EnableDynamicMode { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        [YamlMember(Alias = "statusCode")]
        [JsonProperty("statusCode")]
        public int? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the response content type.
        /// </summary>
        [YamlMember(Alias = "contentType")]
        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [YamlMember(Alias = "text")]
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the base64.
        /// </summary>
        [YamlMember(Alias = "base64")]
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        [YamlMember(Alias = "file")]
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        [YamlMember(Alias = "headers")]
        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the duration of the extra.
        /// </summary>
        [YamlMember(Alias = "extraDuration")]
        [JsonProperty("extraDuration")]
        public int? ExtraDuration { get; set; }

        /// <summary>
        /// Gets or sets the json.
        /// </summary>
        [YamlMember(Alias = "json")]
        [JsonProperty("json")]
        public string Json { get; set; }

        /// <summary>
        /// Gets or sets the XML.
        /// </summary>
        [YamlMember(Alias = "xml")]
        [JsonProperty("xml")]
        public string Xml { get; set; }

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        [YamlMember(Alias = "html")]
        [JsonProperty("html")]
        public string Html { get; set; }

        /// <summary>
        /// Gets or sets the temporary redirect.
        /// </summary>
        [YamlMember(Alias = "temporaryRedirect")]
        [JsonProperty("temporaryRedirect")]
        public string TemporaryRedirect { get; set; }

        /// <summary>
        /// Gets or sets the permanent redirect.
        /// </summary>
        [YamlMember(Alias = "permanentRedirect")]
        [JsonProperty("permanentRedirect")]
        public string PermanentRedirect { get; set; }

        /// <summary>
        /// Gets or sets the reverse proxy settings.
        /// </summary>
        [YamlMember(Alias = "reverseProxy")]
        [JsonProperty("reverseProxy")]
        public StubResponseReverseProxyModel ReverseProxy { get; set; }

        /// <summary>
        /// Gets or sets the line endings type.
        /// </summary>
        [YamlMember(Alias = "lineEndings")]
        [JsonProperty("lineEndings")]
        public string LineEndings { get; set; }

        /// <summary>
        /// Gets or sets the stub image.
        /// </summary>
        [YamlMember(Alias = "image")]
        [JsonProperty("image")]
        public StubResponseImageModel Image { get; set; }
    }
}
