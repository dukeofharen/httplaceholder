using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class FakerServiceFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void GenerateFakeData_GeneratorNotFound_ShouldReturnEmptyString()
    {
        // Arrange
        var service = _mocker.CreateInstance<FakerService>();

        // Act
        var result = service.GenerateFakeData("bla", string.Empty, string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [DataTestMethod]
    [DataRow("zipcode", "")]
    [DataRow("city", "")]
    [DataRow("street_address", "")]
    [DataRow("city_prefix", "")]
    [DataRow("city_suffix", "")]
    [DataRow("street_name", "")]
    [DataRow("building_number", "")]
    [DataRow("street_suffix", "")]
    [DataRow("secondary_address", "")]
    [DataRow("county", "")]
    [DataRow("country", "")]
    [DataRow("full_address", "")]
    [DataRow("country_code", "")]
    [DataRow("state", "")]
    [DataRow("state_abbreviation", "")]
    [DataRow("direction", "")]
    [DataRow("cardinal_direction", "")]
    [DataRow("ordinal_direction", "")]
    [DataRow("first_name", "")]
    [DataRow("last_name", "")]
    [DataRow("full_name", "")]
    [DataRow("prefix", "")]
    [DataRow("suffix", "")]
    [DataRow("job_title", "")]
    [DataRow("job_descriptor", "")]
    [DataRow("job_area", "")]
    [DataRow("job_type", "")]
    [DataRow("phone_number", "")]
    [DataRow("email", "")]
    [DataRow("example_email", "")]
    [DataRow("user_name", "")]
    [DataRow("user_name_unicode", "")]
    [DataRow("domain_name", "")]
    [DataRow("domain_word", "")]
    [DataRow("domain_suffix", "")]
    [DataRow("ip", "")]
    [DataRow("port", "")]
    [DataRow("ipv6", "")]
    [DataRow("user_agent", "")]
    [DataRow("mac", "")]
    [DataRow("password", "")]
    [DataRow("color", "")]
    [DataRow("protocol", "")]
    [DataRow("url", "")]
    [DataRow("url_with_path", "")]
    [DataRow("url_rooted_path", "")]
    [DataRow("word", "")]
    [DataRow("words", "")]
    [DataRow("words", "10")]
    [DataRow("letter", "")]
    [DataRow("letter", "5")]
    [DataRow("sentence", "")]
    [DataRow("sentence", "10")]
    [DataRow("sentences", "")]
    [DataRow("sentences", "10")]
    [DataRow("paragraph", "")]
    [DataRow("paragraph", "5")]
    [DataRow("paragraphs", "")]
    [DataRow("paragraphs", "5")]
    [DataRow("text", "")]
    [DataRow("lines", "")]
    [DataRow("lines", "5")]
    [DataRow("slug", "")]
    [DataRow("slug", "5")]
    [DataRow("past", "")]
    [DataRow("past", "yyyy-MM-dd")]
    [DataRow("past_offset", "")]
    [DataRow("past_offset", "yyyy-MM-dd")]
    [DataRow("soon", "")]
    [DataRow("soon", "yyyy-MM-dd")]
    [DataRow("soon_offset", "")]
    [DataRow("soon_offset", "yyyy-MM-dd")]
    [DataRow("future", "")]
    [DataRow("future", "yyyy-MM-dd")]
    [DataRow("future_offset", "")]
    [DataRow("future_offset", "yyyy-MM-dd")]
    [DataRow("recent", "")]
    [DataRow("recent", "yyyy-MM-dd")]
    [DataRow("recent_offset", "")]
    [DataRow("recent_offset", "yyyy-MM-dd")]
    [DataRow("month", "")]
    [DataRow("weekday", "")]
    [DataRow("timezone_string", "")]
    [DataRow("account", "")]
    [DataRow("account_name", "")]
    [DataRow("amount", "0.000")]
    [DataRow("currency_name", "")]
    [DataRow("currency_code", "")]
    [DataRow("credit_card_number", "")]
    [DataRow("credit_card_cvv", "")]
    [DataRow("routing_number", "")]
    [DataRow("bic", "")]
    [DataRow("iban", "")]
    [DataRow("bitcoin_address", "")]
    [DataRow("ethereum_address", "")]
    [DataRow("litecoin_address", "")]
    [DataRow("file_name", "")]
    [DataRow("directory_path", "")]
    [DataRow("file_path", "")]
    [DataRow("common_file_name", "")]
    [DataRow("mime_type", "")]
    [DataRow("common_file_type", "")]
    [DataRow("common_file_ext", "")]
    [DataRow("file_type", "")]
    [DataRow("file_ext", "")]
    [DataRow("semver", "")]
    [DataRow("android_id", "")]
    [DataRow("apple_push_token", "")]
    [DataRow("department", "")]
    [DataRow("price", "")]
    [DataRow("product_name", "")]
    [DataRow("product", "")]
    [DataRow("product_adjective", "")]
    [DataRow("product_description", "")]
    [DataRow("ean8", "")]
    [DataRow("ean13", "")]
    public void GenerateFakeData_CheckFakeDataGeneration(string generator, string formatting)
    {
        // Arrange
        var service = _mocker.CreateInstance<FakerService>();

        // Act
        var result = service.GenerateFakeData(generator, string.Empty, formatting);

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GenerateFakeData_Locales()
    {
        // Arrange
        var service = _mocker.CreateInstance<FakerService>();

        // Act
        var locales = service.GetLocales();

        // Assert
        Assert.AreEqual(50, locales.Length);

        // Act
        foreach (var locale in locales)
        {
            var result = service.GenerateFakeData("first_name", locale, string.Empty);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        }
    }

    [TestMethod]
    public void GetGenerators_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<FakerService>();

        // Act
        var generators = service.GetGenerators();

        // Assert
        Assert.AreEqual(100, generators.Length);
    }

    [TestMethod]
    public void GetGenerators_AllGeneratorNamesAreUnique()
    {
        // Arrange
        var service = _mocker.CreateInstance<FakerService>();

        // Act
        var generators = service.GetGenerators();

        // Assert
        var groups = generators.GroupBy(g => g.Name);
        Assert.IsTrue(groups.All(g => g.Count() == 1));
    }
}
