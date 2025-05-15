using HttPlaceholder.Client.Dto.Import;
using HttPlaceholder.Client.Implementations;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class HttPlaceholderClientTests
{
    [DataTestMethod]
    [DataRow(false, null, null, "url?doNotCreateStub=False")]
    [DataRow(false, "", "", "url?doNotCreateStub=False")]
    [DataRow(true, "", "", "url?doNotCreateStub=True")]
    [DataRow(true, "tenant1", "", "url?doNotCreateStub=True&tenant=tenant1")]
    [DataRow(true, "tenant1", "prefix-", "url?doNotCreateStub=True&tenant=tenant1&stubIdPrefix=prefix-")]
    public void PrependImportQueryString_HappyFlow(bool doNotCreateStub, string tenant, string stubIdPrefix,
        string expectedResult)
    {
        // Arrange
        var model = new ImportStubsModel
        {
            Tenant = tenant,
            DoNotCreateStub = doNotCreateStub,
            StubIdPrefix = stubIdPrefix
        };

        // Act
        var result = HttPlaceholderClient.PrependImportQueryString("url", model);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
