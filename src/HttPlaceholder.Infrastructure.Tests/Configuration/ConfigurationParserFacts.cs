using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HttPlaceholder.Common;
using HttPlaceholder.Infrastructure.Configuration;

namespace HttPlaceholder.Infrastructure.Tests.Configuration;

[TestClass]
public class ConfigurationParserFacts
{
    private const string ExampleArgs =
        "--usehttps --port 8080 --httpsPort 4430 --inputFile /var/stubs --configJsonLocation /var/httpl_config.json";

    private const string ExampleConfigJson = """

                                             {
                                                 "enableUserInterface": true,
                                                 "storeResponses": false,
                                                 "oldRequestsQueueLength": 101
                                             }
                                             """;

    private const string ExampleConfigWithWeirdCasing = """

                                                        {
                                                            "APIUSERNAME": "user",
                                                            "apipassword": "pass",
                                                            "enableUserInterface": false
                                                        }
                                                        """;

    private readonly Mock<IEnvService> _envServiceMock = new();

    private readonly IDictionary<string, string> _exampleEnv = new Dictionary<string, string>
    {
        { "port", "9999" }, { "oldRequestsQueueLength", "100" }
    };

    private readonly Mock<IFileService> _fileServiceMock = new();
    private ConfigurationParser _parser;

    [TestInitialize]
    public void Initialize() =>
        _parser = new ConfigurationParser(
            _envServiceMock.Object,
            _fileServiceMock.Object);

    [TestCleanup]
    public void Cleanup()
    {
        _envServiceMock.VerifyAll();
        _fileServiceMock.VerifyAll();
    }

    [TestMethod]
    public void ParseConfiguration_HappyFlow()
    {
        // Arrange
        var args = ExampleArgs.Split(' ');
        _envServiceMock
            .Setup(m => m.GetEnvironmentVariables())
            .Returns(_exampleEnv);
        _fileServiceMock
            .Setup(m => m.FileExists("/var/httpl_config.json"))
            .Returns(true);
        _fileServiceMock
            .Setup(m => m.ReadAllText("/var/httpl_config.json"))
            .Returns(ExampleConfigJson);

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual(13, result.Count);
        Assert.AreEqual("true", result["Web:UseHttps"]);
        Assert.AreEqual("8080", result["Web:HttpPort"]);
        Assert.AreEqual("4430", result["Web:HttpsPort"]);
        Assert.AreEqual("/var/stubs", result["Storage:InputFile"]);
        Assert.AreEqual("100", result["Storage:OldRequestsQueueLength"]);
        Assert.AreEqual("true", result["Gui:EnableUserInterface"]);
        Assert.AreEqual("false", result["Storage:StoreResponses"]);
        Assert.AreEqual("False", result["Stub:AllowGlobalFileSearch"]);
    }

    [TestMethod]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Act
        var result = _parser.ParseConfiguration(Array.Empty<string>());

        // Assert
        Assert.AreEqual(12, result.Count);
        Assert.AreEqual("5000", result["Web:HttpPort"]);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result["Web:PfxPath"]));
        Assert.AreEqual("1234", result["Web:PfxPassword"]);
        Assert.AreEqual("5050", result["Web:HttpsPort"]);
        Assert.AreEqual("True", result["Web:UseHttps"]);
        Assert.AreEqual("True", result["Gui:EnableUserInterface"]);
        Assert.AreEqual("40", result["Storage:OldRequestsQueueLength"]);
        Assert.AreEqual("60000", result["Stub:MaximumExtraDurationMillis"]);
        Assert.AreEqual("True", result["Storage:CleanOldRequestsInBackgroundJob"]);
        Assert.AreEqual("False", result["Storage:StoreResponses"]);
        Assert.AreEqual("True", result["Web:ReadProxyHeaders"]);
        Assert.AreEqual("False", result["Stub:AllowGlobalFileSearch"]);
    }

    [DataTestMethod]
    [DataRow("WINDOWS", @"C:\Users\duco", @"C:\Users\duco\.httplaceholder", true)]
    [DataRow("LINUX", "/home/duco", "/home/duco/.httplaceholder", true)]
    [DataRow("OSX", "/home/duco", "/home/duco/.httplaceholder", true)]
    [DataRow(null, "", "", false)]
    public void ShouldSetDefaultFileStorageLocationSuccessfully(
        string platform,
        string userHomeFolder,
        string expectedFileStorageLocation,
        bool fileStorageLocationShouldBeSet)
    {
        // Arrange
        if (!string.IsNullOrWhiteSpace(platform))
        {
            var os = OSPlatform.Create(platform);
            _envServiceMock.Setup(m => m.IsOs(os)).Returns(true);
            _fileServiceMock.Setup(m => m.DirectoryExists(userHomeFolder)).Returns(true);
        }

        _envServiceMock.Setup(m => m.GetEnvironmentVariable("USERPROFILE")).Returns(userHomeFolder);
        _envServiceMock.Setup(m => m.GetEnvironmentVariable("HOME")).Returns(userHomeFolder);

        // Act
        var result = _parser.ParseConfiguration(Array.Empty<string>());

        // Assert
        const string key = "Storage:FileStorageLocation";
        if (fileStorageLocationShouldBeSet)
        {
            Assert.AreEqual(expectedFileStorageLocation, result[key]);
        }
        else
        {
            Assert.IsFalse(result.Any(x => x.Key == key));
        }
    }

    [TestMethod]
    public void ArgsArray_ShouldOverrideDefaultValue()
    {
        // Arrange
        var args = "--port 5001 --httpsPort 5051".Split(' ');

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("5001", result["Web:HttpPort"]);
        Assert.AreEqual("5051", result["Web:HttpsPort"]);
    }


    [TestMethod]
    public void EnvVariablesAndArgsArray_CheckCaseInsensitivity()
    {
        // Arrange
        var args = "--POrT 5001 --httpsPORT 5051".Split(' ');
        var env = new Dictionary<string, string> { { "FILEStorageLocation", "/tmp/stubs" } };

        _envServiceMock
            .Setup(m => m.GetEnvironmentVariables())
            .Returns(env);

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("5001", result["Web:HttpPort"]);
        Assert.AreEqual("5051", result["Web:HttpsPort"]);
        Assert.AreEqual("/tmp/stubs", result["Storage:FileStorageLocation"]);
    }

    [TestMethod]
    public void ReadConfigFileFromArgsArray_CheckCaseInsensitivity()
    {
        // Arrange
        const string path = "/tmp/config.json";
        var args = $"--configjsonlocation {path}".Split(' ');

        _fileServiceMock
            .Setup(m => m.FileExists(path))
            .Returns(true);

        _fileServiceMock
            .Setup(m => m.ReadAllText(path))
            .Returns(ExampleConfigWithWeirdCasing);

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("user", result["Authentication:ApiUsername"]);
        Assert.AreEqual("pass", result["Authentication:ApiPassword"]);
        Assert.AreEqual("false", result["Gui:EnableUserInterface"]);
    }
}
