﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler for generating a random UUID and putting it in the response.
/// </summary>
internal class UuidResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "uuid";

    /// <inheritdoc />
    public override string FullName => ResponseVariableParsingResources.Uuid;

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.UuidDescription;

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken) =>
        matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, Guid.NewGuid().ToString())).AsTask();
}
