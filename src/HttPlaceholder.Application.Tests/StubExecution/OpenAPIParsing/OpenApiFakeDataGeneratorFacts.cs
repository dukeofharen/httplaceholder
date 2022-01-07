using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.Tests.StubExecution.OpenAPIParsing;

[TestClass]
public class OpenApiFakeDataGeneratorFacts
{
    private readonly OpenApiFakeDataGenerator _generator = new();

    [TestMethod]
    public void GetRandomValue_Boolean()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "boolean"};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsInstanceOfType(result, typeof(bool));
    }

    [TestMethod]
    public void GetRandomValue_Null()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "null"};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetRandomValue_Default()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "unknown"};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsTrue(string.IsNullOrWhiteSpace((string)result));
    }

    [DataTestMethod]
    [DataRow("byte")]
    [DataRow("binary")]
    public void GetRandomValue_StringValue_ByteAndBinary(string format)
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "string", Format = format};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String((string)result));
        Assert.IsFalse(string.IsNullOrWhiteSpace(decoded));
    }

    [TestMethod]
    public void GetRandomValue_StringValue_Date()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "string", Format = "date"};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsTrue(DateTime.TryParseExact((string)result, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out _));
    }

    [TestMethod]
    public void GetRandomValue_StringValue_DateTime()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "string", Format = "date-time"};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsTrue(DateTime.TryParseExact((string)result, "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out _));
    }

    [TestMethod]
    public void GetRandomValue_StringValue_Default()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "string", Format = string.Empty};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace((string)result));
    }

    [TestMethod]
    public void GetRandomValue_StringValue_Enum()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "string",
            Enum = new List<IOpenApiAny>
            {
                new OpenApiString("cat"), new OpenApiString("dog"), new OpenApiString("rabbit")
            }
        };

        // Act
        var result = (string)OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsTrue(result is "cat" or "dog" or "rabbit");
    }

    [TestMethod]
    public void GetRandomValue_OneOf()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            OneOf = new List<OpenApiSchema> {new() {Type = "string", Format = string.Empty}}
        };

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace((string)result));
    }

    [TestMethod]
    public void GetRandomValue_AllOf()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            AllOf = new List<OpenApiSchema> {new() {Type = "string", Format = string.Empty}}
        };

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace((string)result));
    }

    [DataTestMethod]
    [DataRow("int32")]
    [DataRow("int64")]
    [DataRow(null)]
    public void GetRandomValue_Integer(string format)
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "integer", Format = format};

        // Act
        var result = OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsInstanceOfType(result, typeof(long));
    }

    [TestMethod]
    public void GetRandomValue_Object()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                {"stringKey", new OpenApiSchema {Type = "string"}},
                {"intKey", new OpenApiSchema {Type = "integer"}},
            }
        };

        // Act
        var result = (IDictionary<string, object>)OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.IsInstanceOfType(result["stringKey"], typeof(string));
        Assert.IsInstanceOfType(result["intKey"], typeof(long));
    }

    [TestMethod]
    public void GetRandomValue_Array()
    {
        // Arrange
        var schema = new OpenApiSchema
        {
            Type = "array",
            Items = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    {"stringKey", new OpenApiSchema {Type = "string"}},
                    {"intKey", new OpenApiSchema {Type = "integer"}},
                }
            }
        };

        // Act
        var result = (object[])OpenApiFakeDataGenerator.GetRandomValue(schema);

        // Assert
        Assert.IsTrue(result.Length >= 1);
        foreach (var res in result)
        {
            var obj = (IDictionary<string, object>)res;
            Assert.IsInstanceOfType(obj["stringKey"], typeof(string));
            Assert.IsInstanceOfType(obj["intKey"], typeof(long));
        }
    }

    [TestMethod]
    public void GetRandomStringValue_HappyFlow()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "integer"};

        // Act
        var result = _generator.GetRandomStringValue(schema);

        // Assert
        Assert.IsTrue(long.TryParse(result, out _));
    }

    [TestMethod]
    public void GetRandomStringValue_HappyFlow_Null()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "null"};

        // Act
        var result = _generator.GetRandomStringValue(schema);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetRandomJsonStringValue_HappyFlow()
    {
        // Arrange
        var schema = new OpenApiSchema {Type = "string"};

        // Act
        var result = _generator.GetRandomJsonStringValue(schema);

        // Assert
        var rawResult = JsonConvert.DeserializeObject<string>(result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(rawResult));
    }

    [TestMethod]
    public void GetResponseJsonExample_NoExamples_ShouldReturnNull()
    {
        // Arrange
        var mediaType = new OpenApiMediaType {Examples = null};

        // Act
        var result = _generator.GetJsonExample(mediaType);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetResponseJsonExample_ExampleIsNull_ShouldReturnNull()
    {
        // Arrange
        var mediaType = new OpenApiMediaType
        {
            Examples = new Dictionary<string, OpenApiExample> {{"foo", new OpenApiExample {Value = null}}}
        };

        // Act
        var result = _generator.GetJsonExample(mediaType);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetResponseJsonExample_Example_ExampleIsOpenApiObject_ShouldReturnJsonString()
    {
        // Arrange
        var obj = new OpenApiObject {{"key", new OpenApiString("val")}};
        var mediaType = new OpenApiMediaType {Example = obj};

        // Act
        var result = _generator.GetJsonExample(mediaType);

        // Assert
        Assert.AreEqual(@"{
  ""key"": ""val""
}", result);
    }

    [TestMethod]
    public void GetResponseJsonExample_Examples_ExampleIsOpenApiObject_ShouldReturnJsonString()
    {
        // Arrange
        var obj = new OpenApiObject {{"key", new OpenApiString("val")}};
        var mediaType = new OpenApiMediaType
        {
            Examples = new Dictionary<string, OpenApiExample> {{"foo", new OpenApiExample {Value = obj}}}
        };

        // Act
        var result = _generator.GetJsonExample(mediaType);

        // Assert
        Assert.AreEqual(@"{
  ""key"": ""val""
}", result);
    }

    [TestMethod]
    public void GetResponseJsonExample_Examples_ExampleIsOpenApiString_ShouldReturnJsonString()
    {
        // Arrange
        var obj = new OpenApiString(@"{
  ""key"": ""val""
}");
        var mediaType = new OpenApiMediaType
        {
            Examples = new Dictionary<string, OpenApiExample> {{"foo", new OpenApiExample {Value = obj}}}
        };

        // Act
        var result = _generator.GetJsonExample(mediaType);

        // Assert
        Assert.AreEqual(@"{
  ""key"": ""val""
}", result);
    }

    [DataTestMethod]
    [DataRow(AnyType.Array)]
    [DataRow(AnyType.Object)]
    [DataRow(AnyType.Null)]
    [DataRow(null)]
    public void ExtractExampleFromOpenApiAny_UnsupportedTypes(AnyType anyType)
    {
        // Arrange
        var any = new Mock<IOpenApiAny>();
        any
            .Setup(m => m.AnyType)
            .Returns(anyType);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractExampleFromOpenApiAny(any.Object);

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    public void ExtractExampleFromOpenApiAny_Primitive()
    {
        // Arrange
        var any = new OpenApiString("test1");

        // Act
        var result = OpenApiFakeDataGenerator.ExtractExampleFromOpenApiAny(any);

        // Assert
        Assert.AreEqual("test1", result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Binary()
    {
        // Arrange
        var any = new OpenApiBinary(new byte[] {1, 2, 3});

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Binary);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Byte()
    {
        // Arrange
        var any = new OpenApiByte(new byte[] {1, 2, 3});

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Byte);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Boolean()
    {
        // Arrange
        var any = new OpenApiBoolean(true);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Boolean);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Integer()
    {
        // Arrange
        var any = new OpenApiInteger(123);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Integer);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Long()
    {
        // Arrange
        var any = new OpenApiLong(123);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Long);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Float()
    {
        // Arrange
        var any = new OpenApiFloat(3_000.5F);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Float);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Double()
    {
        // Arrange
        var any = new OpenApiDouble(1.23);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Double);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_String()
    {
        // Arrange
        var any = new OpenApiString("test1");

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.String);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Date()
    {
        // Arrange
        var any = new OpenApiDate(DateTime.Today);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Date);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_DateTime()
    {
        // Arrange
        var any = new OpenApiDateTime(DateTime.Now);

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.DateTime);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Password()
    {
        // Arrange
        var any = new OpenApiPassword("password");

        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(any, PrimitiveType.Password);

        // Assert
        Assert.AreEqual(any.Value, result);
    }

    [TestMethod]
    public void ExtractOpenApiPrimitiveValue_Null()
    {
        // Act
        var result = OpenApiFakeDataGenerator.ExtractOpenApiPrimitiveValue(null, null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetExampleForHeader_SingleExampleNotSet_SchemaExampleNotSet_MultipleExamplesSet_ShouldReturnExample()
    {
        // Arrange
        var input = new OpenApiHeader
        {
            Example = null,
            Examples = new Dictionary<string, OpenApiExample>
            {
                {"foo", new OpenApiExample {Value = new OpenApiInteger(123)}}
            },
            Schema = new OpenApiSchema
            {
                Example = null
            }
        };

        // Act
        var result = _generator.GetExampleForHeader(input);

        // Assert
        Assert.AreEqual(123, result);
    }

    [TestMethod]
    public void GetExampleForHeader_SingleExampleNotSet_SchemaExampleSet_ShouldReturnExample()
    {
        // Arrange
        var input = new OpenApiHeader
        {
            Example = null,
            Examples = new Dictionary<string, OpenApiExample>
            {
                {"foo", new OpenApiExample {Value = new OpenApiInteger(123)}}
            },
            Schema = new OpenApiSchema
            {
                Example = new OpenApiInteger(789)
            }
        };

        // Act
        var result = _generator.GetExampleForHeader(input);

        // Assert
        Assert.AreEqual(789, result);
    }

    [TestMethod]
    public void GetExampleForHeader_SingleExampleSet_ShouldReturnExample()
    {
        // Arrange
        var input = new OpenApiHeader
        {
            Example = new OpenApiInteger(456),
            Examples = new Dictionary<string, OpenApiExample>
            {
                {"foo", new OpenApiExample {Value = new OpenApiInteger(123)}}
            },
            Schema = new OpenApiSchema
            {
                Example = new OpenApiInteger(789)
            }
        };

        // Act
        var result = _generator.GetExampleForHeader(input);

        // Assert
        Assert.AreEqual(456, result);
    }

    [TestMethod]
    public void GetExampleForHeader_NoExampleFound_ShouldReturnNull()
    {
        // Arrange
        var input = new OpenApiHeader
        {
            Example = null,
            Examples = null
        };

        // Act
        var result = _generator.GetExampleForHeader(input);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetExampleForParameter_SingleExampleNotSet_SchemaExampleNotSet_MultipleExamplesSet_ShouldReturnExample()
    {
        // Arrange
        var input = new OpenApiParameter
        {
            Example = null,
            Examples = new Dictionary<string, OpenApiExample>
            {
                {"foo", new OpenApiExample {Value = new OpenApiInteger(123)}}
            },
            Schema = new OpenApiSchema
            {
                Example = null
            }
        };

        // Act
        var result = _generator.GetExampleForParameter(input);

        // Assert
        Assert.AreEqual(123, result);
    }

    [TestMethod]
    public void GetExampleForParameter_SingleExampleNotSet_SchemaExampleSet_ShouldReturnExample()
    {
        // Arrange
        var input = new OpenApiParameter
        {
            Example = null,
            Examples = new Dictionary<string, OpenApiExample>
            {
                {"foo", new OpenApiExample {Value = new OpenApiInteger(123)}}
            },
            Schema = new OpenApiSchema
            {
                Example = new OpenApiInteger(789)
            }
        };

        // Act
        var result = _generator.GetExampleForParameter(input);

        // Assert
        Assert.AreEqual(789, result);
    }

    [TestMethod]
    public void GetExampleForParameter_SingleExampleSet_ShouldReturnExample()
    {
        // Arrange
        var input = new OpenApiParameter
        {
            Example = new OpenApiInteger(456),
            Examples = new Dictionary<string, OpenApiExample>
            {
                {"foo", new OpenApiExample {Value = new OpenApiInteger(123)}}
            },
            Schema = new OpenApiSchema
            {
                Example = new OpenApiInteger(789)
            }
        };

        // Act
        var result = _generator.GetExampleForParameter(input);

        // Assert
        Assert.AreEqual(456, result);
    }

    [TestMethod]
    public void GetExampleForParameter_NoExampleFound_ShouldReturnNull()
    {
        // Arrange
        var input = new OpenApiParameter
        {
            Example = null,
            Examples = null
        };

        // Act
        var result = _generator.GetExampleForParameter(input);

        // Assert
        Assert.IsNull(result);
    }
}
