using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.Stubs
{
    /// <summary>
    /// A model for storing all conditions for a stub.
    /// </summary>
    public class StubConditionsDto : IMapFrom<StubConditionsModel>, IMapTo<StubConditionsModel>
    {
        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        [YamlMember(Alias = "method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [YamlMember(Alias = "url")]
        public StubUrlConditionDto Url { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [YamlMember(Alias = "body")]
        public IEnumerable<string> Body { get; set; }

        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        [YamlMember(Alias = "form")]
        public IEnumerable<StubFormDto> Form { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        [YamlMember(Alias = "headers")]
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the xpath.
        /// </summary>
        [YamlMember(Alias = "xpath")]
        public IEnumerable<StubXpathDto> Xpath { get; set; }

        /// <summary>
        /// Gets or sets the json path.
        /// </summary>
        [YamlMember(Alias = "jsonPath")]
        public IEnumerable<string> JsonPath { get; set; }

        /// <summary>
        /// Gets or sets the basic authentication.
        /// </summary>
        [YamlMember(Alias = "basicAuthentication")]
        public StubBasicAuthenticationDto BasicAuthentication { get; set; }

        /// <summary>
        /// Gets or sets the client ip.
        /// </summary>
        [YamlMember(Alias = "clientIp")]
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [YamlMember(Alias = "host")]
        public string Host { get; set; }
    }
}
