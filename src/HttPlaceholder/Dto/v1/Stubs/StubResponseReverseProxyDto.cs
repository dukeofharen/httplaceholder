using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.v1.Stubs
{
    /// <summary>
    /// A model for storing reverse proxy settings.
    /// </summary>
    public class StubResponseReverseProxyDto : IMapFrom<StubResponseReverseProxyModel>, IMapTo<StubResponseReverseProxyModel>
    {
        /// <summary>
        /// Gets or sets the URL where the request should be sent to. The request will be sent to exactly this URL.
        /// </summary>
        [YamlMember(Alias = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets whether the query string of the request to HttPlaceholder should be appended to the string that will be send to the proxy URL.
        /// </summary>
        [YamlMember(Alias = "appendQueryString")]
        public bool? AppendQueryString { get; set; }

        /// <summary>
        /// Gets or sets whether the path string of the request to HttPlaceholder should be appended to the string that will be send to the proxy URL.
        /// </summary>
        [YamlMember(Alias = "appendPath")]
        public bool? AppendPath { get; set; }

        /// <summary>
        /// Gets or sets whether the root URL of the response of the target web service should be replaced with the root URL of HttPlaceholder.
        /// </summary>
        [YamlMember(Alias = "replaceRootUrl")]
        public bool? ReplaceRootUrl { get; set; }
    }
}
