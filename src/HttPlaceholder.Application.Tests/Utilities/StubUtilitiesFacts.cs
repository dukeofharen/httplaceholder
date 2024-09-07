using System.Collections.Generic;
using HttPlaceholder.Application.Utilities;

namespace HttPlaceholder.Application.Tests.Utilities;

[TestClass]
public class StubUtilitiesFacts
{
    [TestMethod]
    public void CleanStubId_IdNotSet_ShouldNotClean()
    {
        // Arrange
        var stub = new StubModel { Id = null };

        // Act
        stub.CleanStubId();

        // Assert
        Assert.IsNull(stub.Id);
    }

    [DataTestMethod]
    [DataRow("../stub-id", "stub-id")]
    [DataRow("stub-id", "stub-id")]
    [DataRow(@"..\stub-id", "stub-id")]
    [DataRow(@"..\..\stub-id", "stub-id")]
    public void CleanStubId_IdSet_ShouldClean(string id, string expectedResult)
    {
        // Arrange
        var stub = new StubModel { Id = id };

        // Act
        stub.CleanStubId();

        // Assert
        Assert.AreEqual(expectedResult, stub.Id);
    }

    [DataTestMethod]
    [DataRow("../scenario-name", "scenario-name")]
    [DataRow("scenario-name", "scenario-name")]
    [DataRow(@"..\scenario-name", "scenario-name")]
    [DataRow(@"..\..\scenario-name", "scenario-name")]
    public void CleanScenarioName_HappyFlow(string scenario, string expectedResult)
    {
        // Act
        var result = StubUtilities.CleanScenarioName(scenario);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [DataTestMethod]
    [DataRow("text/plain", "text/csv", "text/csv")]
    [DataRow("text/plain", "", "text/plain")]
    [DataRow("", "text/csv", "text/csv")]
    [DataRow("text/plain", null, "text/plain")]
    [DataRow(null, "text/csv", "text/csv")]
    public void GetContentType_HappyFlow(string contentTypeHeader, string contentTypeRule, string expectedResult)
    {
        // Arrange
        var headers = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(contentTypeHeader))
        {
            headers.Add(HeaderKeys.ContentType, contentTypeHeader);
        }

        var stub = new StubModel
        {
            Response = new StubResponseModel { ContentType = contentTypeRule, Headers = headers }
        };

        // Act
        var result = stub.GetContentType();

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
