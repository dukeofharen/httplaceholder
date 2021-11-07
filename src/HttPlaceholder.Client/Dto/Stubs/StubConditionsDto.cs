using System.Collections.Generic;

namespace HttPlaceholder.Client.Dto.Stubs
{
    /// <summary>
    /// A model for storing all conditions for a stub.
    /// </summary>
    public class StubConditionsDto
    {
        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public StubUrlConditionDto Url { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public IEnumerable<string> Body { get; set; }

        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        public IEnumerable<StubFormDto> Form { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the xpath.
        /// </summary>
        public IEnumerable<StubXpathDto> Xpath { get; set; }

        /// <summary>
        /// Gets or sets the json path.
        /// </summary>
        public IEnumerable<object> JsonPath { get; set; }

        /// <summary>
        /// Gets or sets the basic authentication.
        /// </summary>
        public StubBasicAuthenticationDto BasicAuthentication { get; set; }

        /// <summary>
        /// Gets or sets the client ip.
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the JSON condition model.
        /// </summary>
        public object Json { get; set; }

        /// <summary>
        /// Gets or sets the scenario conditions model.
        /// </summary>
        public StubConditionScenarioDto Scenario { get; set; }
    }
}
