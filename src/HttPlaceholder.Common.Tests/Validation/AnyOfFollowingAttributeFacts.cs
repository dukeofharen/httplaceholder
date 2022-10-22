using HttPlaceholder.Common.Validation;

namespace HttPlaceholder.Common.Tests.Validation;

[TestClass]
public class AnyOfFollowingAttributeFacts
{
    [DataTestMethod]
    [DataRow("val1", new object[] {"val1", "val2"}, true)]
    [DataRow("val2", new object[] {"val1", "val2"}, true)]
    [DataRow("VAL2", new object[] {"val1", "val2"}, false)]
    [DataRow(1, new object[] {1, 2}, true)]
    [DataRow(2, new object[] {1, 2}, true)]
    [DataRow(3, new object[] {1, 2}, false)]
    public void IsValid_ShouldWork(object input, object[] objectsToCheck, bool isValid)
    {
        // Arrange
        var attribute = new AnyOfFollowingAttribute(objectsToCheck);

        // Act
        var result = attribute.IsValid(input);

        // Assert
        Assert.AreEqual(isValid, result);
    }
}
