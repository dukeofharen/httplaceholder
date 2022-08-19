using System;
using System.Linq;
using Bogus;
using HttPlaceholder.Application.StubExecution.Models;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
internal class FakerService : IFakerService
{
    private static readonly string[] _locales =
    {
        "af_ZA", "fr_CH", "ar", "ge", "az", "hr", "cz", "id_ID", "de", "it", "de_AT", "ja", "de_CH", "ko", "el",
        "lv", "en", "nb_NO", "en_AU", "ne", "en_AU_ocker", "nl", "en_BORK", "nl_BE", "en_CA", "pl", "en_GB",
        "pt_BR", "en_IE", "pt_PT", "en_IND", "ro", "en_NG", "ru", "en_US", "sk", "en_ZA", "sv", "es", "tr", "es_MX",
        "uk", "fa", "vi", "fi", "zh_CN", "fr", "zh_TW", "fr_CA", "zu_ZA"
    };

    private static readonly FakeDataGeneratorModel[] _generators =
    {
        new("zipcode", (faker, formatting) => faker.Address.ZipCode())
    };

    private readonly ILogger<FakerService> _logger;

    public FakerService(ILogger<FakerService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public FakeDataGeneratorModel[] GetGenerators() => _generators;

    /// <inheritdoc />
    public string[] GetLocales() => _locales;

    /// <inheritdoc />
    public string GenerateFakeData(string generator, string locale, string format)
    {
        var generatorModel =
            _generators.FirstOrDefault(g => string.Equals(generator, g.Name, StringComparison.OrdinalIgnoreCase));
        if (generatorModel == null)
        {
            _logger.LogWarning($"No fake data generator found with name '{generator}'.");
            return string.Empty;
        }

        if (!_locales.Any(l => string.Equals(l, locale, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogDebug($"Locale '{locale}' not found. Choose from: {string.Join(',', _locales)}.");
            locale = "en";
        }

        var faker = new Faker(locale);
        return generatorModel.FormatterFunction(faker, generatorModel.Formatting);
    }
}
