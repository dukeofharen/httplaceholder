using System;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class ValidationUtilitiesFacts
{
    [TestMethod]
    public async Task IfNull_ResultIsNotNull()
    {
        // Arrange
        var task = Task.FromResult("test123");

        // Act
        var result = await task.IfNull(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual("test123", result);
    }

    [TestMethod]
    public async Task IfNull_ResultIsNull()
    {
        // Arrange
        var task = Task.FromResult((string)null);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => task.IfNull(() => throw new InvalidOperationException()));
    }

    [TestMethod]
    public void IfNull_Value_ResultIsNotNull()
    {
        // Arrange
        var value = "test123";

        // Act
        var result = value.IfNull(() => throw new InvalidOperationException());

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void IfNull_Value_ResultIsNull()
    {
        // Arrange
        string value = null;

        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() => value.IfNull(() => throw new InvalidOperationException()));
    }
}
