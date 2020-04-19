using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class BodyHandlerFacts
    {
        private readonly BodyHandler _handler = new BodyHandler();

        [TestMethod]
        public async Task BodyHandler_HandleStubGenerationAsync_BodyNotSetOnRequest_ShouldReturnFalse()
        {
            // Arrange
            var request = new RequestResultModel {RequestParameters = new RequestParametersModel {Body = string.Empty}};
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(stub.Conditions.Body);
        }

        [TestMethod]
        public async Task BodyHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            var body = "POSTED!";
            var request = new RequestResultModel {RequestParameters = new RequestParametersModel {Body = body}};
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(body, stub.Conditions.Body.Single());
        }
    }
}
