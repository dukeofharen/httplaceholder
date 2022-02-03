using HttPlaceholder.Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Common.Tests.Validation;

[TestClass]
public class BetweenAttributeFacts
{
    [DataTestMethod]
    [DataRow(4, 3, 5, false, false, true)]
    [DataRow(3, 3, 5, false, false, true)]
    [DataRow(5, 3, 5, false, false, false)]
    [DataRow(5, 3, 5, true, false, true)]
    [DataRow(0, 3, 5, false, true, true)]
    [DataRow(null, 3, 5, false, true, true)]
    [DataRow(0, 3, 5, false, false, false)]
    [DataRow(null, 3, 5, false, false, false)]
    public void IsValid_ShouldWork(int? input, int min, int max, bool including, bool allowDefault, bool isValid)
    {
        // Arrange
        var attribute = new BetweenAttribute(min, max, including, allowDefault);

        // Act
        var result = attribute.IsValid(input);

        // Assert
        Assert.AreEqual(isValid, result);
    }
}
