using System.Collections.Generic;
using HttPlaceholder.Domain.Enums;
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
        public bool? EnableDynamicMode { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        [YamlMember(Alias = "statusCode")]
        public int? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the response content type.
        /// </summary>
        [YamlMember(Alias = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [YamlMember(Alias = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the base64.
        /// </summary>
        [YamlMember(Alias = "base64")]
        public string Base64 { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        [YamlMember(Alias = "file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        [YamlMember(Alias = "headers")]
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the duration of the extra.
        /// </summary>
        [YamlMember(Alias = "extraDuration")]
        public int? ExtraDuration { get; set; }

        /// <summary>
        /// Gets or sets the json.
        /// </summary>
        [YamlMember(Alias = "json")]
        public string Json { get; set; }

        /// <summary>
        /// Gets or sets the XML.
        /// </summary>
        [YamlMember(Alias = "xml")]
        public string Xml { get; set; }

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        [YamlMember(Alias = "html")]
        public string Html { get; set; }

        /// <summary>
        /// Gets or sets the temporary redirect.
        /// </summary>
        [YamlMember(Alias = "temporaryRedirect")]
        public string TemporaryRedirect { get; set; }

        /// <summary>
        /// Gets or sets the permanent redirect.
        /// </summary>
        [YamlMember(Alias = "permanentRedirect")]
        public string PermanentRedirect { get; set; }

        /// <summary>
        /// Gets or sets the reverse proxy settings.
        /// </summary>
        [YamlMember(Alias = "reverseProxy")]
        public StubResponseReverseProxyModel ReverseProxy { get; set; }

        /// <summary>
        /// Gets or sets the line endings type.
        /// </summary>
        [YamlMember(Alias = "lineEndings")]
        public string LineEndings { get; set; }

        /// <summary>
        /// Gets or sets the stub image.
        /// </summary>
        [YamlMember(Alias = "image")]
        public StubResponseImageModel Image { get; set; }
    }
}
