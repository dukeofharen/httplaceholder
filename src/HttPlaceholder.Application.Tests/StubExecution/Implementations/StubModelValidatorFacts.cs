using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.Infrastructure.Implementations;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class StubModelValidatorFacts
    {
        private readonly SettingsModel _settings = new() {Stub = new StubSettingsModel()};

        private StubModelValidator _validator;

        [TestInitialize]
        public void Initialize() =>
            _validator = new StubModelValidator(new ModelValidator(), Options.Create(_settings));

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

        [DataTestMethod]
        [DataRow(null, true)]
        [DataRow(0, true)]
        [DataRow(99, false)]
        [DataRow(100, true)]
        [DataRow(200, true)]
        [DataRow(599, true)]
        [DataRow(600, false)]
        [DataRow(601, false)]
        public void ValidateStubModel_ValidateStatusCodes(int? statusCode, bool shouldSucceed)
        {
            // Arrange
            var model = new StubModel {Id = "stub-1", Response = new StubResponseModel {StatusCode = statusCode}};

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.AreEqual(
                !shouldSucceed,
                result.Any(r => r == "Field 'StatusCode' should be between '100' and '599'."),
                $"Actual error messages: {string.Join(", ", result)}");
        }

        [DataTestMethod]
        [DataRow(10, 10, true)]
        [DataRow(10, 9, true)]
        [DataRow(10, 11, false)]
        [DataRow(10, null, true)]
        public void ValidateStubModel_ValidateExtraDurationMillis(int configuredMillis, int? stubMillis,
            bool shouldSucceed)
        {
            // Arrange
            _settings.Stub.MaximumExtraDurationMillis = configuredMillis;
            var model = new StubModel {Id = "stub-1", Response = new StubResponseModel {ExtraDuration = stubMillis}};

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.AreEqual(
                !shouldSucceed,
                result.Any(r => r == $"Value for 'ExtraDuration' cannot be higher than '{configuredMillis}'."),
                $"Actual error messages: {string.Join(", ", result)}");
        }

        [DataTestMethod]
        [DataRow(LineEndingType.Unix, true)]
        [DataRow(LineEndingType.Windows, true)]
        [DataRow(null, true)]
        [DataRow(LineEndingType.NotSet, false)]
        [DataRow((LineEndingType)5, false)]
        public void ValidateStubModel_ValidateLineEndings(LineEndingType? lineEndingType, bool shouldSucceed)
        {
            // Arrange
            var model = new StubModel {Id = "stub-1", Response = new StubResponseModel {LineEndings = lineEndingType}};

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.AreEqual(
                !shouldSucceed,
                result.Any(r => r == "Value for 'LineEndings' should be any of the following values: Unix, Windows."),
                $"Actual error messages: {string.Join(", ", result)}");
        }

        [DataTestMethod]
        [DataRow("#000000", true)]
        [DataRow("#0000FF", true)]
        [DataRow("#FFFFFF", true)]
        [DataRow("#0000ff", true)]
        [DataRow("#999999", true)]
        [DataRow("#0000fg", false)]
        [DataRow("#0000fff", false)]
        public void ValidateStubModel_ValidateImageBackgroundColor(string colorHexCode, bool shouldSucceed)
        {
            // Arrange
            var model = new StubModel
            {
                Id = "stub-1",
                Response = new StubResponseModel
                {
                    Image = new StubResponseImageModel {BackgroundColor = colorHexCode}
                }
            };

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.AreEqual(
                !shouldSucceed,
                result.Any(r =>
                    r == "Field 'BackgroundColor' should be filled with a valid hex color code (e.g. '#1234AF')."),
                $"Actual error messages: {string.Join(", ", result)}");
        }

        [DataTestMethod]
        [DataRow("#000000", true)]
        [DataRow("#0000FF", true)]
        [DataRow("#FFFFFF", true)]
        [DataRow("#0000ff", true)]
        [DataRow("#999999", true)]
        [DataRow("#0000fg", false)]
        [DataRow("#0000fff", false)]
        public void ValidateStubModel_ValidateImageFontColor(string colorHexCode, bool shouldSucceed)
        {
            // Arrange
            var model = new StubModel
            {
                Id = "stub-1",
                Response = new StubResponseModel {Image = new StubResponseImageModel {FontColor = colorHexCode}}
            };

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.AreEqual(
                !shouldSucceed,
                result.Any(r =>
                    r == "Field 'FontColor' should be filled with a valid hex color code (e.g. '#1234AF')."),
                $"Actual error messages: {string.Join(", ", result)}");
        }

        [DataTestMethod]
        [DataRow(1, true)]
        [DataRow(100, true)]
        [DataRow(0, false)]
        [DataRow(-1, false)]
        [DataRow(101, false)]
        public void ValidateStubModel_ValidateJpegQuality(int jpegQuality, bool shouldSucceed)
        {
            // Arrange
            var model = new StubModel
            {
                Id = "stub-1",
                Response = new StubResponseModel {Image = new StubResponseImageModel {JpegQuality = jpegQuality}}
            };

            // Act
            var result = _validator.ValidateStubModel(model);

            // Assert
            Assert.AreEqual(
                !shouldSucceed,
                result.Any(r => r == "Field 'JpegQuality' should be between '1' and '100'."),
                $"Actual error messages: {string.Join(", ", result)}");
        }
    }
}
