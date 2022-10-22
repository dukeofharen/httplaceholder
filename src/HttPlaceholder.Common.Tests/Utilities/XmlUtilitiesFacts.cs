using System.Xml;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class XmlUtilitiesFacts
{
    private const string Body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>Umbrella</m:StockName>
      <m:Description>An umbrella</m:Description>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";

    [TestMethod]
    public void ParseBodyAndAssignNamespaces_HappyFlow()
    {
        // Arrange
        var doc = new XmlDocument();
        doc.LoadXml(Body);
        var nsManager = new XmlNamespaceManager(doc.NameTable);

        // Act
        nsManager.ParseBodyAndAssignNamespaces(Body);

        // Assert
        Assert.AreEqual("http://www.w3.org/2003/05/soap-envelope", nsManager.LookupNamespace("soap"));
        Assert.AreEqual("http://www.example.org/stock/Reddy", nsManager.LookupNamespace("m"));
    }
}
