using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.Models.HAR;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;
using HttPlaceholder.Domain;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.OpenAPIParsing;

[TestClass]
public class OpenApiDataFillerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestMethod]
    public void BuildHttpStatusCode_StatusCodeRegexMatches_ShouldReturnStatusCode()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        const string responseKey = "204";

        // Act
        var result = filler.BuildHttpStatusCode(responseKey);

        // Assert
        Assert.AreEqual(204, result);
    }

    [TestMethod]
    public void BuildHttpStatusCode_StatusCodeRegexDoesNotMatch_ShouldReturn0()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        const string responseKey = "default";

        // Act
        var result = filler.BuildHttpStatusCode(responseKey);

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void BuildResponseBody_ContentIsNull_ShouldReturnNull()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var response = new OpenApiResponse {Content = null};

        // Act
        var result = filler.BuildResponseBody(response);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BuildResponseBody_ContentIsEmpty_ShouldReturnNull()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var response = new OpenApiResponse {Content = new Dictionary<string, OpenApiMediaType>()};

        // Act
        var result = filler.BuildResponseBody(response);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BuildResponseBody_ContentIsNotJson_ShouldReturnNull()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var response = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType> {{Constants.TextMime, new OpenApiMediaType()}}
        };

        // Act
        var result = filler.BuildResponseBody(response);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BuildResponseBody_ExampleIsSet_ShouldReturnExample()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        var schema = new OpenApiSchema();
        var mediaType = new OpenApiMediaType {Schema = schema};
        var response = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType> {{Constants.JsonMime, mediaType}}
        };

        const string example = "JSON EXAMPLE";
        generatorMock
            .Setup(m => m.GetJsonExample(mediaType))
            .Returns(example);

        // Act
        var result = filler.BuildResponseBody(response);

        // Assert
        Assert.AreEqual(example, result);
    }

    [TestMethod]
    public void BuildResponseBody_ContentIsJson_ShouldReturnJsonString()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        var schema = new OpenApiSchema();
        const string jsonString = "json string";
        generatorMock
            .Setup(m => m.GetRandomJsonStringValue(schema))
            .Returns(jsonString);

        var response = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                {Constants.JsonMime, new OpenApiMediaType {Schema = schema}}
            }
        };

        // Act
        var result = filler.BuildResponseBody(response);

        // Assert
        Assert.AreEqual(jsonString, result);
    }

    [TestMethod]
    public void BuildResponseHeaders_ContentTypeSet_ShouldAddContentTypeHeader()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        var headerSchema = new OpenApiSchema();
        var response = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType> {{Constants.JsonMime, new OpenApiMediaType()}},
            Headers = new Dictionary<string, OpenApiHeader>
            {
                {"x-api-key", new OpenApiHeader {Schema = headerSchema}}
            }
        };
        const string randomString = "random string";
        generatorMock
            .Setup(m => m.GetRandomStringValue(headerSchema))
            .Returns(randomString);

        // Act
        var result = filler.BuildResponseHeaders(response);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(Constants.JsonMime, result["content-type"]);
        Assert.AreEqual(randomString, result["x-api-key"]);
    }

    [TestMethod]
    public void BuildResponseHeaders_ContentTypeNotSet_ShouldNotAddContentTypeHeader()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        var headerSchema = new OpenApiSchema();
        var response = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>(),
            Headers = new Dictionary<string, OpenApiHeader>
            {
                {"x-api-key", new OpenApiHeader {Schema = headerSchema}}
            }
        };
        const string randomString = "random string";
        generatorMock
            .Setup(m => m.GetRandomStringValue(headerSchema))
            .Returns(randomString);

        // Act
        var result = filler.BuildResponseHeaders(response);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(randomString, result["x-api-key"]);
    }

    [TestMethod]
    public void BuildResponseHeaders_UseExample()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        var headerSchema = new OpenApiSchema();
        var header = new OpenApiHeader {Schema = headerSchema};
        var response = new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>(),
            Headers = new Dictionary<string, OpenApiHeader> {{"x-api-key",header}}
        };

        const string example = "example string";
        generatorMock
            .Setup(m => m.GetExampleForHeader(header))
            .Returns(example);

        // Act
        var result = filler.BuildResponseHeaders(response);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(example, result["x-api-key"]);
    }

    [TestMethod]
    public void BuildRequestBody_ContentIsNull_ShouldReturnNull()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var operation = new OpenApiOperation {RequestBody = new OpenApiRequestBody {Content = null}};


        // Act
        var result = filler.BuildRequestBody(operation);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BuildRequestBody_ContentIsEmpty_ShouldReturnNull()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var operation = new OpenApiOperation
        {
            RequestBody = new OpenApiRequestBody {Content = new Dictionary<string, OpenApiMediaType>()}
        };

        // Act
        var result = filler.BuildRequestBody(operation);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BuildRequestBody_ContentIsNotJson_ShouldReturnNull()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var operation = new OpenApiOperation
        {
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {Constants.TextMime, new OpenApiMediaType()}
                }
            }
        };

        // Act
        var result = filler.BuildRequestBody(operation);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BuildRequestBody_ContentIsJson_ShouldReturnJsonString()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        var schema = new OpenApiSchema();
        const string jsonString = "json string";
        generatorMock
            .Setup(m => m.GetRandomJsonStringValue(schema))
            .Returns(jsonString);

        var operation = new OpenApiOperation
        {
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {Constants.JsonMime, new OpenApiMediaType {Schema = schema}}
                }
            }
        };

        // Act
        var result = filler.BuildRequestBody(operation);

        // Assert
        Assert.AreEqual(jsonString, result);
    }

    [TestMethod]
    public void BuildRequestBody_ContentIsJson_UseExample()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        var schema = new OpenApiSchema();
        var mediaType = new OpenApiMediaType {Schema = schema};
        var operation = new OpenApiOperation
        {
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {Constants.JsonMime, mediaType}
                }
            }
        };

        const string example = "example string";
        generatorMock
            .Setup(m => m.GetJsonExample(mediaType))
            .Returns(example);

        // Act
        var result = filler.BuildRequestBody(operation);

        // Assert
        Assert.AreEqual(example, result);
    }

    [TestMethod]
    public void BuildRelativeRequestPath_WithPathParams_WithoutQuery()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        const string basePath = "/api/users/{username}/orders/{orderId}";
        var operation = new OpenApiOperation
        {
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "username",
                    In = ParameterLocation.Path,
                    Schema = new OpenApiSchema {Type = "string"}
                },
                new()
                {
                    Name = "orderId",
                    In = ParameterLocation.Path,
                    Schema = new OpenApiSchema {Type = "integer"}
                }
            }
        };

        generatorMock
            .Setup(m => m.GetExampleForParameter(operation.Parameters[0]))
            .Returns("user1");
        generatorMock
            .Setup(m => m.GetRandomStringValue(operation.Parameters[1].Schema))
            .Returns("123");

        // Act
        var result = filler.BuildRelativeRequestPath(operation, basePath);

        // Assert
        const string expectedPath = "/api/users/user1/orders/123";
        Assert.AreEqual(expectedPath, result);
    }

    [TestMethod]
    public void BuildRelativeRequestPath_WithPathParams_WithQuery()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        const string basePath = "/api/users/{username}/orders/{orderId}";
        var operation = new OpenApiOperation
        {
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "username",
                    In = ParameterLocation.Path,
                    Schema = new OpenApiSchema {Type = "string"}
                },
                new()
                {
                    Name = "orderId",
                    In = ParameterLocation.Path,
                    Schema = new OpenApiSchema {Type = "integer"}
                },
                new()
                {
                    Name = "filter",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "string"}
                },
                new()
                {
                    Name = "anotherquery",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema {Type = "string"}
                },
                new()
                {
                    Name = "headerkey",
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema {Type = "string"}
                }
            }
        };

        generatorMock
            .Setup(m => m.GetRandomStringValue(operation.Parameters[0].Schema))
            .Returns("user1");
        generatorMock
            .Setup(m => m.GetExampleForParameter(operation.Parameters[1]))
            .Returns("123");
        generatorMock
            .Setup(m => m.GetRandomStringValue(operation.Parameters[2].Schema))
            .Returns("filterval");
        generatorMock
            .Setup(m => m.GetExampleForParameter(operation.Parameters[3]))
            .Returns("filterval2");

        // Act
        var result = filler.BuildRelativeRequestPath(operation, basePath);

        // Assert
        const string expectedPath = "/api/users/user1/orders/123?filter=filterval&anotherquery=filterval2";
        Assert.AreEqual(expectedPath, result);
    }

    [TestMethod]
    public void BuildRelativeRequestPath_WithoutPathParams_WithoutQuery()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();

        const string basePath = "/api/users";
        var operation = new OpenApiOperation();

        // Act
        var result = filler.BuildRelativeRequestPath(operation, basePath);

        // Assert
        Assert.AreEqual(basePath, result);
    }

    [TestMethod]
    public void BuildRequestHeaders_WithContent_ShouldAddContentType()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();
        var operation = new OpenApiOperation
        {
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "x-header-1",
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema {Type = "string"}
                },
                new()
                {
                    Name = "orderId",
                    In = ParameterLocation.Path,
                    Schema = new OpenApiSchema {Type = "integer"}
                }
            },
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {Constants.TextMime, new OpenApiMediaType()}
                }
            }
        };

        const string headerValue = "headerval";
        generatorMock
            .Setup(m => m.GetRandomStringValue(operation.Parameters[0].Schema))
            .Returns(headerValue);

        // Act
        var result = filler.BuildRequestHeaders(operation);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(headerValue, result["x-header-1"]);
        Assert.AreEqual(Constants.TextMime, result["content-type"]);
    }

    [TestMethod]
    public void BuildRequestHeaders_WithoutContent_ShouldNotAddContentType()
    {
        // Arrange
        var filler = _mocker.CreateInstance<OpenApiDataFiller>();
        var generatorMock = _mocker.GetMock<IOpenApiFakeDataGenerator>();
        var operation = new OpenApiOperation
        {
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "x-header-1",
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema {Type = "string"}
                },
                new()
                {
                    Name = "orderId",
                    In = ParameterLocation.Path,
                    Schema = new OpenApiSchema {Type = "integer"}
                }
            }
        };

        const string headerValue = "headerval";
        generatorMock
            .Setup(m => m.GetRandomStringValue(operation.Parameters[0].Schema))
            .Returns(headerValue);

        // Act
        var result = filler.BuildRequestHeaders(operation);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(headerValue, result["x-header-1"]);
    }
}
