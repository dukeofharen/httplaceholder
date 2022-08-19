using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert fake data.
/// </summary>
internal class FakeDataVariableParsingHandler : BaseVariableParsingHandler
{
    private readonly Lazy<string[]> _exampleLazy;
    private readonly IFakerService _fakerService;

    public FakeDataVariableParsingHandler(IFileService fileService, IFakerService fakerService) : base(fileService)
    {
        _fakerService = fakerService;
        _exampleLazy = new Lazy<string[]>(InitializeExamples);
    }

    /// <inheritdoc />
    public override string Name => "fake_data";

    /// <inheritdoc />
    public override string FullName => "Fake data";

    /// <inheritdoc />
    public override string[] Examples => _exampleLazy.Value;

    public override string GetDescription()
    {
        var description = base.GetDescription();
        description = description.Replace("[LOCALES]",
            string.Join(", ", _fakerService.GetLocales().Select(l => $"_{l}_")));
        return description;
    }

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var enumerable = matches as Match[] ?? matches.ToArray();
        if (!enumerable.Any())
        {
            return input;
        }

        foreach (var match in enumerable)
        {
            var fakeDataInput = ParseFakeDataInput(match.Groups[2].Value);
            input = input.Replace(match.Value,
                _fakerService.GenerateFakeData(fakeDataInput.generator, fakeDataInput.locale,
                    fakeDataInput.formatting));
        }

        return input;
    }

    private (string locale, string generator, string formatting) ParseFakeDataInput(string input)
    {
        var parts = input.Split(':');
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
                formatting = parts[1];
            }
        }

        return (locale, generator, formatting);
    }

    private string[] InitializeExamples()
    {
        // TODO add examples with locale
        var result = new List<string>();
        foreach (var generator in _fakerService.GetGenerators())
        {
            result.Add($"((fake_data:{generator.Name}))");
            if (!string.IsNullOrWhiteSpace(generator.Formatting))
            {
                result.Add($"((fake_data:{generator.Name}:{generator.Formatting}))");
            }
        }

        return result.ToArray();
    }
}
