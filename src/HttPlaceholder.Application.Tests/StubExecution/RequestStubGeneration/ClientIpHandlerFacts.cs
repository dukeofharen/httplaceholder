using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class ClientIpHandlerFacts
    {
        private readonly ClientIpHandler _handler = new();

        [TestMethod]
        public async Task ClientIpHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            const string ip = "11.22.33.44";
            var request = new HttpRequestModel { ClientIp = ip };
            var conditions = new StubConditionsModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, conditions);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(ip, conditions.ClientIp);
        }
    }
}
