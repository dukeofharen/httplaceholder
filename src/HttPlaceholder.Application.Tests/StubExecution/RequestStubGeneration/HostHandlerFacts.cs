using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class HostHandlerFacts
    {
        private readonly HostHandler _handler = new HostHandler();

        [TestMethod]
        public async Task HostHandler_HandleStubGenerationAsync_Port80_NoPortInHost()
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
            Assert.IsTrue(result);
            Assert.AreEqual("httplaceholder.com", stub.Conditions.Host);
        }

        [TestMethod]
        public async Task HostHandler_HandleStubGenerationAsync_Port443_NoPortInHost()
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
            Assert.AreEqual("httplaceholder.com", stub.Conditions.Host);
        }

        [TestMethod]
        public async Task HostHandler_HandleStubGenerationAsync_Port5000_PortInHost()
        {
            // Arrange
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Url = "https://httplaceholder.com:5000"}
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("httplaceholder.com:5000", stub.Conditions.Host);
        }
    }
}
