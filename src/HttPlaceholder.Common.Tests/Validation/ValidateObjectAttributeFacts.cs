using System.ComponentModel.DataAnnotations;
using System.Linq;
using HttPlaceholder.Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Common.Tests.Validation;

[TestClass]
public class ValidateObjectAttributeFacts
{
    private readonly TestValidateObjectAttribute _attribute = new();

    [TestMethod]
    public void IsValid_InputIsNull_ShouldReturnSuccess()
    {
        // Act
        var result = _attribute.TestIsValid(null);

        // Assert
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_InputIsObjectAndNotValid_ShouldReturnValidationErrors()
    {
        // Arrange
        var input = new TestModel {Name = null};

        // Act
        var result = (CompositeValidationResult)_attribute.TestIsValid(input);

        // Assert
        Assert.AreEqual("Validation for Object failed!", result.ErrorMessage);

        var results = result.Results.ToArray();
        Assert.AreEqual(1, results.Length);
        Assert.AreEqual("The Name field is required.", results.Single().ErrorMessage);
    }

    [TestMethod]
    public void IsValid_InputIsObjectAndValid_ShouldReturnSuccess()
    {
        // Arrange
        var input = new TestModel {Name = "john"};

        // Act
        var result = (CompositeValidationResult)_attribute.TestIsValid(input);

        // Assert
        Assert.AreEqual(ValidationResult.Success, result);
    }

    [TestMethod]
    public void IsValid_InputIsArrayAndNotValid_ShouldReturnValidationErrors()
    {
        // Arrange
        var input = new[] {new TestModel {Name = null}, new TestModel {Name = null}};

        // Act
        var result = (CompositeValidationResult)_attribute.TestIsValid(input);

        // Assert
        Assert.AreEqual("Validation for Object failed!", result.ErrorMessage);

        var results = result.Results.ToArray();
        Assert.AreEqual(2, results.Length);
        Assert.IsTrue(results.All(r => r.ErrorMessage == "The Name field is required."));
    }

    [TestMethod]
    public void IsValid_InputIsArrayAndValid_ShouldReturnSuccess()
    {
        // Arrange
        var input = new[] {new TestModel {Name = "john"}, new TestModel {Name = "john"}};

        // Act
        var result = (CompositeValidationResult)_attribute.TestIsValid(input);

        // Assert
        Assert.AreEqual(ValidationResult.Success, result);
    }

    private class TestValidateObjectAttribute : ValidateObjectAttribute
    {
        public ValidationResult TestIsValid(object value) =>
            IsValid(value, new ValidationContext(new object()));
    }

    private class TestModel
    {
        [Required] public string Name { get; set; }
    }
}
