using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using Bogus;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class FakerService : IFakerService, ISingletonService
{
    private const string DefaultDatetimeFormat = "yyyy-MM-dd HH:mm:ss";

    private static readonly string[] _locales =
    {
        "af_ZA", "fr_CH", "ar", "ge", "az", "hr", "cz", "id_ID", "de", "it", "de_AT", "ja", "de_CH", "ko", "el",
        "lv", "en", "nb_NO", "en_AU", "ne", "en_AU_ocker", "nl", "en_BORK", "nl_BE", "en_CA", "pl", "en_GB",
        "pt_BR", "en_IE", "pt_PT", "en_IND", "ro", "en_NG", "ru", "en_US", "sk", "en_ZA", "sv", "es", "tr", "es_MX",
        "uk", "fa", "vi", "fi", "zh_CN", "fr", "zh_TW", "fr_CA", "zu_ZA"
    };

    private static readonly FakeDataGeneratorModel[] _generators =
    {
        new("zipcode", (faker, _, _) => faker.Address.ZipCode()), new("city", (faker, _, _) => faker.Address.City()),
        new("street_address", (faker, _, _) => faker.Address.StreetAddress()),
        new("city_prefix", (faker, _, _) => faker.Address.CityPrefix()),
        new("city_suffix", (faker, _, _) => faker.Address.CitySuffix()),
        new("street_name", (faker, _, _) => faker.Address.StreetName()),
        new("building_number", (faker, _, _) => faker.Address.BuildingNumber()),
        new("street_suffix", (faker, _, _) => faker.Address.StreetSuffix()),
        new("secondary_address", (faker, _, _) => faker.Address.SecondaryAddress()),
        new("county", (faker, _, _) => faker.Address.County()),
        new("country", (faker, _, _) => faker.Address.Country()),
        new("full_address", (faker, _, _) => faker.Address.FullAddress()),
        new("country_code", (faker, _, _) => faker.Address.CountryCode()),
        new("state", (faker, _, _) => faker.Address.State()),
        new("state_abbreviation", (faker, _, _) => faker.Address.StateAbbr()),
        new("direction", (faker, _, _) => faker.Address.Direction()),
        new("cardinal_direction", (faker, _, _) => faker.Address.CardinalDirection()),
        new("ordinal_direction", (faker, _, _) => faker.Address.OrdinalDirection()),
        new("first_name", (faker, _, _) => faker.Name.FirstName()),
        new("last_name", (faker, _, _) => faker.Name.LastName()),
        new("full_name", (faker, _, _) => faker.Name.FullName()), new("prefix", (faker, _, _) => faker.Name.Prefix()),
        new("suffix", (faker, _, _) => faker.Name.Suffix()), new("job_title", (faker, _, _) => faker.Name.JobTitle()),
        new("job_descriptor", (faker, _, _) => faker.Name.JobDescriptor()),
        new("job_area", (faker, _, _) => faker.Name.JobArea()), new("job_type", (faker, _, _) => faker.Name.JobType()),
        new("phone_number", (faker, _, _) => faker.Phone.PhoneNumber()),
        new("email", (faker, _, _) => faker.Internet.Email()),
        new("example_email", (faker, _, _) => faker.Internet.ExampleEmail()),
        new("user_name", (faker, _, _) => faker.Internet.UserName()),
        new("user_name_unicode", (faker, _, _) => faker.Internet.UserNameUnicode()),
        new("domain_name", (faker, _, _) => faker.Internet.DomainName()),
        new("domain_word", (faker, _, _) => faker.Internet.DomainWord()),
        new("domain_suffix", (faker, _, _) => faker.Internet.DomainSuffix()),
        new("ip", (faker, _, _) => faker.Internet.Ip()), new("port", (faker, _, _) => faker.Internet.Port().ToString()),
        new("ipv6", (faker, _, _) => faker.Internet.Ipv6()),
        new("user_agent", (faker, _, _) => faker.Internet.UserAgent()),
        new("mac", (faker, _, _) => faker.Internet.Mac()), new("password", (faker, _, _) => faker.Internet.Password()),
        new("color", (faker, _, _) => faker.Internet.Color()),
        new("protocol", (faker, _, _) => faker.Internet.Protocol()), new("url", (faker, _, _) => faker.Internet.Url()),
        new("url_with_path", (faker, _, _) => faker.Internet.UrlWithPath()),
        new("url_rooted_path", (faker, _, _) => faker.Internet.UrlRootedPath()),
        new("word", (faker, _, _) => faker.Lorem.Word()), new("words", "3",
            (faker, formatting, _) => string.Join(' ', faker.Lorem.Words(ConvertToInt(formatting, 3)))),
        new("letter", "1", (faker, formatting, _) => faker.Lorem.Letter(ConvertToInt(formatting, 1))),
        new("sentence", "5", (faker, formatting, _) => faker.Lorem.Sentence(ConvertToInt(formatting, 5))),
        new("sentences", "3", (faker, formatting, _) => faker.Lorem.Sentences(ConvertToInt(formatting, 3))),
        new("paragraph", "3", (faker, formatting, _) => faker.Lorem.Paragraph(ConvertToInt(formatting, 3))),
        new("paragraphs", "3", (faker, formatting, _) => faker.Lorem.Paragraphs(ConvertToInt(formatting, 3))),
        new("text", (faker, _, _) => faker.Lorem.Text()),
        new("lines", "3", (faker, formatting, _) => faker.Lorem.Lines(ConvertToInt(formatting, 3))),
        new("slug", "3", (faker, formatting, _) => faker.Lorem.Slug(ConvertToInt(formatting, 3))), new("past",
            DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Past(), formatting, locale)),
        new("past_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.PastOffset(), formatting, locale)),
        new("soon", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Soon(), formatting, locale)),
        new("soon_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.SoonOffset(), formatting, locale)),
        new("future", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Future(), formatting, locale)),
        new("future_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.FutureOffset(), formatting, locale)),
        new("recent", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Recent(), formatting, locale)),
        new("recent_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.RecentOffset(), formatting, locale)),
        new("month", (faker, _, _) => faker.Date.Month()), new("weekday", (faker, _, _) => faker.Date.Weekday()),
        new("timezone_string", (faker, _, _) => faker.Date.TimeZoneString()),
        new("account", (faker, _, _) => faker.Finance.Account()),
        new("account_name", (faker, _, _) => faker.Finance.AccountName()), new("amount", "0.00",
            (faker, formatting, locale) => ConvertDecimal(faker.Finance.Amount(), formatting, locale)),
        new("currency_name", (faker, _, _) => faker.Finance.Currency().Description),
        new("currency_code", (faker, _, _) => faker.Finance.Currency().Code),
        new("credit_card_number", (faker, _, _) => faker.Finance.CreditCardNumber()),
        new("credit_card_cvv", (faker, _, _) => faker.Finance.CreditCardCvv()),
        new("routing_number", (faker, _, _) => faker.Finance.RoutingNumber()),
        new("bic", (faker, _, _) => faker.Finance.Bic()), new("iban", (faker, _, _) => faker.Finance.Iban()),
        new("bitcoin_address", (faker, _, _) => faker.Finance.BitcoinAddress()),
        new("ethereum_address", (faker, _, _) => faker.Finance.EthereumAddress()),
        new("litecoin_address", (faker, _, _) => faker.Finance.LitecoinAddress()),
        new("file_name", (faker, _, _) => faker.System.FileName()),
        new("directory_path", (faker, _, _) => faker.System.DirectoryPath()),
        new("file_path", (faker, _, _) => faker.System.FilePath()),
        new("common_file_name", (faker, _, _) => faker.System.CommonFileName()),
        new("mime_type", (faker, _, _) => faker.System.MimeType()),
        new("common_file_type", (faker, _, _) => faker.System.CommonFileType()),
        new("common_file_ext", (faker, _, _) => faker.System.CommonFileExt()),
        new("file_type", (faker, _, _) => faker.System.FileType()),
        new("file_ext", (faker, _, _) => faker.System.FileExt()), new("semver", (faker, _, _) => faker.System.Semver()),
        new("android_id", (faker, _, _) => faker.System.AndroidId()),
        new("apple_push_token", (faker, _, _) => faker.System.ApplePushToken()),
        new("department", (faker, _, _) => faker.Commerce.Department()),
        new("price", (faker, _, _) => faker.Commerce.Price()),
        new("product_name", (faker, _, _) => faker.Commerce.ProductName()),
        new("product", (faker, _, _) => faker.Commerce.Product()),
        new("product_adjective", (faker, _, _) => faker.Commerce.ProductAdjective()),
        new("product_description", (faker, _, _) => faker.Commerce.ProductDescription()),
        new("ean8", (faker, _, _) => faker.Commerce.Ean8()), new("ean13", (faker, _, _) => faker.Commerce.Ean13())
    };

    private readonly ConcurrentDictionary<string, Faker> _cachedFakers = new();
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
            if (!string.IsNullOrWhiteSpace(locale))
            {
                _logger.LogDebug($"Locale '{locale}' not found. Choose from: {string.Join(',', _locales)}.");
            }

            locale = "en";
        }

        var faker = _cachedFakers.GetOrAdd(locale, l => new Faker(l));
        return generatorModel.FormatterFunction(faker,
            string.IsNullOrWhiteSpace(format) ? generatorModel.Formatting : format, locale);
    }

    private static int ConvertToInt(string input, int defaultValue) =>
        !int.TryParse(input, out var result) ? defaultValue : result;

    private static string ConvertDateTime(DateTime input, string formatting, string locale) =>
        input.ToString(formatting, GetCultureInfo(locale));

    private static string ConvertDateTimeOffset(DateTimeOffset input, string formatting, string locale) =>
        input.ToString(formatting, GetCultureInfo(locale));

    private static string ConvertDecimal(decimal input, string formatting, string locale) =>
        input.ToString(formatting, GetCultureInfo(locale));

    private static CultureInfo GetCultureInfo(string locale)
    {
        var cultureInfo = CultureInfo.InvariantCulture;
        try
        {
            cultureInfo = new CultureInfo(locale);
        }
        catch (CultureNotFoundException) { }

        return cultureInfo;
    }
}
