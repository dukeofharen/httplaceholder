using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class PathHandlerFacts
    {
        private readonly PathHandler _handler = new PathHandler();

        [TestMethod]
        public async Task PathHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            var url = "https://httplaceholder.com/A/Path?query1=val1&query2=val2";
            var request = new RequestResultModel {RequestParameters = new RequestParametersModel {Url = url}};
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("/A/Path", stub.Conditions.Url.Path);
        }
    }
}
