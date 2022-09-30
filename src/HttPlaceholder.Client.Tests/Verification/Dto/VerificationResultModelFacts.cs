using HttPlaceholder.Client.Verification.Dto;
using HttPlaceholder.Client.Verification.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests.Verification.Dto;

[TestClass]
public class VerificationResultModelFacts
{
    [TestMethod]
    public void EnsureVerificationPassed_Passed()
    {
        // Arrange
        var model = new VerificationResultModel {Passed = true, Message = "message"};

        // Act / Assert
        model.EnsureVerificationPassed();
    }

    [TestMethod]
    public void EnsureVerificationPassed_NotPassed()
    {
        // Arrange
        var model = new VerificationResultModel {Passed = false, Message = "message"};

        // Act
        var ex = Assert.ThrowsException<StubVerificationFailedException>(() => model.EnsureVerificationPassed());

        // Assert
        Assert.AreEqual(model.Message, ex.Message);
    }
}
