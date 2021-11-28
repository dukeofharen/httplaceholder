using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class QueryParamHandlerFacts
    {
        private readonly QueryParamHandler _handler = new();

        [TestMethod]
        public async Task QueryParamHandler_HandleStubGenerationAsync_NoQuerySet_ShouldReturnFalse()
        {
            // Arrange
            const string url = "https://httplaceholder.com/A/Path";
            var request = new HttpRequestModel { Url = url };
            var conditions = new StubConditionsModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, conditions);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(conditions.Url.Query.Any());
        }

        [TestMethod]
        public async Task QueryParamHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            const string url = "https://httplaceholder.com/A/Path?query1=val1&query2=val2";
            var request = new HttpRequestModel { Url = url };
            var conditions = new StubConditionsModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, conditions);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, conditions.Url.Query.Count);
            Assert.AreEqual("val1", conditions.Url.Query["query1"]);
            Assert.AreEqual("val2", conditions.Url.Query["query2"]);
        }
    }
}
