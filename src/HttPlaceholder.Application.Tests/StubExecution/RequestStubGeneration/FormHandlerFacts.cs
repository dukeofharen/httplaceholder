using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class FormHandlerFacts
    {
        private readonly FormHandler _handler = new();

        [TestMethod]
        public async Task FormHandler_HandleStubGenerationAsync_NoContentTypeSet_ShouldReturnFalse()
        {
            // Arrange
            var request = new HttpRequestModel { Headers = new Dictionary<string, string>() };
            var conditions = new StubConditionsModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, conditions);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(conditions.Headers.Any());
        }

        [TestMethod]
        public async Task FormHandler_HandleStubGenerationAsync_NoFormContentTypeSet_ShouldReturnFalse()
        {
            // Arrange
            const string contentType = "application/json";
            var request = new HttpRequestModel
            {
                Headers = new Dictionary<string, string> { { "Content-Type", contentType } }
            };
            var conditions = new StubConditionsModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, conditions);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(conditions.Headers.Any());
        }

        [DataTestMethod]
        [DataRow("application/x-www-form-urlencoded")]
        [DataRow("application/x-www-form-urlencoded; charset=UTF-8")]
        public async Task FormHandler_HandleStubGenerationAsync_HappyFlow(string contentType)
        {
            // Arrange
            const string form = "form1=val1&form2=val2";
            var request = new HttpRequestModel
            {
                Headers = new Dictionary<string, string> { { "Content-Type", contentType } }, Body = form
            };
            var conditions = new StubConditionsModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, conditions);

            // Assert
            Assert.IsTrue(result);

            var formDict = conditions.Form.ToDictionary(f => f.Key, f => f.Value);
            Assert.AreEqual("val1", formDict["form1"]);
            Assert.AreEqual("val2", formDict["form2"]);
            Assert.IsFalse(conditions.Body.Any());
        }
    }
}
