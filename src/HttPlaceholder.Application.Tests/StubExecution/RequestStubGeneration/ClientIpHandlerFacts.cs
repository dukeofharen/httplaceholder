using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class ClientIpHandlerFacts
    {
        private readonly ClientIpHandler _handler = new ClientIpHandler();

        [TestMethod]
        public async Task ClientIpHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            string ip = "11.22.33.44";
            var request = new RequestResultModel {RequestParameters = new RequestParametersModel {ClientIp = ip}};
            var stub = new StubModel();

            // Act
            bool result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(ip, stub.Conditions.ClientIp);
        }
    }
}
