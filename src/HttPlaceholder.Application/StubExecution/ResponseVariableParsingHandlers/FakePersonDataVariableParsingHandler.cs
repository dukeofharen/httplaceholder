using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bogus;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert fake person data.
/// </summary>
internal class FakePersonDataVariableParsingHandler : BaseVariableParsingHandler
{
    public FakePersonDataVariableParsingHandler(IFileService fileService) : base(fileService)
    {
    }

    /// <inheritdoc />
    public override string Name => "fake_person_data";

    /// <inheritdoc />
    public override string FullName => "Fake person data";

    /// <inheritdoc />
    public override string[] Examples => new string[] { };

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var faker = new Faker();
        return input;
    }
}
