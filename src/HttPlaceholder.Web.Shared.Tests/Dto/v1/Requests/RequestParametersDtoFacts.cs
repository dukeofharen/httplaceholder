using System.Text;
using AutoMapper;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;

namespace HttPlaceholder.Web.Shared.Tests.Dto.v1.Requests;

[TestClass]
public class RequestParametersDtoFacts
{
    private readonly IMapper _mapper = AutoMapperHelper.CreateMapper();

    [TestMethod]
    public void Map_BodyIsSet_ShouldUseBodyInMapping()
    {
        // Arrange
        var model = new RequestParametersModel {Body = "request body"};

        // Act
        var result = _mapper.Map<RequestParametersDto>(model);

        // Assert
        Assert.IsFalse(result.BodyIsBinary);
        Assert.AreEqual(model.Body, result.Body);
    }

    [TestMethod]
    public void Map_BodyIsNotSet_BinaryBodyIsSet_BinaryBodyIsText_ShouldUseBodyInMapping()
    {
        // Arrange
        var model = new RequestParametersModel {BinaryBody = "request body"u8.ToArray()};

        // Act
        var result = _mapper.Map<RequestParametersDto>(model);

        // Assert
        Assert.IsFalse(result.BodyIsBinary);
        Assert.AreEqual("request body", result.Body);
    }

    [TestMethod]
    public void Map_BodyIsNotSet_BinaryBodyIsSet_BinaryBodyIsBinary_ShouldUseBodyInMapping()
    {
        // Arrange
        var model = new RequestParametersModel {BinaryBody = new byte[]{255}};

        // Act
        var result = _mapper.Map<RequestParametersDto>(model);

        // Assert
        Assert.IsTrue(result.BodyIsBinary);
        Assert.AreEqual("/w==", result.Body);
    }
}
