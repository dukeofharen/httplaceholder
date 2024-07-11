using System.Collections.Generic;
using System.Text;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class StringReplaceResponseWriterFacts
{
    private readonly ResponseModel _response = new();
    private readonly StubModel _stub = new() { Response = new StubResponseModel() };
    private readonly StringReplaceResponseWriter _writer = new();

    public static IEnumerable<object[]> ProvideStringReplaceData => new[]
    {
        [new[] { GetModel("VALUE1", "VALUE2", false) }, "VALUE1 body", "VALUE2 body"],
        [
            new[] { GetModel("value1", "VALUE2", false) }, "VALUE1 body", "VALUE1 body"
        ],
        [new[] { GetModel("value1", "VALUE2", true) }, "VALUE1 body", "VALUE2 body"],
        [
            new[] { GetModel("value1", "VALUE2", null) }, "VALUE1 body", "VALUE2 body"
        ],
        new object[]
        {
            new[] { GetModel("VALUE1", "VALUE2", false), GetModel("VALUE2", "VALUE3", false) }, "VALUE1 body",
            "VALUE3 body"
        }
    };

    public static IEnumerable<object[]> ProvideRegexReplaceData => new[]
    {
        new object[]
        {
            new[] { GetRegexModel("\\!", "?"), GetRegexModel("Hello", "Bye") }, "Hello, World!", "Bye, World?"
        }
    };

    public static IEnumerable<object[]> ProvideJsonPathReplaceData => new[]
    {
        new object[]
        {
            new[] { GetJsonPathModel("$.name", "Henk") }, """{"name":"Klaas"}""", """{"name":"Henk"}"""
        },
        new object[]
        {
            new[] { GetJsonPathModel("$[1].name", "Henk") }, """[{"name":"Klaas"},{"name":"Piet"}]""",
            """[{"name":"Klaas"},{"name":"Henk"}]"""
        },
        new object[]
        {
            new[] { GetJsonPathModel("$[*].name", "Henk") }, """[{"name":"Klaas"},{"name":"Piet"}]""",
            """[{"name":"Henk"},{"name":"Henk"}]"""
        }
    };

    [TestMethod]
    public async Task WriteToResponseAsync_ReplaceIsNull_ShouldReturnNotExecuted()
    {
        // Arrange
        _stub.Response.Replace = null;

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ReplaceIsEmpty_ShouldReturnNotExecuted()
    {
        // Arrange
        _stub.Response.Replace = Array.Empty<StubResponseReplaceModel>();

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ResponseBodyIsNull_ShouldReturnNotExecuted()
    {
        // Arrange
        _stub.Response.Replace = new[] { new StubResponseReplaceModel() };
        _response.Body = null;

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ResponseBodyIsEmpty_ShouldReturnNotExecuted()
    {
        // Arrange
        _stub.Response.Replace = new[] { new StubResponseReplaceModel() };
        _response.Body = Array.Empty<byte>();

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    [DynamicData(nameof(ProvideStringReplaceData))]
    public async Task WriteToResponseAsync_StringReplace_HappyFlow(
        IEnumerable<StubResponseReplaceModel> models,
        string responseBody,
        string expectedBody)
    {
        // Arrange
        _stub.Response.Replace = models;
        _response.Body = Encoding.UTF8.GetBytes(responseBody);

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(expectedBody, Encoding.UTF8.GetString(_response.Body));
    }

    [TestMethod]
    [DynamicData(nameof(ProvideRegexReplaceData))]
    public async Task WriteToResponseAsync_RegexReplace_HappyFlow(
        IEnumerable<StubResponseReplaceModel> models,
        string responseBody,
        string expectedBody)
    {
        // Arrange
        _stub.Response.Replace = models;
        _response.Body = Encoding.UTF8.GetBytes(responseBody);

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(expectedBody, Encoding.UTF8.GetString(_response.Body));
    }

    [TestMethod]
    [DynamicData(nameof(ProvideJsonPathReplaceData))]
    public async Task WriteToResponseAsync_JsonPathReplace_HappyFlow(
        IEnumerable<StubResponseReplaceModel> models,
        string responseBody,
        string expectedBody)
    {
        // Arrange
        _stub.Response.Replace = models;
        _response.Body = Encoding.UTF8.GetBytes(responseBody);

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(expectedBody, Encoding.UTF8.GetString(_response.Body));
    }

    [TestMethod]
    public async Task WriteToResponseAsync_JsonPathReplace_CorruptJson_ShouldLogMessage()
    {
        // Arrange
        _stub.Response.Replace = [GetJsonPathModel("$.name", "replace value")];
        _response.Body = "invalid JSON"u8.ToArray();

        // Act
        var result = await WriteToResponseAsync(_stub, _response);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual("invalid JSON", Encoding.UTF8.GetString(_response.Body));
        Assert.IsTrue(result.Log.Contains("Unexpected character encountered"));
    }

    private async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response) =>
        await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

    private static StubResponseReplaceModel GetModel(string text, string replaceWith, bool? ignoreCase) =>
        new() { Text = text, ReplaceWith = replaceWith, IgnoreCase = ignoreCase };

    private static StubResponseReplaceModel GetRegexModel(string regex, string replaceWith) =>
        new() { Regex = regex, ReplaceWith = replaceWith };

    private static StubResponseReplaceModel GetJsonPathModel(string jsonPath, string replaceWith) =>
        new() { JsonPath = jsonPath, ReplaceWith = replaceWith };
}
