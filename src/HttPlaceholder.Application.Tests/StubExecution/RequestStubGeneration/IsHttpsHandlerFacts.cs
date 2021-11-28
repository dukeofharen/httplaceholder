using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class IsHttpsHandlerFacts
    {
        private readonly IsHttpsHandler _handler = new();

        [TestMethod]
        public async Task IsHttpsHandler_HandleStubGenerationAsync_NoHttps_ShouldNotSetIsHttps()
        {
            // Arrange
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Url = "http://httplaceholder.com"}
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(stub.Conditions.Url.IsHttps.HasValue);
        }

        [TestMethod]
        public async Task IsHttpsHandler_HandleStubGenerationAsync_Https_ShouldSetToTrue()
        {
            // Arrange
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Url = "https://httplaceholder.com"}
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(stub.Conditions.Url.IsHttps.HasValue && stub.Conditions.Url.IsHttps.Value);
        }
    }
}
