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
        public void Validate_IdNotSet_ShouldReturnError()
        {
            // Arrange
            var model = new StubModel {Id = null};

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.IsTrue(result.Any(r => r == "The Id field is required."));
        }

        [TestMethod]
        public void Validate_ResponseNotSet_ShouldReturnError()
        {
            // Arrange
            var model = new StubModel {Id = "stub-1", Response = null};

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.IsTrue(result.Any(r => r == "The Response field is required."));
        }
    }
}
