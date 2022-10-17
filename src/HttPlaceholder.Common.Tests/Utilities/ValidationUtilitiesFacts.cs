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
}
