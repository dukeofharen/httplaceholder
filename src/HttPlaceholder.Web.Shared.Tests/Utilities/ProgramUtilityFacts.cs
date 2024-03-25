using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Common;
using HttPlaceholder.Web.Shared.Utilities.Implementations;

namespace HttPlaceholder.Web.Shared.Tests.Utilities;

[TestClass]
public class ProgramUtilityFacts
{
    private readonly Mock<IIpService> _mockIpService = new();
    private readonly Mock<ITcpService> _mockTcp = new();
    private readonly SettingsModel _settings = MockSettingsFactory.GetSettings();
    private ProgramUtility _utility;

    [TestInitialize]
    public void Initialize()
    {
        _settings.Web.HttpPort = DefaultConfiguration.DefaultHttpPort.ToString();
        _settings.Web.HttpsPort = DefaultConfiguration.DefaultHttpsPort.ToString();
        _settings.Web.UseHttps = true;
        _settings.Web.PfxPath = "/etc/key.pfx";
        _settings.Web.PfxPassword = "1234";
        _utility = new ProgramUtility(_mockTcp.Object, _mockIpService.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _mockTcp.VerifyAll();
        _mockIpService.VerifyAll();
    }

    [TestMethod]
    public void GetPorts_Http_DefaultPortTaken_ShouldTakeNextFreePort()
    {
        // Arrange
        _mockTcp
            .Setup(m => m.PortIsTaken(DefaultConfiguration.DefaultHttpPort))
            .Returns(true);

        const int nextFreePort = 6000;
        _mockTcp
            .Setup(m => m.GetNextFreeTcpPort())
            .Returns(nextFreePort);

        // Act
        var result = _utility.GetPorts(_settings);

        // Assert
        var httpPorts = result.httpPorts.ToArray();
        Assert.AreEqual(1, httpPorts.Length);
        Assert.AreEqual(nextFreePort, httpPorts[0]);
    }

    [TestMethod]
    public void GetPorts_Http_NonDefaultPort_PortIsTaken_ShouldThrowException()
    {
        // Arrange
        _settings.Web.HttpPort = "7000,7001";
        _mockTcp
            .Setup(m => m.PortIsTaken(7000))
            .Returns(true);
        _mockTcp
            .Setup(m => m.PortIsTaken(7001))
            .Returns(true);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _utility.GetPorts(_settings));

        // Assert
        Assert.AreEqual("The following ports are already taken: 7000, 7001", exception.Message);
    }

    [TestMethod]
    public void GetPorts_Http_NonDefaultPort_PortsAreNotTaken_ShouldReturnPorts()
    {
        // Arrange
        _settings.Web.HttpPort = "7000,7001";
        _mockTcp
            .Setup(m => m.PortIsTaken(7000))
            .Returns(false);
        _mockTcp
            .Setup(m => m.PortIsTaken(7001))
            .Returns(false);

        // Act
        var result = _utility.GetPorts(_settings);

        // Assert
        var httpPorts = result.httpPorts.ToArray();
        Assert.AreEqual(2, httpPorts.Length);
        Assert.AreEqual(7000, httpPorts[0]);
        Assert.AreEqual(7001, httpPorts[1]);
    }

    [TestMethod]
    public void GetPorts_Https_DefaultPortTaken_ShouldTakeNextFreePort()
    {
        // Arrange
        _mockTcp
            .Setup(m => m.PortIsTaken(DefaultConfiguration.DefaultHttpsPort))
            .Returns(true);

        const int nextFreePort = 6000;
        _mockTcp
            .Setup(m => m.GetNextFreeTcpPort())
            .Returns(nextFreePort);

        // Act
        var result = _utility.GetPorts(_settings);

        // Assert
        var httpsPorts = result.httpsPorts.ToArray();
        Assert.AreEqual(1, httpsPorts.Length);
        Assert.AreEqual(nextFreePort, httpsPorts[0]);
    }

    [TestMethod]
    public void GetPorts_Https_NonDefaultPort_PortIsTaken_ShouldThrowException()
    {
        // Arrange
        _settings.Web.HttpsPort = "7000,7001";
        _mockTcp
            .Setup(m => m.PortIsTaken(7000))
            .Returns(true);
        _mockTcp
            .Setup(m => m.PortIsTaken(7001))
            .Returns(true);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _utility.GetPorts(_settings));

        // Assert
        Assert.AreEqual("The following ports are already taken: 7000, 7001", exception.Message);
    }

    [TestMethod]
    public void GetPorts_Https_NonDefaultPort_PortsAreNotTaken_ShouldReturnPorts()
    {
        // Arrange
        _settings.Web.HttpsPort = "7000,7001";
        _mockTcp
            .Setup(m => m.PortIsTaken(7000))
            .Returns(false);
        _mockTcp
            .Setup(m => m.PortIsTaken(7001))
            .Returns(false);

        // Act
        var result = _utility.GetPorts(_settings);

        // Assert
        var httpsPorts = result.httpsPorts.ToArray();
        Assert.AreEqual(2, httpsPorts.Length);
        Assert.AreEqual(7000, httpsPorts[0]);
        Assert.AreEqual(7001, httpsPorts[1]);
    }

    [TestMethod]
    public void GetPorts_Https_UseHttpsIsFalse_ShouldNotParseHttpsPorts()
    {
        // Arrange
        _settings.Web.HttpsPort = "7000,7001";
        _settings.Web.UseHttps = false;

        // Act
        var result = _utility.GetPorts(_settings);

        // Assert
        Assert.AreEqual(0, result.httpsPorts.Count());
    }

    [TestMethod]
    public void GetPorts_Https_PfxPathNotSet_ShouldNotParseHttpsPorts()
    {
        // Arrange
        _settings.Web.HttpsPort = "7000,7001";
        _settings.Web.PfxPath = string.Empty;

        // Act
        var result = _utility.GetPorts(_settings);

        // Assert
        Assert.AreEqual(0, result.httpsPorts.Count());
    }

    [TestMethod]
    public void GetPorts_Https_PfxPasswordNotSet_ShouldNotParseHttpsPorts()
    {
        // Arrange
        _settings.Web.HttpsPort = "7000,7001";
        _settings.Web.PfxPassword = string.Empty;

        // Act
        var result = _utility.GetPorts(_settings);

        // Assert
        Assert.AreEqual(0, result.httpsPorts.Count());
    }

    [DataTestMethod]
    [DataRow(false, "0", "Port '0' is invalid.")]
    [DataRow(false, "65536", "Port '65536' is invalid.")]
    [DataRow(false, "hi", "Port 'hi' is invalid.")]
    [DataRow(false, "80,hi", "Port 'hi' is invalid.")]
    [DataRow(false, "80, 65536", "Port '65536' is invalid.")]
    [DataRow(true, "0", "Port '0' is invalid.")]
    [DataRow(true, "65536", "Port '65536' is invalid.")]
    [DataRow(true, "hi", "Port 'hi' is invalid.")]
    [DataRow(true, "80,hi", "Port 'hi' is invalid.")]
    [DataRow(true, "80, 65536", "Port '65536' is invalid.")]
    public void GetPorts_PortIsInvalid(bool isHttps, string port, string expectedError)
    {
        // Arrange
        if (isHttps)
        {
            _settings.Web.HttpPort = port;
        }
        else
        {
            _settings.Web.HttpsPort = port;
        }

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => _utility.GetPorts(_settings));

        // Assert
        Assert.AreEqual(expectedError, exception.Message);
    }

    [TestMethod]
    public void GetHostnames_NoLocalIpAddressFound_ShouldReturnLocalhostOnly()
    {
        // Arrange
        _mockIpService
            .Setup(m => m.GetLocalIpAddress())
            .Returns((string)null);

        // Act
        var result = _utility.GetHostnames().ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("127.0.0.1", result[0]);
        Assert.AreEqual("localhost", result[1]);
    }

    [TestMethod]
    public void GetHostnames_LocalIpAddressFound_ShouldReturnLocalhostAndLocalIp()
    {
        // Arrange
        const string localIp = "192.168.178.32";
        _mockIpService
            .Setup(m => m.GetLocalIpAddress())
            .Returns(localIp);

        // Act
        var result = _utility.GetHostnames().ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual("127.0.0.1", result[0]);
        Assert.AreEqual("localhost", result[1]);
        Assert.AreEqual(localIp, result[2]);
    }
}
