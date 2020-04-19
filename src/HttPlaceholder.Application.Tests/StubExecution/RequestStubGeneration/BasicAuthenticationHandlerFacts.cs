using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestStubGeneration
{
    [TestClass]
    public class BasicAuthenticationHandlerFacts
    {
        private readonly BasicAuthenticationHandler _handler = new BasicAuthenticationHandler();

        [TestMethod]
        public async Task BasicAuthenticationHandler_HandleStubGenerationAsync_AuthorizationHeaderNotSet_ShouldReturnFalse()
        {
            // Arrange
            var stub = new StubModel();
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Headers = new Dictionary<string, string>()}
            };

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(stub.Conditions.BasicAuthentication);
        }

        [TestMethod]
        public async Task BasicAuthenticationHandler_HandleStubGenerationAsync_AuthorizationWithout2Parts_ShouldReturnFalse()
        {
            // Arrange
            var stub = new StubModel();
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass:rubble")) }
                    }
                }
            };

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(stub.Conditions.BasicAuthentication);
        }

        [TestMethod]
        public async Task BasicAuthenticationHandler_HandleStubGenerationAsync_HappyFlow()
        {
            // Arrange
            const string username = "httplaceholder";
            const string password = "secret";
            var auth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            var stub = new StubModel
            {
                Conditions = {Headers = new Dictionary<string, string> {{"Authorization", auth}}}
            };
            var request = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "Authorization", auth }
                    }
                }
            };

            // Act
            var result = await _handler.HandleStubGenerationAsync(request, stub);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(username, stub.Conditions.BasicAuthentication.Username);
            Assert.AreEqual(password, stub.Conditions.BasicAuthentication.Password);
            Assert.IsFalse(stub.Conditions.Headers.Any());
        }
    }
}
