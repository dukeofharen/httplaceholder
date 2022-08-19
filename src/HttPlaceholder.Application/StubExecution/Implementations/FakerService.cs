using System;
using System.Globalization;
using System.Linq;
using Bogus;
using HttPlaceholder.Application.StubExecution.Models;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
internal class FakerService : IFakerService
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
        new("zipcode", (faker, formatting, locale) => faker.Address.ZipCode()),
        new("city", (faker, formatting, locale) => faker.Address.City()),
        new("street_address", (faker, formatting, locale) => faker.Address.StreetAddress()),
        new("city_prefix", (faker, formatting, locale) => faker.Address.CityPrefix()),
        new("city_suffix", (faker, formatting, locale) => faker.Address.CitySuffix()),
        new("street_name", (faker, formatting, locale) => faker.Address.StreetName()),
        new("building_number", (faker, formatting, locale) => faker.Address.BuildingNumber()),
        new("street_suffix", (faker, formatting, locale) => faker.Address.StreetSuffix()),
        new("secondary_address", (faker, formatting, locale) => faker.Address.SecondaryAddress()),
        new("county", (faker, formatting, locale) => faker.Address.County()),
        new("country", (faker, formatting, locale) => faker.Address.Country()),
        new("full_address", (faker, formatting, locale) => faker.Address.FullAddress()),
        new("country_code", (faker, formatting, locale) => faker.Address.CountryCode()),
        new("state", (faker, formatting, locale) => faker.Address.State()),
        new("state_abbreviation", (faker, formatting, locale) => faker.Address.StateAbbr()),
        new("direction", (faker, formatting, locale) => faker.Address.Direction()),
        new("cardinal_direction", (faker, formatting, locale) => faker.Address.CardinalDirection()),
        new("ordinal_direction", (faker, formatting, locale) => faker.Address.OrdinalDirection()),
        new("first_name", (faker, formatting, locale) => faker.Name.FirstName()),
        new("last_name", (faker, formatting, locale) => faker.Name.LastName()),
        new("full_name", (faker, formatting, locale) => faker.Name.FullName()),
        new("prefix", (faker, formatting, locale) => faker.Name.Prefix()),
        new("suffix", (faker, formatting, locale) => faker.Name.Suffix()),
        new("job_title", (faker, formatting, locale) => faker.Name.JobTitle()),
        new("job_descriptor", (faker, formatting, locale) => faker.Name.JobDescriptor()),
        new("job_area", (faker, formatting, locale) => faker.Name.JobArea()),
        new("job_type", (faker, formatting, locale) => faker.Name.JobType()),
        new("phone_number", (faker, formatting, locale) => faker.Phone.PhoneNumber()),
        new("email", (faker, formatting, locale) => faker.Internet.Email()),
        new("example_email", (faker, formatting, locale) => faker.Internet.ExampleEmail()),
        new("user_name", (faker, formatting, locale) => faker.Internet.UserName()),
        new("user_name_unicode", (faker, formatting, locale) => faker.Internet.UserNameUnicode()),
        new("domain_name", (faker, formatting, locale) => faker.Internet.DomainName()),
        new("domain_word", (faker, formatting, locale) => faker.Internet.DomainWord()),
        new("domain_suffix", (faker, formatting, locale) => faker.Internet.DomainSuffix()),
        new("ip", (faker, formatting, locale) => faker.Internet.Ip()),
        new("port", (faker, formatting, locale) => faker.Internet.Port().ToString()),
        new("ipv6", (faker, formatting, locale) => faker.Internet.Ipv6()),
        new("user_agent", (faker, formatting, locale) => faker.Internet.UserAgent()),
        new("mac", (faker, formatting, locale) => faker.Internet.Mac()),
        new("password", (faker, formatting, locale) => faker.Internet.Password()),
        new("color", (faker, formatting, locale) => faker.Internet.Color()),
        new("protocol", (faker, formatting, locale) => faker.Internet.Protocol()),
        new("url", (faker, formatting, locale) => faker.Internet.Url()),
        new("url_with_path", (faker, formatting, locale) => faker.Internet.UrlWithPath()),
        new("url_rooted_path", (faker, formatting, locale) => faker.Internet.UrlRootedPath()),
        new("lorem", (faker, formatting, locale) => faker.Lorem.Word()),
        new("words", "3",
            (faker, formatting, locale) => string.Join(' ', faker.Lorem.Words(ConvertToInt(formatting, 3)))),
        new("letter", "1", (faker, formatting, locale) => faker.Lorem.Letter()), // TODO formatting
        new("sentence", "5", (faker, formatting, locale) => faker.Lorem.Sentence()), // TODO formatting
        new("sentences", "3", (faker, formatting, locale) => faker.Lorem.Sentences()), // TODO formatting
        new("paragraph", "3", (faker, formatting, locale) => faker.Lorem.Paragraph()), // TODO formatting
        new("paragraphs", "3", (faker, formatting, locale) => faker.Lorem.Paragraphs()), // TODO formatting
        new("text", (faker, formatting, locale) => faker.Lorem.Text()),
        new("lines", "3", (faker, formatting, locale) => faker.Lorem.Lines()), // TODO formatting
        new("slug", "3", (faker, formatting, locale) => faker.Lorem.Slug()), // TODO formatting
        new("past", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.Past().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("past_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.PastOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("soon", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.Soon().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("soon_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.SoonOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("future", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.Future().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("future_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.FutureOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("recent", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.Recent().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("recent_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                faker.Date.RecentOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting AND locale
        new("month", (faker, formatting, locale) => faker.Date.Month()),
        new("weekday", (faker, formatting, locale) => faker.Date.Weekday()),
        new("timezone_string", (faker, formatting, locale) => faker.Date.TimeZoneString()),
        new("account", (faker, formatting, locale) => faker.Finance.Account()),
        new("account_name", (faker, formatting, locale) => faker.Finance.AccountName()), new("amount", "0.00",
            (faker, formatting, locale) => faker.Finance.Amount().ToString()), // TODO formatting AND locale
        new("currency_name", (faker, formatting, locale) => faker.Finance.Currency().Description),
        new("currency_code", (faker, formatting, locale) => faker.Finance.Currency().Code),
        new("currency_symbol", (faker, formatting, locale) => faker.Finance.Currency().Symbol),
        new("credit_card_number", (faker, formatting, locale) => faker.Finance.CreditCardNumber()),
        new("credit_card_cvv", (faker, formatting, locale) => faker.Finance.CreditCardCvv()),
        new("routing_number", (faker, formatting, locale) => faker.Finance.RoutingNumber()),
        new("bic", (faker, formatting, locale) => faker.Finance.Bic()),
        new("iban", (faker, formatting, locale) => faker.Finance.Iban()),
        new("bitcoin_address", (faker, formatting, locale) => faker.Finance.BitcoinAddress()),
        new("ethereum_address", (faker, formatting, locale) => faker.Finance.EthereumAddress()),
        new("litecoin_address", (faker, formatting, locale) => faker.Finance.LitecoinAddress()),
        new("file_name", (faker, formatting, locale) => faker.System.FileName()),
        new("directory_path", (faker, formatting, locale) => faker.System.DirectoryPath()),
        new("file_path", (faker, formatting, locale) => faker.System.FilePath()),
        new("common_file_name", (faker, formatting, locale) => faker.System.CommonFileName()),
        new("mime_type", (faker, formatting, locale) => faker.System.MimeType()),
        new("common_file_type", (faker, formatting, locale) => faker.System.CommonFileType()),
        new("common_file_ext", (faker, formatting, locale) => faker.System.CommonFileExt()),
        new("file_type", (faker, formatting, locale) => faker.System.FileType()),
        new("file_ext", (faker, formatting, locale) => faker.System.FileExt()),
        new("semver", (faker, formatting, locale) => faker.System.Semver()),
        new("android_id", (faker, formatting, locale) => faker.System.AndroidId()),
        new("apple_push_token", (faker, formatting, locale) => faker.System.ApplePushToken()),
        new("department", (faker, formatting, locale) => faker.Commerce.Department()),
        new("price", (faker, formatting, locale) => faker.Commerce.Price()),
        new("product_name", (faker, formatting, locale) => faker.Commerce.ProductName()),
        new("product", (faker, formatting, locale) => faker.Commerce.Product()),
        new("product_adjective", (faker, formatting, locale) => faker.Commerce.ProductAdjective()),
        new("product_description", (faker, formatting, locale) => faker.Commerce.ProductDescription()),
        new("ean8", (faker, formatting, locale) => faker.Commerce.Ean8()),
        new("ean13", (faker, formatting, locale) => faker.Commerce.Ean13())
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
            if (!string.IsNullOrWhiteSpace(locale))
            {
                _logger.LogDebug($"Locale '{locale}' not found. Choose from: {string.Join(',', _locales)}.");
            }

            locale = "en";
        }

        // TODO cache faker per locale for later use.
        var faker = new Faker(locale);
        return generatorModel.FormatterFunction(faker,
            string.IsNullOrWhiteSpace(format) ? generatorModel.Formatting : format, locale);
    }

    private static int ConvertToInt(string input, int defaultValue) =>
        !int.TryParse(input, out var result) ? defaultValue : result;
}
