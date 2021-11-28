using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class HeaderHandlerFacts
    {
        private readonly HeaderHandler _handler = new HeaderHandler();

        [TestMethod]
        public async Task HeaderHandler_HandleStubGenerationAsync_NoHeadersSet_ShouldReturnFalse()
        {
            // Arrange
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Headers = new Dictionary<string, string>()}
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(stub.Conditions.Headers.Any());
        }

        [TestMethod]
        public async Task HeaderHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "Postman-Token", Guid.NewGuid().ToString() },
                        { "Host", "httplaceholder.com" },
                        { "X-Api-Key", "123"},
                        { "X-Bla", "bla" }
                    }
                }
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, stub.Conditions.Headers.Count);
            Assert.AreEqual("123", stub.Conditions.Headers["X-Api-Key"]);
            Assert.AreEqual("bla", stub.Conditions.Headers["X-Bla"]);
        }
    }
}
