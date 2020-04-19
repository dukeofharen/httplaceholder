using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class QueryParamHandlerFacts
    {
        private readonly QueryParamHandler _handler = new QueryParamHandler();

        [TestMethod]
        public async Task QueryParamHandler_HandleStubGenerationAsync_NoQuerySet_ShouldReturnFalse()
        {
            // Arrange
            var url = "https://httplaceholder.com/A/Path";
            var request = new RequestResultModel {RequestParameters = new RequestParametersModel {Url = url}};
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(stub.Conditions.Url.Query.Any());
        }

        [TestMethod]
        public async Task QueryParamHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            var url = "https://httplaceholder.com/A/Path?query1=val1&query2=val2";
            var request = new RequestResultModel {RequestParameters = new RequestParametersModel {Url = url}};
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, stub.Conditions.Url.Query.Count);
            Assert.AreEqual("val1", stub.Conditions.Url.Query["query1"]);
            Assert.AreEqual("val2", stub.Conditions.Url.Query["query2"]);
        }
    }
}
