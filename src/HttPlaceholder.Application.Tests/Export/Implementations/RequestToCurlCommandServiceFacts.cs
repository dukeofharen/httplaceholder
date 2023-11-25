using System.Collections.Generic;
using System.Text;
using HttPlaceholder.Application.Export.Implementations;

namespace HttPlaceholder.Application.Tests.Export.Implementations;

[TestClass]
public class RequestToCurlCommandServiceFacts
{
    private readonly RequestToCurlCommandService _service = new();

    public static IEnumerable<object[]> TestData =>
        new[]
        {
            new object[]
            {
                CreateModel("GET", "http://localhost:5000/url",
                    new Dictionary<string, string> {{"Accept", "*/*"}}),
                "curl 'http://localhost:5000/url' -H 'Accept: */*'"
            },
            new object[]
            {
                CreateModel("DELETE", "http://localhost:5000/url",
                    new Dictionary<string, string> {{"Accept", "*/*"}}),
                "curl -X DELETE 'http://localhost:5000/url' -H 'Accept: */*'"
            },
            new object[]
            {
                CreateModel("POST", "http://localhost:5000/url",
                    new Dictionary<string, string> {{"Accept", "*/*"}, {"Content-Type", "application/json"}, {"Content-Length", "100"}},
                    """
                    {
                        "key": "val"
                    }
                    """),
                """
                curl 'http://localhost:5000/url' -H 'Accept: */*' -H 'Content-Type: application/json' -d '{
                    "key": "val"
                }'
                """
            },
            new object[]
            {
                CreateModel("PUT", "http://localhost:5000/url",
                    new Dictionary<string, string> {{"Accept", "*/*"}, {"Content-Type", "application/json"}, {"Content-Length", "100"}},
                    """
                    {
                        "key": "val"
                    }
                    """),
                """
                curl -X PUT 'http://localhost:5000/url' -H 'Accept: */*' -H 'Content-Type: application/json' -d '{
                    "key": "val"
                }'
                """
            }
        };

    [DataTestMethod]
    [DynamicData(nameof(TestData))]
    public void Convert_HappyFlow(RequestResultModel model, string expectedCurlCommand)
    {
        // Act
        var result = _service.Convert(model);

        // Assert
        Assert.AreEqual(expectedCurlCommand, result);
    }

    private static RequestResultModel CreateModel(string method, string url, IDictionary<string, string> headers,
        string body = null) =>
        new()
        {
            RequestParameters = new RequestParametersModel
            {
                Headers = headers,
                Method = method,
                Url = url,
                BinaryBody = body != null ? Encoding.UTF8.GetBytes(body) : null
            }
        };
}
