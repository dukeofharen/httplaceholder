﻿using System.ComponentModel.DataAnnotations;
using HttPlaceholder.Common.Validation;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing all information about a stub.
/// </summary>
public class StubModel
{
    /// <summary>
    ///     Gets or sets the identifier.
    /// </summary>
    [YamlMember(Alias = "id")]
    [Required]
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets the conditions.
    /// </summary>
    [YamlMember(Alias = "conditions")]
    [ValidateObject]
    public StubConditionsModel Conditions { get; set; } = new();

    /// <summary>
    ///     Gets or sets the response.
    /// </summary>
    [YamlMember(Alias = "response")]
    [Required]
    [ValidateObject]
    public StubResponseModel Response { get; set; } = new();

    /// <summary>
    ///     Gets or sets the priority.
    /// </summary>
    [YamlMember(Alias = "priority")]
    public int Priority { get; set; }

    /// <summary>
    ///     Gets or sets the tenant.
    /// </summary>
    [YamlMember(Alias = "tenant")]
    public string Tenant { get; set; }

    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    [YamlMember(Alias = "description")]
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether this stub is enabled or not.
    /// </summary>
    [YamlMember(Alias = "enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Gets or sets the scenario the stub is executed under.
    /// </summary>
    [YamlMember(Alias = "scenario")]
    public string Scenario { get; set; }
}
