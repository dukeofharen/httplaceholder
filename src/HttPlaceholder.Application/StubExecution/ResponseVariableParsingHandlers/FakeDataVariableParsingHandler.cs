using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert fake person data.
/// </summary>
internal class FakeDataVariableParsingHandler : BaseVariableParsingHandler
{
    private readonly IFakerService _fakerService;

    public FakeDataVariableParsingHandler(IFileService fileService, IFakerService fakerService) : base(fileService)
    {
        _fakerService = fakerService;
    }

    /// <inheritdoc />
    public override string Name => "fake_data";

    /// <inheritdoc />
    public override string FullName => "Fake person data";

    /// <inheritdoc />
    public override string[] Examples => new string[] { };

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
            input = input.Replace(match.Value, _fakerService.GenerateFakeData(fakeDataInput.generator, fakeDataInput.locale, fakeDataInput.formatting));
        }

        return input;
    }

    private static (string locale, string generator, string formatting) ParseFakeDataInput(string input)
    {
        var parts = input.Split(':');
        return parts.Length switch
        {
            1 => (string.Empty, parts[0], string.Empty),
            2 => (parts[0], parts[1], string.Empty),
            _ => (parts[0], parts[1], parts[2])
        };
    }
}
