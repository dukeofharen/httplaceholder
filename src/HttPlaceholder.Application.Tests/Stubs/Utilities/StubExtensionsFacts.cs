using HttPlaceholder.Application.Stubs.Utilities;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.Stubs.Utilities;

[TestClass]
public class StubExtensionsFacts
{
    [TestMethod]
    public void EnsureStubId_StubIdAlreadySet_ShouldNotSetItAgain()
    {
        // Arrange
        var stub = new StubModel {Id = "stub1"};

        // Act
        var result = stub.EnsureStubId();

        // Assert
        Assert.AreEqual(stub.Id, result);
        Assert.AreEqual("stub1", result);
    }

    [TestMethod]
    public void EnsureStubId_StubIdNotSet_ShouldSetStubId()
    {
        // Arrange
        var stub = new StubModel
        {
            Id = null,
            Conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Path = "/path"
                }
            },
            Response = new StubResponseModel
            {
                Text = "OK!!"
            }
        };

        // Act
        var result = stub.EnsureStubId();

        // Assert
        Assert.AreEqual(stub.Id, result);
        Assert.AreEqual("generated-0636a401deba2996db19d55558faf594", result);
    }
}
