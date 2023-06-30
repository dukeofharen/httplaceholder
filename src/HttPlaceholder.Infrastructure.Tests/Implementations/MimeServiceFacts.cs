using HttPlaceholder.Infrastructure.Implementations;

namespace HttPlaceholder.Infrastructure.Tests.Implementations;

[TestClass]
public class MimeServiceFacts
{
    private readonly MimeService _mimeService = new();

    [DataTestMethod]
    [DataRow(".jpg", "image/jpeg")]
    [DataRow(".jpeg", "image/jpeg")]
    [DataRow("/path/somefile.jpeg", "image/jpeg")]
    [DataRow("file.bin", "application/octet-stream")]
    [DataRow("jpeg", "application/octet-stream")]
    [DataRow("file.json", "application/json")]
    [DataRow("FILE.JSON", "application/json")]
    public void GetMimeType_HappyFlow(string input, string expectedOutput)
    {
        // Act
        var result = _mimeService.GetMimeType(input);

        // Assert
        Assert.AreEqual(expectedOutput, result);
    }
}
