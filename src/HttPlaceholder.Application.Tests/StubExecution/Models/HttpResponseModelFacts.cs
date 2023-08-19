using AutoMapper;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.Tests.StubExecution.Models;

[TestClass]
public class HttpResponseModelFacts
{
    private readonly IMapper _mapper = AutoMapperHelper.CreateMapper();

    [TestMethod]
    public void ResponseBodyIsBinary_ShouldConvertToBase64()
    {
        // Arrange
        var response = new ResponseModel {BodyIsBinary = true, Body = new byte[] {1, 2, 3}};

        // Act
        var result = _mapper.Map<HttpResponseModel>(response);

        // Assert
        Assert.IsTrue(result.ContentIsBase64);
        Assert.AreEqual("AQID", result.Content);
    }

    [TestMethod]
    public void ResponseBodyIsNotBinary_ShouldConvertToStringAsIs()
    {
        // Arrange
        var response = new ResponseModel {BodyIsBinary = false, Body = "some text"u8.ToArray()};

        // Act
        var result = _mapper.Map<HttpResponseModel>(response);

        // Assert
        Assert.IsFalse(result.ContentIsBase64);
        Assert.AreEqual("some text", result.Content);
    }
}
