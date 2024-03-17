using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Attributes;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model for storing all conditions for a stub.
/// </summary>
[CustomOpenApi]
public class StubConditionsDto : IMapFrom<StubConditionsModel>, IMapTo<StubConditionsModel>
{
    /// <summary>
    ///     Gets or sets the method.
    /// </summary>
    [YamlMember(Alias = "method")]
    [OneOf(Types = [typeof(string), typeof(string[])])]
    public object Method { get; set; }

    /// <summary>
    ///     Gets or sets the URL.
    /// </summary>
    [YamlMember(Alias = "url")]
    public StubUrlConditionDto Url { get; set; }

    /// <summary>
    ///     Gets or sets the body.
    /// </summary>
    [YamlMember(Alias = "body")]
    [OneOf(ItemsTypes = [typeof(string), typeof(StubConditionStringCheckingDto)])]
    public IEnumerable<object> Body { get; set; }

    /// <summary>
    ///     Gets or sets the form.
    /// </summary>
    [YamlMember(Alias = "form")]
    public IEnumerable<StubFormDto> Form { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    [YamlMember(Alias = "headers")]
    [OneOf(AdditionalPropertiesTypes = [typeof(string), typeof(StubConditionStringCheckingDto)])]
    public IDictionary<string, object> Headers { get; set; }

    /// <summary>
    ///     Gets or sets the xpath.
    /// </summary>
    [YamlMember(Alias = "xpath")]
    public IEnumerable<StubXpathDto> Xpath { get; set; }

    /// <summary>
    ///     Gets or sets the json path.
    /// </summary>
    [YamlMember(Alias = "jsonPath")]
    [OneOf(ItemsTypes = [typeof(string), typeof(StubJsonPathDto)])]
    public IEnumerable<object> JsonPath { get; set; }

    /// <summary>
    ///     Gets or sets the basic authentication.
    /// </summary>
    [YamlMember(Alias = "basicAuthentication")]
    public StubBasicAuthenticationDto BasicAuthentication { get; set; }

    /// <summary>
    ///     Gets or sets the client ip.
    /// </summary>
    [YamlMember(Alias = "clientIp")]
    public string ClientIp { get; set; }

    /// <summary>
    ///     Gets or sets the host.
    /// </summary>
    [YamlMember(Alias = "host")]
    [OneOf(Types = [typeof(string), typeof(StubConditionStringCheckingDto)])]
    public object Host { get; set; }

    /// <summary>
    ///     Gets or sets the JSON condition model.
    /// </summary>
    [YamlMember(Alias = "json")]
    public object Json { get; set; }

    /// <summary>
    ///     Gets or sets the scenario conditions model.
    /// </summary>
    [YamlMember(Alias = "scenario")]
    public StubConditionScenarioDto Scenario { get; set; }
}
