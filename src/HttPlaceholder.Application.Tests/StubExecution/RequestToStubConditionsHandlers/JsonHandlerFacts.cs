using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class JsonHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task HandleStubGenerationAsync_ContentTypeNotSet_ShouldReturnFalse()
    {
        // Arrange
        var handler = _mocker.CreateInstance<JsonHandler>();
        var request = new HttpRequestModel();
        var conditions = new StubConditionsModel();

        // Act
        var result = await handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.Json);
    }

    [TestMethod]
    public async Task HandleStubGenerationAsync_ContentTypeNotJson_ShouldReturnFalse()
    {
        // Arrange
        var handler = _mocker.CreateInstance<JsonHandler>();
        var request = new HttpRequestModel {Headers = {{HeaderKeys.ContentType, Constants.TextMime}}};
        var conditions = new StubConditionsModel();

        // Act
        var result = await handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.Json);
    }

    [TestMethod]
    public async Task HandleStubGenerationAsync_JsonIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var handler = _mocker.CreateInstance<JsonHandler>();
        var request = new HttpRequestModel {Headers = {{HeaderKeys.ContentType, Constants.JsonMime}}, Body = "INVALID JSON!!"};
        var conditions = new StubConditionsModel();

        // Act
        var result = await handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.Json);
    }

    [TestMethod]
    public async Task HandleStubGenerationAsync_JsonIsValid_ShouldReturnTrue()
    {
        // Arrange
        var handler = _mocker.CreateInstance<JsonHandler>();
        var request = new HttpRequestModel
        {
            Headers = {{HeaderKeys.ContentType, Constants.JsonMime}},
            Body = @"[""value1"",44,false,{""key1"":""val1""},[""1"",2]]"
        };
        var conditions = new StubConditionsModel();

        // Act
        var result = await handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsInstanceOfType(conditions.Json, typeof(JToken));
    }
}
