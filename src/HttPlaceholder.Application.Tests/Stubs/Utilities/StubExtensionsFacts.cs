using HttPlaceholder.Application.Stubs.Utilities;

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
            Conditions = new StubConditionsModel {Url = new StubUrlConditionModel {Path = "/path"}},
            Response = new StubResponseModel {Text = "OK!!"}
        };

        // Act
        var result = stub.EnsureStubId();

        // Assert
        Assert.AreEqual(stub.Id, result);
        Assert.AreEqual("generated-055b2e3db5303ab3279025e78e48db3c", result);
    }

    [DataTestMethod]
    [DataRow("stub-id", "prefix-", "prefix-stub-id")]
    [DataRow(null, "prefix-", "prefix-generated-b6c8f89e49069883c993c2175b68f09c")]
    public void EnsureStubId_StubIdPrefix(string currentStubId, string prefix, string expectedResult)
    {
        // Arrange
        var stub = new StubModel {Id = currentStubId};

        // Act
        var result = stub.EnsureStubId(prefix);

        // Assert
        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(stub.Id, result);
    }
}
