using AutoMapper;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.Tests.StubExecution.Models;

[TestClass]
public class HttpRequestModelFacts
{
    private readonly IMapper _mapper = AutoMapperHelper.CreateMapper();

    [TestMethod]
    public void Map_BodyIsSet_ShouldUseBodyInMapping()
    {
        // Arrange
        var model = new RequestParametersModel { Body = "request body" };

        // Act
        var result = _mapper.Map<HttpRequestModel>(model);

        // Assert
        Assert.AreEqual(model.Body, result.Body);
    }

    [TestMethod]
    public void Map_BodyIsNotSet_BinaryBodyIsSet_BinaryIsText_ShouldUseBodyInMapping()
    {
        // Arrange
        var model = new RequestParametersModel { BinaryBody = "request body"u8.ToArray() };

        // Act
        var result = _mapper.Map<HttpRequestModel>(model);

        // Assert
        Assert.AreEqual("request body", result.Body);
    }

    [TestMethod]
    public void Map_BodyIsNotSet_BinaryBodyIsSet_BinaryIsNotText_ShouldLeaveBodyEmpty()
    {
        // Arrange
        var model = new RequestParametersModel { BinaryBody = [255] };

        // Act
        var result = _mapper.Map<HttpRequestModel>(model);

        // Assert
        Assert.IsNull(result.Body);
    }
}
