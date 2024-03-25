using HttPlaceholder.Application.Stubs.Utilities;

namespace HttPlaceholder.Application.Tests.Stubs.Utilities;

[TestClass]
public class StubExtensionsFacts
{
    [TestMethod]
    public void EnsureStubId_StubIdAlreadySet_ShouldNotSetItAgain()
    {
        // Arrange
        var stub = new StubModel { Id = "stub1" };

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
            Conditions = new StubConditionsModel { Url = new StubUrlConditionModel { Path = "/path" } },
            Response = new StubResponseModel { Text = "OK!!" }
        };

        // Act
        var result = stub.EnsureStubId();

        // Assert
        Assert.AreEqual(stub.Id, result);
        Assert.AreEqual("generated-662956759e097310a3b9ee8ba43a00db", result);
    }

    [DataTestMethod]
    [DataRow("stub-id", "prefix-", "prefix-stub-id")]
    [DataRow(null, "prefix-", "prefix-generated-0d64002c8a32c99f1863f8f748d6dc12")]
    public void EnsureStubId_StubIdPrefix(string currentStubId, string prefix, string expectedResult)
    {
        // Arrange
        var stub = new StubModel { Id = currentStubId };

        // Act
        var result = stub.EnsureStubId(prefix);

        // Assert
        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(stub.Id, result);
    }
}
