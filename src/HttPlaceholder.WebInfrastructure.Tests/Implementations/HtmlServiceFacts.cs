using HttPlaceholder.WebInfrastructure.Implementations;

namespace HttPlaceholder.WebInfrastructure.Tests.Implementations;

[TestClass]
public class HtmlServiceFacts
{
    private readonly HtmlService _service = new();

    [TestMethod]
    public void ReadHtml_HappyFlow()
    {
        // Arrange
        const string html = "<html></html>";

        // Act
        var result = _service.ReadHtml(html);

        // Assert
        Assert.AreEqual(html, result.DocumentNode.OuterHtml);
    }
}
