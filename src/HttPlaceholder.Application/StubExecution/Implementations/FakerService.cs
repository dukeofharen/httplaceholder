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
    private static readonly string[] _locales =
    {
        "af_ZA", "fr_CH", "ar", "ge", "az", "hr", "cz", "id_ID", "de", "it", "de_AT", "ja", "de_CH", "ko", "el",
        "lv", "en", "nb_NO", "en_AU", "ne", "en_AU_ocker", "nl", "en_BORK", "nl_BE", "en_CA", "pl", "en_GB",
        "pt_BR", "en_IE", "pt_PT", "en_IND", "ro", "en_NG", "ru", "en_US", "sk", "en_ZA", "sv", "es", "tr", "es_MX",
        "uk", "fa", "vi", "fi", "zh_CN", "fr", "zh_TW", "fr_CA", "zu_ZA"
    };

    private static readonly FakeDataGeneratorModel[] _generators =
    {
        new("zipcode", (faker, formatting) => faker.Address.ZipCode()),
        new("city", (faker, formatting) => faker.Address.City()),
        new("street_address", (faker, formatting) => faker.Address.StreetAddress()),
        new("city_prefix", (faker, formatting) => faker.Address.CityPrefix()),
        new("city_suffix", (faker, formatting) => faker.Address.CitySuffix()),
        new("street_name", (faker, formatting) => faker.Address.StreetName()),
        new("building_number", (faker, formatting) => faker.Address.BuildingNumber()),
        new("street_suffix", (faker, formatting) => faker.Address.StreetSuffix()),
        new("secondary_address", (faker, formatting) => faker.Address.SecondaryAddress()),
        new("county", (faker, formatting) => faker.Address.County()),
        new("country", (faker, formatting) => faker.Address.Country()),
        new("full_address", (faker, formatting) => faker.Address.FullAddress()),
        new("country_code", (faker, formatting) => faker.Address.CountryCode()),
        new("state", (faker, formatting) => faker.Address.State()),
        new("state_abbreviation", (faker, formatting) => faker.Address.StateAbbr()),
        new("direction", (faker, formatting) => faker.Address.Direction()),
        new("cardinal_direction", (faker, formatting) => faker.Address.CardinalDirection()),
        new("ordinal_direction", (faker, formatting) => faker.Address.OrdinalDirection()),
        new("first_name", (faker, formatting) => faker.Name.FirstName()),
        new("last_name", (faker, formatting) => faker.Name.LastName()),
        new("full_name", (faker, formatting) => faker.Name.FullName()),
        new("prefix", (faker, formatting) => faker.Name.Prefix()),
        new("suffix", (faker, formatting) => faker.Name.Suffix()),
        new("job_title", (faker, formatting) => faker.Name.JobTitle()),
        new("job_descriptor", (faker, formatting) => faker.Name.JobDescriptor()),
        new("job_area", (faker, formatting) => faker.Name.JobArea()),
        new("job_type", (faker, formatting) => faker.Name.JobType()),
        new("phone_number", (faker, formatting) => faker.Phone.PhoneNumber()),
        new("email", (faker, formatting) => faker.Internet.Email()),
        new("example_email", (faker, formatting) => faker.Internet.ExampleEmail()),
        new("user_name", (faker, formatting) => faker.Internet.UserName()),
        new("user_name_unicode", (faker, formatting) => faker.Internet.UserNameUnicode()),
        new("domain_name", (faker, formatting) => faker.Internet.DomainName()),
        new("domain_word", (faker, formatting) => faker.Internet.DomainWord()),
        new("domain_suffix", (faker, formatting) => faker.Internet.DomainSuffix()),
        new("ip", (faker, formatting) => faker.Internet.Ip()),
        new("port", (faker, formatting) => faker.Internet.Port().ToString()),
        new("ipv6", (faker, formatting) => faker.Internet.Ipv6()),
        new("user_agent", (faker, formatting) => faker.Internet.UserAgent()),
        new("mac", (faker, formatting) => faker.Internet.Mac()),
        new("password", (faker, formatting) => faker.Internet.Password()),
        new("color", (faker, formatting) => faker.Internet.Color()),
        new("protocol", (faker, formatting) => faker.Internet.Protocol()),
        new("url", (faker, formatting) => faker.Internet.Url()),
        new("url_with_path", (faker, formatting) => faker.Internet.UrlWithPath()),
        new("url_rooted_path", (faker, formatting) => faker.Internet.UrlRootedPath()),
        new("lorem", (faker, formatting) => faker.Lorem.Word()),
        new("words", (faker, formatting) => string.Join(' ', faker.Lorem.Words())), // TODO formatting
        new("letter", (faker, formatting) => faker.Lorem.Letter()), // TODO formatting
        new("sentence", (faker, formatting) => faker.Lorem.Sentence()), // TODO formatting
        new("sentences", (faker, formatting) => faker.Lorem.Sentences()), // TODO formatting
        new("paragraph", (faker, formatting) => faker.Lorem.Paragraph()), // TODO formatting
        new("paragraphs", (faker, formatting) => faker.Lorem.Paragraphs()), // TODO formatting
        new("text", (faker, formatting) => faker.Lorem.Text()),
        new("lines", (faker, formatting) => faker.Lorem.Lines()), // TODO formatting
        new("slug", (faker, formatting) => faker.Lorem.Slug()), // TODO formatting
        new("past", (faker, formatting) => faker.Date.Past().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("past_offset",
            (faker, formatting) => faker.Date.PastOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("soon", (faker, formatting) => faker.Date.Soon().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("soon_offset",
            (faker, formatting) => faker.Date.SoonOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("future",
            (faker, formatting) => faker.Date.Future().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("future_offset",
            (faker, formatting) => faker.Date.FutureOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("recent",
            (faker, formatting) => faker.Date.Recent().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("recent_offset",
            (faker, formatting) => faker.Date.RecentOffset().ToString(CultureInfo.InvariantCulture)), // TODO formatting
        new("month", (faker, formatting) => faker.Date.Month()),
        new("weekday", (faker, formatting) => faker.Date.Weekday()),
        new("timezone_string", (faker, formatting) => faker.Date.TimeZoneString()),
        new("account", (faker, formatting) => faker.Finance.Account()),
        new("account_name", (faker, formatting) => faker.Finance.AccountName()),
        new("amount", (faker, formatting) => faker.Finance.Amount().ToString()), // TODO formatting
        new("currency_name", (faker, formatting) => faker.Finance.Currency().Description),
        new("currency_code", (faker, formatting) => faker.Finance.Currency().Code),
        new("currency_symbol", (faker, formatting) => faker.Finance.Currency().Symbol),
        new("credit_card_number", (faker, formatting) => faker.Finance.CreditCardNumber()),
        new("credit_card_cvv", (faker, formatting) => faker.Finance.CreditCardCvv()),
        new("routing_number", (faker, formatting) => faker.Finance.RoutingNumber()),
        new("bic", (faker, formatting) => faker.Finance.Bic()),
        new("iban", (faker, formatting) => faker.Finance.Iban()),
        new("bitcoin_address", (faker, formatting) => faker.Finance.BitcoinAddress()),
        new("ethereum_address", (faker, formatting) => faker.Finance.EthereumAddress()),
        new("litecoin_address", (faker, formatting) => faker.Finance.LitecoinAddress()),
        new("file_name", (faker, formatting) => faker.System.FileName()),
        new("directory_path", (faker, formatting) => faker.System.DirectoryPath()),
        new("file_path", (faker, formatting) => faker.System.FilePath()),
        new("common_file_name", (faker, formatting) => faker.System.CommonFileName()),
        new("mime_type", (faker, formatting) => faker.System.MimeType()),
        new("common_file_type", (faker, formatting) => faker.System.CommonFileType()),
        new("common_file_ext", (faker, formatting) => faker.System.CommonFileExt()),
        new("file_type", (faker, formatting) => faker.System.FileType()),
        new("file_ext", (faker, formatting) => faker.System.FileExt()),
        new("semver", (faker, formatting) => faker.System.Semver()),
        new("android_id", (faker, formatting) => faker.System.AndroidId()),
        new("apple_push_token", (faker, formatting) => faker.System.ApplePushToken()),
        new("department", (faker, formatting) => faker.Commerce.Department()),
        new("price", (faker, formatting) => faker.Commerce.Price()),
        new("product_name", (faker, formatting) => faker.Commerce.ProductName()),
        new("product", (faker, formatting) => faker.Commerce.Product()),
        new("product_adjective", (faker, formatting) => faker.Commerce.ProductAdjective()),
        new("product_description", (faker, formatting) => faker.Commerce.ProductDescription()),
        new("ean8", (faker, formatting) => faker.Commerce.Ean8()),
        new("ean13", (faker, formatting) => faker.Commerce.Ean13())
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
