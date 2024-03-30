using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using Bogus;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class FakerService(ILogger<FakerService> logger) : IFakerService, ISingletonService
{
    private const string DefaultDatetimeFormat = "yyyy-MM-dd HH:mm:ss";

    private static readonly string[] _locales =
    [
        "af_ZA", "fr_CH", "ar", "ge", "az", "hr", "cz", "id_ID", "de", "it", "de_AT", "ja", "de_CH", "ko", "el",
        "lv", "en", "nb_NO", "en_AU", "ne", "en_AU_ocker", "nl", "en_BORK", "nl_BE", "en_CA", "pl", "en_GB",
        "pt_BR", "en_IE", "pt_PT", "en_IND", "ro", "en_NG", "ru", "en_US", "sk", "en_ZA", "sv", "es", "tr", "es_MX",
        "uk", "fa", "vi", "fi", "zh_CN", "fr", "zh_TW", "fr_CA", "zu_ZA"
    ];

    private static readonly FakeDataGeneratorModel[] _generators =
    [
        new FakeDataGeneratorModel("zipcode", (faker, _, _) => faker.Address.ZipCode()),
        new FakeDataGeneratorModel("city", (faker, _, _) => faker.Address.City()),
        new FakeDataGeneratorModel("street_address", (faker, _, _) => faker.Address.StreetAddress()),
        new FakeDataGeneratorModel("city_prefix", (faker, _, _) => faker.Address.CityPrefix()),
        new FakeDataGeneratorModel("city_suffix", (faker, _, _) => faker.Address.CitySuffix()),
        new FakeDataGeneratorModel("street_name", (faker, _, _) => faker.Address.StreetName()),
        new FakeDataGeneratorModel("building_number", (faker, _, _) => faker.Address.BuildingNumber()),
        new FakeDataGeneratorModel("street_suffix", (faker, _, _) => faker.Address.StreetSuffix()),
        new FakeDataGeneratorModel("secondary_address", (faker, _, _) => faker.Address.SecondaryAddress()),
        new FakeDataGeneratorModel("county", (faker, _, _) => faker.Address.County()),
        new FakeDataGeneratorModel("country", (faker, _, _) => faker.Address.Country()),
        new FakeDataGeneratorModel("full_address", (faker, _, _) => faker.Address.FullAddress()),
        new FakeDataGeneratorModel("country_code", (faker, _, _) => faker.Address.CountryCode()),
        new FakeDataGeneratorModel("state", (faker, _, _) => faker.Address.State()),
        new FakeDataGeneratorModel("state_abbreviation", (faker, _, _) => faker.Address.StateAbbr()),
        new FakeDataGeneratorModel("direction", (faker, _, _) => faker.Address.Direction()),
        new FakeDataGeneratorModel("cardinal_direction", (faker, _, _) => faker.Address.CardinalDirection()),
        new FakeDataGeneratorModel("ordinal_direction", (faker, _, _) => faker.Address.OrdinalDirection()),
        new FakeDataGeneratorModel("first_name", (faker, _, _) => faker.Name.FirstName()),
        new FakeDataGeneratorModel("last_name", (faker, _, _) => faker.Name.LastName()),
        new FakeDataGeneratorModel("full_name", (faker, _, _) => faker.Name.FullName()),
        new FakeDataGeneratorModel("prefix", (faker, _, _) => faker.Name.Prefix()),
        new FakeDataGeneratorModel("suffix", (faker, _, _) => faker.Name.Suffix()),
        new FakeDataGeneratorModel("job_title", (faker, _, _) => faker.Name.JobTitle()),
        new FakeDataGeneratorModel("job_descriptor", (faker, _, _) => faker.Name.JobDescriptor()),
        new FakeDataGeneratorModel("job_area", (faker, _, _) => faker.Name.JobArea()),
        new FakeDataGeneratorModel("job_type", (faker, _, _) => faker.Name.JobType()),
        new FakeDataGeneratorModel("phone_number", (faker, _, _) => faker.Phone.PhoneNumber()),
        new FakeDataGeneratorModel("email", (faker, _, _) => faker.Internet.Email()),
        new FakeDataGeneratorModel("example_email", (faker, _, _) => faker.Internet.ExampleEmail()),
        new FakeDataGeneratorModel("user_name", (faker, _, _) => faker.Internet.UserName()),
        new FakeDataGeneratorModel("user_name_unicode", (faker, _, _) => faker.Internet.UserNameUnicode()),
        new FakeDataGeneratorModel("domain_name", (faker, _, _) => faker.Internet.DomainName()),
        new FakeDataGeneratorModel("domain_word", (faker, _, _) => faker.Internet.DomainWord()),
        new FakeDataGeneratorModel("domain_suffix", (faker, _, _) => faker.Internet.DomainSuffix()),
        new FakeDataGeneratorModel("ip", (faker, _, _) => faker.Internet.Ip()),
        new FakeDataGeneratorModel("port", (faker, _, _) => faker.Internet.Port().ToString()),
        new FakeDataGeneratorModel("ipv6", (faker, _, _) => faker.Internet.Ipv6()),
        new FakeDataGeneratorModel("user_agent", (faker, _, _) => faker.Internet.UserAgent()),
        new FakeDataGeneratorModel("mac", (faker, _, _) => faker.Internet.Mac()),
        new FakeDataGeneratorModel("password", (faker, _, _) => faker.Internet.Password()),
        new FakeDataGeneratorModel("color", (faker, _, _) => faker.Internet.Color()),
        new FakeDataGeneratorModel("protocol", (faker, _, _) => faker.Internet.Protocol()),
        new FakeDataGeneratorModel("url", (faker, _, _) => faker.Internet.Url()),
        new FakeDataGeneratorModel("url_with_path", (faker, _, _) => faker.Internet.UrlWithPath()),
        new FakeDataGeneratorModel("url_rooted_path", (faker, _, _) => faker.Internet.UrlRootedPath()),
        new FakeDataGeneratorModel("word", (faker, _, _) => faker.Lorem.Word()), new FakeDataGeneratorModel("words",
            "3",
            (faker, formatting, _) => string.Join(' ', faker.Lorem.Words(ConvertToInt(formatting, 3)))),
        new FakeDataGeneratorModel("letter", "1",
            (faker, formatting, _) => faker.Lorem.Letter(ConvertToInt(formatting, 1))),
        new FakeDataGeneratorModel("sentence", "5",
            (faker, formatting, _) => faker.Lorem.Sentence(ConvertToInt(formatting, 5))),
        new FakeDataGeneratorModel("sentences", "3",
            (faker, formatting, _) => faker.Lorem.Sentences(ConvertToInt(formatting, 3))),
        new FakeDataGeneratorModel("paragraph", "3",
            (faker, formatting, _) => faker.Lorem.Paragraph(ConvertToInt(formatting, 3))),
        new FakeDataGeneratorModel("paragraphs", "3",
            (faker, formatting, _) => faker.Lorem.Paragraphs(ConvertToInt(formatting, 3))),
        new FakeDataGeneratorModel("text", (faker, _, _) => faker.Lorem.Text()),
        new FakeDataGeneratorModel("lines", "3",
            (faker, formatting, _) => faker.Lorem.Lines(ConvertToInt(formatting, 3))),
        new FakeDataGeneratorModel("slug", "3",
            (faker, formatting, _) => faker.Lorem.Slug(ConvertToInt(formatting, 3))),
        new FakeDataGeneratorModel("past",
            DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Past(), formatting, locale)),
        new FakeDataGeneratorModel("past_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.PastOffset(), formatting, locale)),
        new FakeDataGeneratorModel("soon", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Soon(), formatting, locale)),
        new FakeDataGeneratorModel("soon_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.SoonOffset(), formatting, locale)),
        new FakeDataGeneratorModel("future", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Future(), formatting, locale)),
        new FakeDataGeneratorModel("future_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.FutureOffset(), formatting, locale)),
        new FakeDataGeneratorModel("recent", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTime(faker.Date.Recent(), formatting, locale)),
        new FakeDataGeneratorModel("recent_offset", DefaultDatetimeFormat,
            (faker, formatting, locale) =>
                ConvertDateTimeOffset(faker.Date.RecentOffset(), formatting, locale)),
        new FakeDataGeneratorModel("month", (faker, _, _) => faker.Date.Month()),
        new FakeDataGeneratorModel("weekday", (faker, _, _) => faker.Date.Weekday()),
        new FakeDataGeneratorModel("timezone_string", (faker, _, _) => faker.Date.TimeZoneString()),
        new FakeDataGeneratorModel("account", (faker, _, _) => faker.Finance.Account()),
        new FakeDataGeneratorModel("account_name", (faker, _, _) => faker.Finance.AccountName()),
        new FakeDataGeneratorModel("amount", "0.00",
            (faker, formatting, locale) => ConvertDecimal(faker.Finance.Amount(), formatting, locale)),
        new FakeDataGeneratorModel("currency_name", (faker, _, _) => faker.Finance.Currency().Description),
        new FakeDataGeneratorModel("currency_code", (faker, _, _) => faker.Finance.Currency().Code),
        new FakeDataGeneratorModel("credit_card_number", (faker, _, _) => faker.Finance.CreditCardNumber()),
        new FakeDataGeneratorModel("credit_card_cvv", (faker, _, _) => faker.Finance.CreditCardCvv()),
        new FakeDataGeneratorModel("routing_number", (faker, _, _) => faker.Finance.RoutingNumber()),
        new FakeDataGeneratorModel("bic", (faker, _, _) => faker.Finance.Bic()),
        new FakeDataGeneratorModel("iban", (faker, _, _) => faker.Finance.Iban()),
        new FakeDataGeneratorModel("bitcoin_address", (faker, _, _) => faker.Finance.BitcoinAddress()),
        new FakeDataGeneratorModel("ethereum_address", (faker, _, _) => faker.Finance.EthereumAddress()),
        new FakeDataGeneratorModel("litecoin_address", (faker, _, _) => faker.Finance.LitecoinAddress()),
        new FakeDataGeneratorModel("file_name", (faker, _, _) => faker.System.FileName()),
        new FakeDataGeneratorModel("directory_path", (faker, _, _) => faker.System.DirectoryPath()),
        new FakeDataGeneratorModel("file_path", (faker, _, _) => faker.System.FilePath()),
        new FakeDataGeneratorModel("common_file_name", (faker, _, _) => faker.System.CommonFileName()),
        new FakeDataGeneratorModel("mime_type", (faker, _, _) => faker.System.MimeType()),
        new FakeDataGeneratorModel("common_file_type", (faker, _, _) => faker.System.CommonFileType()),
        new FakeDataGeneratorModel("common_file_ext", (faker, _, _) => faker.System.CommonFileExt()),
        new FakeDataGeneratorModel("file_type", (faker, _, _) => faker.System.FileType()),
        new FakeDataGeneratorModel("file_ext", (faker, _, _) => faker.System.FileExt()),
        new FakeDataGeneratorModel("semver", (faker, _, _) => faker.System.Semver()),
        new FakeDataGeneratorModel("android_id", (faker, _, _) => faker.System.AndroidId()),
        new FakeDataGeneratorModel("apple_push_token", (faker, _, _) => faker.System.ApplePushToken()),
        new FakeDataGeneratorModel("department", (faker, _, _) => faker.Commerce.Department()),
        new FakeDataGeneratorModel("price", (faker, _, _) => faker.Commerce.Price()),
        new FakeDataGeneratorModel("product_name", (faker, _, _) => faker.Commerce.ProductName()),
        new FakeDataGeneratorModel("product", (faker, _, _) => faker.Commerce.Product()),
        new FakeDataGeneratorModel("product_adjective", (faker, _, _) => faker.Commerce.ProductAdjective()),
        new FakeDataGeneratorModel("product_description", (faker, _, _) => faker.Commerce.ProductDescription()),
        new FakeDataGeneratorModel("ean8", (faker, _, _) => faker.Commerce.Ean8()),
        new FakeDataGeneratorModel("ean13", (faker, _, _) => faker.Commerce.Ean13())
    ];

    private readonly ConcurrentDictionary<string, Faker> _cachedFakers = new();

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
            logger.LogWarning("No fake data generator found with name '{Generator}'.", generator);
            return string.Empty;
        }

        if (!_locales.Any(l => string.Equals(l, locale, StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrWhiteSpace(locale))
            {
                logger.LogDebug("Locale '{Locale}' not found. Choose from: {Locales}.",
                    locale,
                    string.Join(',', _locales));
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
