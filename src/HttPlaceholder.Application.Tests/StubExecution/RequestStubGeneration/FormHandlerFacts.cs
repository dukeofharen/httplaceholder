using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class FormHandlerFacts
    {
        private readonly FormHandler _handler = new FormHandler();

        [TestMethod]
        public async Task FormHandler_HandleStubGenerationAsync_NoContentTypeSet_ShouldReturnFalse()
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
        public async Task FormHandler_HandleStubGenerationAsync_NoFormContentTypeSet_ShouldReturnFalse()
        {
            // Arrange
            const string contentType = "application/json";
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", contentType }
                    }
                }
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(stub.Conditions.Headers.Any());
        }

        [DataTestMethod]
        [DataRow("application/x-www-form-urlencoded")]
        [DataRow("application/x-www-form-urlencoded; charset=UTF-8")]
        public async Task FormHandler_HandleStubGenerationAsync_HappyFlow(string contentType)
        {
            // Arrange
            const string form = "form1=val1&form2=val2";
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", contentType }
                    },
                    Body = form
                }
            };
            var stub = new StubModel();

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);

            var formDict = stub.Conditions.Form.ToDictionary(f => f.Key, f => f.Value);
            Assert.AreEqual("val1", formDict["form1"]);
            Assert.AreEqual("val2", formDict["form2"]);
            Assert.IsFalse(stub.Conditions.Body.Any());
        }
    }
}
