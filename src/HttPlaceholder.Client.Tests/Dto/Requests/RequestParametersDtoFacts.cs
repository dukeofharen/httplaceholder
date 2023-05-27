using System.Linq;
using System.Text;
using HttPlaceholder.Client.Dto.Requests;

namespace HttPlaceholder.Client.Tests.Dto.Requests;

[TestClass]
public class RequestParametersDtoFacts
{
    [TestMethod]
    public void GetBodyAsBytes_BodyIsBinary()
    {
        // Arrange
        var dto = new RequestParametersDto {BodyIsBinary = true, Body = Convert.ToBase64String(new byte[] {254, 255})};

        // Act
        var result = dto.GetBodyAsBytes();

        // Assert
        Assert.IsTrue(result.SequenceEqual(new byte[]{254, 255}));
    }

    [TestMethod]
    public void GetBodyAsBytes_BodyIsText()
    {
        // Arrange
        var dto = new RequestParametersDto {BodyIsBinary = false, Body = "Some text"};

        // Act
        var result = dto.GetBodyAsBytes();

        // Assert
        Assert.IsTrue(result.SequenceEqual("Some text"u8.ToArray()));
    }
}
