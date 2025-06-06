﻿using System.Linq;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Common;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.Infrastructure.Implementations;
using HttPlaceholder.TestUtilities.Options;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class StubModelValidatorFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly IModelValidator _modelValidator = new ModelValidator();
    private readonly SettingsModel _settings = new() { Stub = new StubSettingsModel() };

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
        _mocker.Use(_modelValidator);
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void ValidateStubModel_IdNotSet_ShouldReturnError()
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel { Id = null };

        // Act
        var result = validator.ValidateStubModel(model);

        // Assert
        Assert.IsTrue(result.Any(r => r == "The Id field is required."));
    }

    [TestMethod]
    public void ValidateStubModel_ResponseNotSet_ShouldReturnError()
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel { Id = "stub-1", Response = null };

        // Act
        var result = validator.ValidateStubModel(model);

        // Assert
        Assert.IsTrue(result.Any(r => r == "The Response field is required."));
    }

    [DataTestMethod]
    [DataRow(null, true)]
    [DataRow(0, true)]
    [DataRow(99, false)]
    [DataRow(100, true)]
    [DataRow(200, true)]
    [DataRow(599, true)]
    [DataRow(600, false)]
    [DataRow(601, false)]
    public void ValidateStubModel_ValidateStatusCodes(int? statusCode, bool shouldSucceed)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel { Id = "stub-1", Response = new StubResponseModel { StatusCode = statusCode } };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        Assert.AreEqual(
            !shouldSucceed,
            result.Any(r => r == "Field 'StatusCode' should be between '100' and '599'."),
            $"Actual error messages: {string.Join(", ", result)}");
    }

    [DataTestMethod]
    [DataRow(10, 10, true)]
    [DataRow(10, (long)10, true)]
    [DataRow(10, 9, true)]
    [DataRow(10, "9", true)]
    [DataRow(10, 11, false)]
    [DataRow(10, "11", false)]
    [DataRow(10, null, true)]
    public void ValidateStubModel_ValidateExtraDurationMillis(int configuredMillis, object stubMillis,
        bool shouldSucceed)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        _settings.Stub.MaximumExtraDurationMillis = configuredMillis;
        var model = new StubModel { Id = "stub-1", Response = new StubResponseModel { ExtraDuration = stubMillis } };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        Assert.AreEqual(
            !shouldSucceed,
            result.Any(r => r == $"Value for 'ExtraDuration' cannot be higher than '{configuredMillis}'."),
            $"Actual error messages: {string.Join(", ", result)}");
    }

    [DataTestMethod]
    [DataRow(10, 9, 10, null)]
    [DataRow(10, 10, 11, "Value for 'ExtraDuration.Max' cannot be higher than '10'.")]
    [DataRow(10, 11, 12, "Value for 'ExtraDuration.Min' cannot be higher than '10'.")]
    [DataRow(10, 9, 8, "ExtraDuration.Min should be lower than or equal to ExtraDuration.Max.")]
    public void ValidateStubModel_ValidateExtraDurationMillis_Model(int configuredMillis, int? min, int? max,
        string expectedError)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        _settings.Stub.MaximumExtraDurationMillis = configuredMillis;
        var model = new StubModel
        {
            Id = "stub-1",
            Response = new StubResponseModel { ExtraDuration = new StubExtraDurationModel { Min = min, Max = max } }
        };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        Assert.AreEqual(
            expectedError != null,
            result.Any(r => r == expectedError),
            $"Actual error messages: {string.Join(", ", result)}");
    }

    [DataTestMethod]
    [DataRow(LineEndingType.Unix, true)]
    [DataRow(LineEndingType.Windows, true)]
    [DataRow(null, true)]
    [DataRow(LineEndingType.NotSet, false)]
    [DataRow((LineEndingType)5, false)]
    public void ValidateStubModel_ValidateLineEndings(LineEndingType? lineEndingType, bool shouldSucceed)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel { Id = "stub-1", Response = new StubResponseModel { LineEndings = lineEndingType } };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        Assert.AreEqual(
            !shouldSucceed,
            result.Any(r => r == "Value for 'LineEndings' should be any of the following values: Unix, Windows."),
            $"Actual error messages: {string.Join(", ", result)}");
    }

    [DataTestMethod]
    [DataRow("#000000", true)]
    [DataRow("#0000FF", true)]
    [DataRow("#FFFFFF", true)]
    [DataRow("#0000ff", true)]
    [DataRow("#999999", true)]
    [DataRow("#0000fg", false)]
    [DataRow("#0000fff", true)]
    [DataRow("#0000ffg", false)]
    public void ValidateStubModel_ValidateImageBackgroundColor(string colorHexCode, bool shouldSucceed)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel
        {
            Id = "stub-1",
            Response = new StubResponseModel
            {
                Image = new StubResponseImageModel { BackgroundColor = colorHexCode }
            }
        };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        Assert.AreEqual(
            !shouldSucceed,
            result.Any(r =>
                r == "Field 'BackgroundColor' should be filled with a valid hex color code (e.g. '#1234AF')."),
            $"Actual error messages: {string.Join(", ", result)}");
    }

    [DataTestMethod]
    [DataRow("#000000", true)]
    [DataRow("#0000FF", true)]
    [DataRow("#FFFFFF", true)]
    [DataRow("#0000ff", true)]
    [DataRow("#999999", true)]
    [DataRow("#0000fg", false)]
    [DataRow("#0000fff", true)]
    [DataRow("#0000ffg", false)]
    public void ValidateStubModel_ValidateImageFontColor(string colorHexCode, bool shouldSucceed)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel
        {
            Id = "stub-1",
            Response = new StubResponseModel { Image = new StubResponseImageModel { FontColor = colorHexCode } }
        };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        Assert.AreEqual(
            !shouldSucceed,
            result.Any(r =>
                r == "Field 'FontColor' should be filled with a valid hex color code (e.g. '#1234AF')."),
            $"Actual error messages: {string.Join(", ", result)}");
    }

    [DataTestMethod]
    [DataRow(1, true)]
    [DataRow(100, true)]
    [DataRow(0, false)]
    [DataRow(-1, false)]
    [DataRow(101, false)]
    public void ValidateStubModel_ValidateJpegQuality(int jpegQuality, bool shouldSucceed)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel
        {
            Id = "stub-1",
            Response = new StubResponseModel { Image = new StubResponseImageModel { JpegQuality = jpegQuality } }
        };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        Assert.AreEqual(
            !shouldSucceed,
            result.Any(r => r == "Field 'JpegQuality' should be between '1' and '100'."),
            $"Actual error messages: {string.Join(", ", result)}");
    }

    [DataTestMethod]
    [DataRow("scenario-1", 1, 1, null, null, null, null, "minHits and maxHits can not be equal.")]
    [DataRow("scenario-1", 1, 0, null, null, null, null, "maxHits can not be lower than minHits.")]
    [DataRow("scenario-1", 1, null, 1, null, null, null,
        "exactHits can not be set if minHits and maxHits are set.")]
    [DataRow("scenario-1", null, 1, 1, null, null, null,
        "exactHits can not be set if minHits and maxHits are set.")]
    [DataRow("scenario-1", null, null, null, null, "new-state", true,
        "setScenarioState and clearState can not both be set at the same time.")]
    [DataRow("scenario-1", null, null, null, null, "new-state", false, null)]
    [DataRow(null, 1, null, null, null, null, null,
        "Scenario condition checkers and response writers can not be set if no 'scenario' is provided.")]
    [DataRow(null, null, 1, null, null, null, null,
        "Scenario condition checkers and response writers can not be set if no 'scenario' is provided.")]
    [DataRow(null, null, null, 1, null, null, null,
        "Scenario condition checkers and response writers can not be set if no 'scenario' is provided.")]
    [DataRow(null, null, null, null, "new-state", null, null,
        "Scenario condition checkers and response writers can not be set if no 'scenario' is provided.")]
    [DataRow(null, null, null, null, null, "new-state", null,
        "Scenario condition checkers and response writers can not be set if no 'scenario' is provided.")]
    [DataRow(null, null, null, null, null, null, null, null)]
    [DataRow("scenario-1", 1, 2, null, null, null, null, null)]
    [DataRow("scenario-1", null, null, 3, null, null, null, null)]
    [DataRow("scenario-1", null, null, null, null, "new-state", null, null)]
    [DataRow("scenario-1", null, null, null, "expected-state", null, true, null)]
    public void ValidateStubModel_ValidateScenario(
        string scenario,
        int? minHits,
        int? maxHits,
        int? exactHits,
        string scenarioState,
        string setScenarioState,
        bool? clearState,
        string expectedError)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel
        {
            Id = "stub-id",
            Scenario = scenario,
            Conditions = new StubConditionsModel
            {
                Scenario = new StubConditionScenarioModel
                {
                    MinHits = minHits,
                    MaxHits = maxHits,
                    ExactHits = exactHits,
                    ScenarioState = scenarioState
                }
            },
            Response = new StubResponseModel
            {
                Scenario = new StubResponseScenarioModel
                {
                    ClearState = clearState, SetScenarioState = setScenarioState
                }
            }
        };

        // Arrange
        var result = validator.ValidateStubModel(model).ToArray();
        if (expectedError == null)
        {
            Assert.IsFalse(result.Length != 0,
                $"No validation errors expected, but got at least one: {string.Join(Environment.NewLine, result)}");
        }
        else
        {
            Assert.AreEqual(result.First(), expectedError);
        }
    }

    [DataTestMethod]
    [DataRow(null, null, null, null, null, null, false)]
    [DataRow("text", null, null, null, null, null, false)]
    [DataRow(null, "xml", null, null, null, null, false)]
    [DataRow(null, null, "json", null, null, null, false)]
    [DataRow(null, null, null, "html", null, null, false)]
    [DataRow(null, null, null, null, "base64", null, false)]
    [DataRow(null, null, null, null, null, "file", false)]
    [DataRow("text", "xml", null, null, null, null, true)]
    [DataRow(null, "xml", "json", null, null, null, true)]
    [DataRow(null, null, "json", "html", null, null, true)]
    [DataRow(null, null, null, "html", "base64", null, true)]
    [DataRow(null, null, null, null, "base64", "file", true)]
    [DataRow("text", "xml", "json", "html", "base64", "file", true)]
    public void ValidateStubModel_ResponseValidation(string text, string xml, string json, string html, string base64,
        string file, bool shouldReturnError)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel
        {
            Id = "stub",
            Response = new StubResponseModel
            {
                Text = text,
                Xml = xml,
                Json = json,
                Html = html,
                Base64 = base64,
                File = file
            }
        };

        // Act
        var result = validator.ValidateStubModel(model);

        // Assert
        const string errorToCheck =
            "Only one of the response body fields (text, json, xml, html, base64, file) can be set.";
        if (shouldReturnError)
        {
            Assert.IsTrue(result.Any(r => r == errorToCheck));
        }
        else
        {
            Assert.IsFalse(result.Any(r => r == errorToCheck));
        }
    }

    [DataTestMethod]
    [DataRow(204, true)]
    [DataRow(200, false)]
    public void ValidateStubModel_ResponseValidation_ResponseSetAndStatusIs204(int statusCode, bool shouldReturnError)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel
        {
            Id = "stub", Response = new StubResponseModel { StatusCode = statusCode, Text = "Some response" }
        };

        // Act
        var result = validator.ValidateStubModel(model);

        // Assert
        const string errorToCheck =
            "When HTTP status code is 204, no response body can be set.";
        if (shouldReturnError)
        {
            Assert.IsTrue(result.Any(r => r == errorToCheck));
        }
        else
        {
            Assert.IsFalse(result.Any(r => r == errorToCheck));
        }
    }

    [DataTestMethod]
    [DataRow("X-HttPlaceholder-Correlation,X-HttPlaceholder-ExecutedStub", true)]
    [DataRow("X-HttPlaceholder-Correlation", true)]
    [DataRow("X-HttPlaceholder-ExecutedStub", true)]
    [DataRow("x-httplaceholder-correlation,x-httplaceholder-executedstub", true)]
    [DataRow("x-httplaceholder-correlation", true)]
    [DataRow("x-httplaceholder-executedstub", true)]
    [DataRow("X-HttPlaceholder-Correlation,Content-Type", true)]
    [DataRow("Etag,Content-Type", false)]
    public void ValidateStubModel_IllegalResponseHeaders(string headers, bool shouldReturnError)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var model = new StubModel
        {
            Id = "stub",
            Response = new StubResponseModel
            {
                Text = "Some response", Headers = headers.Split(',').ToDictionary(h => h, _ => "headerValue")
            }
        };

        // Act
        var result = validator.ValidateStubModel(model);

        // Assert
        const string errorToCheck = "can't be used as response header.";
        if (shouldReturnError)
        {
            Assert.IsTrue(result.Any(r => r.Contains(errorToCheck)));
        }
        else
        {
            Assert.IsFalse(result.Any(r => r.Contains(errorToCheck)));
        }
    }

    [DataTestMethod]
    [DataRow("text", "regex", "jsonPath", null, "replace", "Replace [0]: set either 'text', 'regex' or 'jsonReplace'.")]
    [DataRow(null, null, null, null, "replace",
        "Replace [0]: either 'text', 'regex' or 'jsonReplace' needs to be set.")]
    [DataRow(null, "regex", null, true, "replace",
        "Replace [0]: 'ignoreCase' can only be used with 'text'.")]
    [DataRow(null, null, "jsonPath", true, "replace",
        "Replace [0]: 'ignoreCase' can only be used with 'text'.")]
    [DataRow(null, "regex", null, null, null, "Replace [0]: 'replaceWith' should be set.")]
    [DataRow(null, "regex", null, null, "replace", null)]
    [DataRow("text", null, null, true, "replace", null)]
    [DataRow(null, null, "jsonPath", null, "replace", null)]
    public void ValidateStringRegexReplace(string text, string regex, string jsonPath, bool? ignoreCase,
        string replaceWith,
        string expectedError)
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var dto = new StubResponseReplaceModel
        {
            Text = text,
            Regex = regex,
            IgnoreCase = ignoreCase,
            ReplaceWith = replaceWith,
            JsonPath = jsonPath
        };
        var model = new StubModel { Id = "stub", Response = new StubResponseModel { Replace = new[] { dto } } };

        // Act
        var result = validator.ValidateStubModel(model).ToArray();

        // Assert
        if (expectedError == null)
        {
            Assert.IsFalse(result.Any(r => r.Contains("Replace")));
        }
        else
        {
            Assert.AreEqual(1, result.Count(r => r.Equals(expectedError)),
                $"Expected error: {expectedError}. Actual errors: {string.Join(',', result)}");
        }
    }

    [TestMethod]
    public void ValidateReverseProxy_HostNotAllowed_ShouldContainError()
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var hostnameValidatorMock = _mocker.GetMock<IHostnameValidator>();
        var model = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    Url = "https://httplaceholder.org/destination"
                }
            }
        };

        hostnameValidatorMock
            .Setup(m => m.HostnameIsValid("httplaceholder.org"))
            .Returns(false);

        // Act
        var result = validator.ValidateStubModel(model);

        // Assert
        Assert.IsTrue(result.Any(r => r == "Hostname 'httplaceholder.org' is invalid for use with the reverse proxy response writer. Consult the documentation on how to enable the reverse proxy or how to enable the \"dev mode\"."));
    }

    [TestMethod]
    public void ValidateReverseProxy_HostAllowed_ShouldNotContainError()
    {
        // Arrange
        var validator = _mocker.CreateInstance<StubModelValidator>();
        var hostnameValidatorMock = _mocker.GetMock<IHostnameValidator>();
        var model = new StubModel
        {
            Response = new StubResponseModel
            {
                ReverseProxy = new StubResponseReverseProxyModel
                {
                    Url = "https://httplaceholder.org/destination"
                }
            }
        };

        hostnameValidatorMock
            .Setup(m => m.HostnameIsValid("httplaceholder.org"))
            .Returns(true);

        // Act
        var result = validator.ValidateStubModel(model);

        // Assert
        Assert.IsFalse(result.Any(r => r.Contains("is invalid for use with the reverse proxy response writer")));
    }
}
