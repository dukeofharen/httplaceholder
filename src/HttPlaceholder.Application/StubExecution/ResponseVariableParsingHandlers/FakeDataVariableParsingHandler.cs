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
///     Response variable parsing handler that is used to insert fake data.
/// </summary>
internal class FakeDataVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly Lazy<string> _descriptionLazy;
    private readonly Lazy<string[]> _exampleLazy;
    private readonly IFakerService _fakerService;

    public FakeDataVariableParsingHandler(IFakerService fakerService)
    {
        _fakerService = fakerService;
        _exampleLazy = new Lazy<string[]>(InitializeExamples);
        _descriptionLazy = new Lazy<string>(InitializeDescription);
    }

    /// <inheritdoc />
    public override string Name => "fake_data";

    /// <inheritdoc />
    public override string FullName => ResponseVariableParsingResources.FakeData;

    /// <inheritdoc />
    public override string[] Examples => _exampleLazy.Value;

    public override string GetDescription() => _descriptionLazy.Value;

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken) =>
        matches
            .Where(match => match.Groups.Count >= 3)
            .Aggregate(input, InsertFakeData).AsTask();

    private string InsertFakeData(string current, Match match)
    {
        var fakeDataInput = ParseFakeDataInput(match.Groups[2].Value);
        return current.Replace(match.Value,
            _fakerService.GenerateFakeData(fakeDataInput.generator, fakeDataInput.locale,
                fakeDataInput.formatting));
    }

    internal (string locale, string generator, string formatting) ParseFakeDataInput(string input)
    {
        var parts = input.Split(':', 3);
        if (parts.Length == 0)
        {
            return (string.Empty, string.Empty, string.Empty);
        }

        var locale = string.Empty;
        var generator = string.Empty;
        var formatting = string.Empty;

        var locales = _fakerService.GetLocales();
        if (locales.Any(l => string.Equals(l, parts[0], StringComparison.OrdinalIgnoreCase)))
        {
            // First part is locale, so treat it like that.
            locale = parts[0];
            if (parts.Length > 1)
            {
                generator = parts[1];
            }

            if (parts.Length > 2)
            {
                formatting = parts[2];
            }
        }
        else
        {
            generator = parts[0];
            if (parts.Length > 1)
            {
                formatting = string.Join(':', parts.Skip(1).Take(parts.Length - 1));
            }
        }

        return (locale, generator, formatting);
    }

    private string[] InitializeExamples()
    {
        var result = _fakerService.GetLocales()
            .Select(locale => $"(({Name}:{locale}:first_name))")
            .ToList();
        foreach (var generator in _fakerService.GetGenerators())
        {
            result.Add($"(({Name}:{generator.Name}))");
            if (!string.IsNullOrWhiteSpace(generator.Formatting))
            {
                result.Add($"(({Name}:{generator.Name}:{generator.Formatting}))");
            }
        }

        return result.ToArray();
    }

    private string InitializeDescription() =>
        ResponseVariableParsingResources.FakeDataDescription
            .Replace("[LOCALES]",
                string.Join(", ", _fakerService.GetLocales().Select(l => $"_{l}_")));
}
