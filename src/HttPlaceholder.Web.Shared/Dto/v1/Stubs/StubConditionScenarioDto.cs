﻿using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Dto.v1.Stubs;

/// <summary>
///     A model for storing all scenario conditions for a stub
/// </summary>
public class StubConditionScenarioDto : IMapFrom<StubConditionScenarioModel>, IMapTo<StubConditionScenarioModel>
{
    /// <summary>
    ///     Gets or sets the inclusive min hit count.
    /// </summary>
    [YamlMember(Alias = "minHits")]
    public int? MinHits { get; set; }

    /// <summary>
    ///     Gets or sets the exclusive max hit count.
    /// </summary>
    [YamlMember(Alias = "maxHits")]
    public int? MaxHits { get; set; }

    /// <summary>
    ///     Gets or sets the exact hits count.
    /// </summary>
    [YamlMember(Alias = "exactHits")]
    public int? ExactHits { get; set; }

    /// <summary>
    ///     Gets or sets the state the scenario should be in.
    /// </summary>
    [YamlMember(Alias = "scenarioState")]
    public string ScenarioState { get; set; }
}
