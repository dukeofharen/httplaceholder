using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HttPlaceholder.Common;
using HttPlaceholder.Infrastructure.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Infrastructure.Tests.Configuration;

[TestClass]
public class ConfigurationParserFacts
{
    private const string ExampleConfig = @"
{
    ""apiUsername"": ""user"",
    ""apiPassword"": ""pass"",
    ""enableUserInterface"": false
}";

    private const string ExampleConfigWithWeirdCasing = @"
{
    ""APIUSERNAME"": ""user"",
    ""apipassword"": ""pass"",
    ""enableUserInterface"": false
}";

    private readonly Mock<IEnvService> _envServiceMock = new Mock<IEnvService>();
    private readonly Mock<IFileService> _fileServiceMock = new Mock<IFileService>();
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
    public void ArgsArray_ShouldParseCorrectly()
    {
        // Arrange
        var args = ToArgs("--apiUsername user --apiPassword pass");

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("user", result["Authentication:ApiUsername"]);
        Assert.AreEqual("pass", result["Authentication:ApiPassword"]);
    }

    [TestMethod]
    public void ArgsArray_BoolArgsWithoutValue_ShouldInterpretAsTrue()
    {
        // Arrange
        var args = ToArgs("--useHttps --enableRequestLogging --enableUserInterface --port 5001");

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("5001", result["Web:HttpPort"]);
        Assert.AreEqual("True", result["Web:UseHttps"]);
        Assert.AreEqual("True", result["Storage:EnableRequestLogging"]);
        Assert.AreEqual("True", result["Gui:EnableUserInterface"]);
    }

    [TestMethod]
    public void ArgsArray_BoolArgsWithValue_ShouldTakeThatValue()
    {
        // Arrange
        var args = ToArgs("--useHttps false --port 5001");

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("5001", result["Web:HttpPort"]);
        Assert.AreEqual("false", result["Web:UseHttps"]);
    }

    [TestMethod]
    public void ReadConfigFileFromArgsArray_FileNotFound_ShouldThrowFileNotFoundException()
    {
        // Arrange
        const string path = "/tmp/config.json";
        var args = ToArgs($"--configjsonlocation {path}");

        _fileServiceMock
            .Setup(m => m.FileExists(path))
            .Returns(false);

        // Act / Assert
        Assert.ThrowsException<FileNotFoundException>(() => _parser.ParseConfiguration(args));
    }

    [TestMethod]
    public void ReadConfigFileFromArgsArray_FileFound_ShouldParseCorrectly()
    {
        // Arrange
        const string path = "/tmp/config.json";
        var args = ToArgs($"--configjsonlocation {path}");

        _fileServiceMock
            .Setup(m => m.FileExists(path))
            .Returns(true);

        _fileServiceMock
            .Setup(m => m.ReadAllText(path))
            .Returns(ExampleConfig);

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("user", result["Authentication:ApiUsername"]);
        Assert.AreEqual("pass", result["Authentication:ApiPassword"]);
        Assert.AreEqual("false", result["Gui:EnableUserInterface"]);
    }

    [TestMethod]
    public void ReadConfigFileFromArgsArray_FileFound_CaseInsensitiveCheck()
    {
        // Arrange
        const string path = "/tmp/config.json";
        var args = ToArgs($"--configJSONlocAtion {path}");

        _fileServiceMock
            .Setup(m => m.FileExists(path))
            .Returns(true);

        _fileServiceMock
            .Setup(m => m.ReadAllText(path))
            .Returns(ExampleConfig);

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("user", result["Authentication:ApiUsername"]);
        Assert.AreEqual("pass", result["Authentication:ApiPassword"]);
        Assert.AreEqual("false", result["Gui:EnableUserInterface"]);
    }

    [TestMethod]
    public void ReadConfigFileFromEnvVar_FileFound_ShouldParseCorrectly()
    {
        // Arrange
        const string path = "/tmp/config.json";
        var args = new string[0];
        var envVars = new Dictionary<string, string> {{"configjsonlocation", path}};

        _envServiceMock
            .Setup(m => m.GetEnvironmentVariables())
            .Returns(envVars);

        _fileServiceMock
            .Setup(m => m.FileExists(path))
            .Returns(true);

        _fileServiceMock
            .Setup(m => m.ReadAllText(path))
            .Returns(ExampleConfig);

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("user", result["Authentication:ApiUsername"]);
        Assert.AreEqual("pass", result["Authentication:ApiPassword"]);
        Assert.AreEqual("false", result["Gui:EnableUserInterface"]);
    }

    [TestMethod]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Act
        var result = _parser.ParseConfiguration(Array.Empty<string>());

        // Assert
        Assert.AreEqual("5000", result["Web:HttpPort"]);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result["Web:PfxPath"]));
        Assert.AreEqual("1234", result["Web:PfxPassword"]);
        Assert.AreEqual("5050", result["Web:HttpsPort"]);
        Assert.AreEqual("True", result["Web:UseHttps"]);
        Assert.AreEqual("True", result["Gui:EnableUserInterface"]);
        Assert.AreEqual("40", result["Storage:OldRequestsQueueLength"]);
        Assert.AreEqual("60000", result["Stub:MaximumExtraDurationMillis"]);
    }

    [DataTestMethod]
    [DataRow("WINDOWS", "C:\\Users\\duco", "C:\\Users\\duco\\.httplaceholder", false, true, true)]
    [DataRow("WINDOWS", "C:\\Users\\duco", "C:\\Users\\duco\\.httplaceholder", true, false, true)]
    [DataRow("LINUX", "/home/duco", "/home/duco/.httplaceholder", false, true, true)]
    [DataRow("LINUX", "/home/duco", "/home/duco/.httplaceholder", true, false, true)]
    [DataRow("OSX", "/home/duco", "/home/duco/.httplaceholder", false, true, true)]
    [DataRow("OSX", "/home/duco", "/home/duco/.httplaceholder", false, false, true)]
    [DataRow(null, "", "", false, false, false)]
    public void ShouldSetDefaultFileStorageLocationSuccessfully(
        string platform,
        string userHomeFolder,
        string expectedFileStorageLocation,
        bool folderAlreadyExists,
        bool shouldCreateFolder,
        bool fileStorageLocationShouldBeSet)
    {
        // Arrange
        if (!string.IsNullOrWhiteSpace(platform))
        {
            var os = OSPlatform.Create(platform);
            _envServiceMock.Setup(m => m.IsOs(os)).Returns(true);
            _fileServiceMock.Setup(m => m.DirectoryExists(userHomeFolder)).Returns(true);
            _fileServiceMock.Setup(m => m.DirectoryExists(expectedFileStorageLocation)).Returns(folderAlreadyExists);
        }

        _envServiceMock.Setup(m => m.GetEnvironmentVariable("USERPROFILE")).Returns(userHomeFolder);
        _envServiceMock.Setup(m => m.GetEnvironmentVariable("HOME")).Returns(userHomeFolder);

        // Act
        var result = _parser.ParseConfiguration(new string[0]);

        // Assert
        const string key = "Storage:FileStorageLocation";
        if (fileStorageLocationShouldBeSet)
        {
            Assert.AreEqual(expectedFileStorageLocation, result[key]);
            _fileServiceMock.Verify(
                m => m.CreateDirectory(expectedFileStorageLocation),
                Times.Exactly(folderAlreadyExists ? 0 : 1));
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
        var args = ToArgs("--port 5001 --httpsPort 5051");

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("5001", result["Web:HttpPort"]);
        Assert.AreEqual("5051", result["Web:HttpsPort"]);
    }

    [TestMethod]
    public void EnvVariables_ShouldOverrideArgsArray()
    {
        // Arrange
        var args = ToArgs("--port 5001 --httpsPort 5051");
        var env = new Dictionary<string, string> {{"port", "5002"}, {"httpsPort", "5052"}};

        _envServiceMock
            .Setup(m => m.GetEnvironmentVariables())
            .Returns(env);

        // Act
        var result = _parser.ParseConfiguration(args);

        // Assert
        Assert.AreEqual("5002", result["Web:HttpPort"]);
        Assert.AreEqual("5052", result["Web:HttpsPort"]);
    }

    [TestMethod]
    public void EnvVariablesAndArgsArray_CheckCaseInsensitivity()
    {
        // Arrange
        var args = ToArgs("--POrT 5001 --httpsPORT 5051");
        var env = new Dictionary<string, string> {{"FILEStorageLocation", "/tmp/stubs"}};

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
        var args = ToArgs($"--configjsonlocation {path}");

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

    private static string[] ToArgs(string input) => input.Split(' ');
}