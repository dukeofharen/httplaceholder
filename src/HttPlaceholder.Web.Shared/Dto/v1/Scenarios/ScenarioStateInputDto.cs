﻿using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Web.Shared.Dto.v1.Scenarios;

/// <summary>
///     A model that is used to set the scenario.
/// </summary>
public class ScenarioStateInputDto : IHaveCustomMapping
{
    /// <summary>
    ///     Gets or sets the state the scenario is in.
    /// </summary>
    public string State { get; init; }

    /// <summary>
    ///     Gets or sets the number of times the scenario has been hit.
    /// </summary>
    public int HitCount { get; init; } = -1;

    /// <inheritdoc />
    public void CreateMappings(Profile configuration) => configuration
        .CreateMap<ScenarioStateInputDto, ScenarioStateModel>()
        .ForMember(src => src.Scenario, opt => opt.Ignore());
}
