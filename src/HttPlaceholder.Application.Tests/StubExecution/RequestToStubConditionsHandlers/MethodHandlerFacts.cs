using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class MethodHandlerFacts
{
    private readonly MethodHandler _handler = new();

    [TestMethod]
    public async Task MethodHandler_HandleStubGenerationAsync_MethodNotSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new HttpRequestModel {Method = string.Empty};
        var conditions = new StubConditionsModel();

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None));
    }

    [TestMethod]
    public async Task MethodHandler_HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        const string method = "GET";
        var request = new HttpRequestModel {Method = method};
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(method, conditions.Method);
    }
}
