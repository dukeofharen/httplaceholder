using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.TestUtilities.Options;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class HostnameValidatorFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly SettingsModel _settings = new() { Stub = new StubSettingsModel() };

    [TestInitialize]
    public void Initialize() =>
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void HostnameIsValid_ReverseProxyIsDisabled_ShouldReturnFalse()
    {
        // Arrange
        var validator = _mocker.CreateInstance<HostnameValidator>();
        _settings.Stub.EnableReverseProxy = false;
        _settings.Stub.AllowedHosts = "";
        _settings.Stub.DisallowedHosts = "";

        // Act
        var result = validator.HostnameIsValid("httplaceholder.org");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HostnameIsValid_ReverseProxyIsEnabled_NoHostsSet_ShouldReturnTrue()
    {
        // Arrange
        var validator = _mocker.CreateInstance<HostnameValidator>();
        _settings.Stub.EnableReverseProxy = true;
        _settings.Stub.AllowedHosts = "";
        _settings.Stub.DisallowedHosts = "";

        // Act
        var result = validator.HostnameIsValid("httplaceholder.org");

        // Assert
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("httplaceholder.org", "httplaceholder.org", "", true)]
    [DataRow("httplaceholder.org", "", "httplaceholder.org", false)]
    [DataRow("httplaceholder.org", "", "httplaceholder.com", true)]
    [DataRow("httplaceholder.org", "httplaceholder.org", "httplaceholder.org", true)]
    [DataRow("httplaceholder.org", "httplaceholder.com", "httplaceholder.org", false)]
    [DataRow("httplaceholder.org", "httplaceholder.com", "httplaceholder.org", false)]
    [DataRow("httplaceholder.org", "reddit.com,^httpl(.*)\\.org$", "", true)]
    [DataRow("httplaceholder.org", "reddit.com,^https(.*)\\.org$", "", false)]
    [DataRow("127.0.0.1", "127.0.0.1", "", true)]
    [DataRow("127.0.0.1", "", "127.0.0.1", false)]
    [DataRow("127.0.0.2", "", "127.0.0.1", true)]
    [DataRow("127.0.0.1", "127.0.0.1", "127.0.0.1", true)]
    [DataRow("127.0.0.1", "127.0.0.1,127.0.0.2", "", true)]
    [DataRow("127.0.0.9", "127.0.0.0/29", "", false)]
    [DataRow("127.0.0.6", "127.0.0.0/26", "", true)]
    [DataRow("127.0.0.1", "127.0.0.1,127.0.0.2,127.0.0.0/26", "", true)]
    [DataRow("127.0.0.9", "", "127.0.0.0/29", true)]
    [DataRow("127.0.0.6", "", "127.0.0.0/26", false)]
    public void HostnameIsValid_HappyFlow(
        string hostname,
        string allowedHosts,
        string disallowedHosts,
        bool expectedResult)
    {
        // Arrange
        var validator = _mocker.CreateInstance<HostnameValidator>();
        _settings.Stub.EnableReverseProxy = true;
        _settings.Stub.AllowedHosts = allowedHosts;
        _settings.Stub.DisallowedHosts = disallowedHosts;

        // Act
        var result = validator.HostnameIsValid(hostname);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
