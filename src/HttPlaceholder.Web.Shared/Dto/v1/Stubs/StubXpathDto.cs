﻿using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model for storing information about the XPath condition checker.
/// </summary>
public class StubXpathDto : IMapFrom<StubXpathModel>, IMapTo<StubXpathModel>
{
    /// <summary>
    ///     Gets or sets the query string.
    /// </summary>
    [YamlMember(Alias = "queryString")]
    public string QueryString { get; set; }

    /// <summary>
    ///     Gets or sets the namespaces.
    /// </summary>
    [YamlMember(Alias = "namespaces")]
    public IDictionary<string, string> Namespaces { get; set; }
}
