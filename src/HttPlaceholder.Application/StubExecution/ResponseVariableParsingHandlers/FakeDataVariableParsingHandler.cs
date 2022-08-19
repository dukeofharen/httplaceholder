using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bogus;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert fake person data.
/// </summary>
internal class FakeDataVariableParsingHandler : BaseVariableParsingHandler
{
    private static readonly string[] _locales =
    {
        "af_ZA", "fr_CH", "ar", "ge", "az", "hr", "cz", "id_ID", "de", "it", "de_AT", "ja", "de_CH", "ko", "el",
        "lv", "en", "nb_NO", "en_AU", "ne", "en_AU_ocker", "nl", "en_BORK", "nl_BE", "en_CA", "pl", "en_GB",
        "pt_BR", "en_IE", "pt_PT", "en_IND", "ro", "en_NG", "ru", "en_US", "sk", "en_ZA", "sv", "es", "tr", "es_MX",
        "uk", "fa", "vi", "fi", "zh_CN", "fr", "zh_TW", "fr_CA", "zu_ZA"
    };

    public FakeDataVariableParsingHandler(IFileService fileService) : base(fileService)
    {
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
        description = description.Replace("[LOCALES]", string.Join(", ", _locales.Select(l => $"_{l}_")));
        return description;
    }

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var faker = new Faker();
        return input;
    }
}
