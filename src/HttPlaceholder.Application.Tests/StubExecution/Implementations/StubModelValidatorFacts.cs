using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Infrastructure.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class StubModelValidatorFacts
    {
        private readonly StubModelValidator _validator = new StubModelValidator(new ModelValidator());

        [TestMethod]
        public void ValidateStubModel_IdNotSet_ShouldReturnError()
        {
            // Arrange
            var model = new StubModel {Id = null};

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.IsTrue(result.Any(r => r == "The Id field is required."));
        }

        [TestMethod]
        public void ValidateStubModel_ResponseNotSet_ShouldReturnError()
        {
            // Arrange
            var model = new StubModel {Id = "stub-1", Response = null};

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.IsTrue(result.Any(r => r == "The Response field is required."));
        }

        [TestMethod]
        public void ValidateStubModel_ValidateStatusCodes()
        {
            // Not using DataTestMethod because it barely works in Rider.
            (int statusCode, bool shouldSucceed)[] testData =
            {
                (99, false), (100, true), (200, true), (599, true), (600, false), (601, false)
            };
            foreach (var value in testData)
            {
                // Arrange
                var model = new StubModel
                {
                    Id = "stub-1", Response = new StubResponseModel {StatusCode = value.statusCode}
                };

                // Act
                var result = _validator.ValidateStubModel(model);

                // Assert
                if (value.shouldSucceed)
                {
                    Assert.IsFalse(
                        result.Any(r => r == "Field 'StatusCode' should be between '100' and '599'."),
                        $"Assertion failed for status code {value.statusCode}. Error messages: {string.Join(", ", result)}");
                }
                else
                {
                    Assert.IsTrue(
                        result.Any(r => r == "Field 'StatusCode' should be between '100' and '599'."),
                        $"Assertion failed for status code {value.statusCode}. Error messages: {string.Join(", ", result)}");
                }
            }
        }
    }
}
