﻿using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing all conditions for a stub.
/// </summary>
public class StubConditionsModel
{
    /// <summary>
    ///     Gets or sets the method.
    /// </summary>
    [YamlMember(Alias = "method")]
    public object Method { get; set; }

    /// <summary>
    ///     Gets or sets the URL.
    /// </summary>
    [YamlMember(Alias = "url")]
    public StubUrlConditionModel Url { get; set; } = new();

    /// <summary>
    ///     Gets or sets the body.
    /// </summary>
    [YamlMember(Alias = "body")]
    public IEnumerable<object> Body { get; set; }

    /// <summary>
    ///     Gets or sets the form.
    /// </summary>
    [YamlMember(Alias = "form")]
    public IEnumerable<StubFormModel> Form { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    [YamlMember(Alias = "headers")]
    public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

    /// <summary>
    ///     Gets or sets the xpath.
    /// </summary>
    [YamlMember(Alias = "xpath")]
    public IEnumerable<StubXpathModel> Xpath { get; set; }

    /// <summary>
    ///     Gets or sets the json path.
    /// </summary>
    [YamlMember(Alias = "jsonPath")]
    public IEnumerable<object> JsonPath { get; set; }

    /// <summary>
    ///     Gets or sets the basic authentication.
    /// </summary>
    [YamlMember(Alias = "basicAuthentication")]
    public StubBasicAuthenticationModel BasicAuthentication { get; set; }

    /// <summary>
    ///     Gets or sets the client ip.
    /// </summary>
    [YamlMember(Alias = "clientIp")]
    public string ClientIp { get; set; }

    /// <summary>
    ///     Gets or sets the host.
    /// </summary>
    [YamlMember(Alias = "host")]
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
    public StubConditionScenarioModel Scenario { get; set; }
}
