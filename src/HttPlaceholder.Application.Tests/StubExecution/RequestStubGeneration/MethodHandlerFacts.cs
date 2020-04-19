using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class MethodHandlerFacts
    {
        private readonly MethodHandler _handler = new MethodHandler();

        [TestMethod]
        public async Task MethodHandler_HandleStubGenerationAsync_MethodNotSet_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Method = string.Empty}
            };
            var stub = new StubModel();

            // Act / Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _handler.HandleStubGenerationAsync(request, stub));
        }

        [TestMethod]
        public async Task MethodHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            var method = "GET";
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Method = method}
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(method, stub.Conditions.Method);
        }
    }
}
