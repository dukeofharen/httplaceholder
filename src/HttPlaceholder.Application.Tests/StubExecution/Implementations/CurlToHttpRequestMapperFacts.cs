using System.IO;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class CurlToHttpRequestMapperFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_CommandIsEmpty_ShouldReturnEmptyResult()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(string.Empty);

        // Assert
        Assert.IsFalse(result.Any());
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_CommandIsNotCurl_ShouldReturnEmptyResult()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest("not a curl command");

        // Assert
        Assert.IsFalse(result.Any());
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_FirefoxOnUbuntu_Scenario1()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/firefox_on_ubuntu_scenario1.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var request = result[0];
        Assert.AreEqual("POST", request.Method);
        Assert.AreEqual("https://api.site.com/api/v1/users/authenticate", request.Url);
        Assert.AreEqual("{}", request.Body);
        Assert.AreEqual(14, request.Headers.Count);

        var headers = request.Headers;
        Assert.AreEqual("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:94.0) Gecko/20100101 Firefox/94.0",
            headers["User-Agent"]);
        Assert.AreEqual("application/json, text/plain, */*", headers["Accept"]);
        Assert.AreEqual("en-US,en;q=0.5", headers["Accept-Language"]);
        Assert.AreEqual("application/json;charset=utf-8", headers[HeaderKeys.ContentType]);
        Assert.AreEqual("Basic ZHVjbzpibGFkaWJsYQ==", headers[HeaderKeys.Authorization]);
        Assert.AreEqual("https://site.com", headers["Origin"]);
        Assert.AreEqual("keep-alive", headers[HeaderKeys.Connection]);
        Assert.AreEqual("empty", headers["Sec-Fetch-Dest"]);
        Assert.AreEqual("cors", headers["Sec-Fetch-Mode"]);
        Assert.AreEqual("same-site", headers["Sec-Fetch-Site"]);
        Assert.AreEqual("1", headers["DNT"]);
        Assert.AreEqual("1", headers["Sec-GPC"]);
        Assert.AreEqual("trailers", headers["TE"]);
        Assert.AreEqual("deflate, gzip, br", headers[HeaderKeys.AcceptEncoding]);
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_FirefoxOnUbuntu_Scenario2()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/firefox_on_ubuntu_scenario2.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var request = result[0];
        Assert.AreEqual("PUT", request.Method);
        Assert.AreEqual("https://api.site.com/api/v1/users", request.Url);
        Assert.AreEqual(
            """{"id":1,"created":"2015-10-21T14:39:55","updated":"2021-11-26T22:10:52","userName":"d","firstName":"d'","lastName":"h h","street":"Road","number":"6","postalCode":"1234AB","city":"Amsterdam","phone":"0612345678","email":"info@example.com","placeId":1,"newsletter":false,"driversLicenseNumber":"112233","emailRepeat":"info@example.com"}""",
            request.Body);
        Assert.AreEqual(14, request.Headers.Count);

        var headers = request.Headers;
        Assert.AreEqual("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:94.0) Gecko/20100101 Firefox/94.0",
            headers["User-Agent"]);
        Assert.AreEqual("application/json, text/plain, */*", headers["Accept"]);
        Assert.AreEqual("en-US,en;q=0.5", headers["Accept-Language"]);
        Assert.AreEqual("application/json;charset=utf-8", headers[HeaderKeys.ContentType]);
        Assert.AreEqual("Bearer VERYLONGSTRING", headers[HeaderKeys.Authorization]);
        Assert.AreEqual("https://site.com", headers["Origin"]);
        Assert.AreEqual("keep-alive", headers[HeaderKeys.Connection]);
        Assert.AreEqual("empty", headers["Sec-Fetch-Dest"]);
        Assert.AreEqual("cors", headers["Sec-Fetch-Mode"]);
        Assert.AreEqual("same-site", headers["Sec-Fetch-Site"]);
        Assert.AreEqual("1", headers["DNT"]);
        Assert.AreEqual("1", headers["Sec-GPC"]);
        Assert.AreEqual("trailers", headers["TE"]);
        Assert.AreEqual("deflate, gzip, br", headers[HeaderKeys.AcceptEncoding]);
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_ChromeOnUbuntuSingle_Scenario1()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/chrome_on_ubuntu_single_scenario1.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var request = result[0];
        Assert.AreEqual("POST", request.Method);
        Assert.AreEqual("https://api.site.com/api/v1/users/authenticate", request.Url);
        Assert.AreEqual("{}", request.Body);
        Assert.AreEqual(14, request.Headers.Count);

        var headers = request.Headers;
        Assert.AreEqual("api.site.com", headers["authority"]);
        Assert.AreEqual("""
                        " Not A;Brand";v="99", "Chromium";v="96", "Google Chrome";v="96"
                        """,
            headers["sec-ch-ua"]);
        Assert.AreEqual("application/json, text/plain, */*", headers["accept"]);
        Assert.AreEqual("application/json;charset=UTF-8", headers[HeaderKeys.ContentType.ToLower()]);
        Assert.AreEqual("Basic dXNlcjpwYXNz", headers[HeaderKeys.Authorization.ToLower()]);
        Assert.AreEqual("?0", headers["sec-ch-ua-mobile"]);
        Assert.AreEqual(
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36",
            headers["user-agent"]);
        Assert.AreEqual("""
                        "Linux"
                        """, headers["sec-ch-ua-platform"]);
        Assert.AreEqual("https://site.com", headers["origin"]);
        Assert.AreEqual("same-site", headers["sec-fetch-site"]);
        Assert.AreEqual("cors", headers["sec-fetch-mode"]);
        Assert.AreEqual("empty", headers["sec-fetch-dest"]);
        Assert.AreEqual("en-US,en;q=0.9,nl;q=0.8", headers["accept-language"]);
        Assert.AreEqual("deflate, gzip, br", headers[HeaderKeys.AcceptEncoding]);
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_ChromeOnUbuntuSingle_Scenario2()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/chrome_on_ubuntu_single_scenario2.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var request = result[0];
        Assert.AreEqual("PUT", request.Method);
        Assert.AreEqual("https://api.site.com/api/v1/users", request.Url);
        Assert.AreEqual(
            """{"id":1,"created":"2015-10-21T14:39:55","updated":"2021-11-26T22:10:52","userName":"d","firstName":"d'","lastName":"h","street":"Road","number":"6","postalCode":"1234AB","city":"Amsterdam","phone":"0612345678","email":"info@example.com","placeId":1,"newsletter":false,"driversLicenseNumber":"112233","emailRepeat":"info@example.com"}""",
            request.Body);
        Assert.AreEqual(14, request.Headers.Count);

        var headers = request.Headers;
        Assert.AreEqual("api.site.com", headers["authority"]);
        Assert.AreEqual("""
                        " Not A;Brand";v="99", "Chromium";v="96", "Google Chrome";v="96"
                        """,
            headers["sec-ch-ua"]);
        Assert.AreEqual("application/json, text/plain, */*", headers["accept"]);
        Assert.AreEqual("application/json;charset=UTF-8", headers[HeaderKeys.ContentType.ToLower()]);
        Assert.AreEqual("Bearer VERYLONGSTRING", headers[HeaderKeys.Authorization.ToLower()]);
        Assert.AreEqual("?0", headers["sec-ch-ua-mobile"]);
        Assert.AreEqual(
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36",
            headers["user-agent"]);
        Assert.AreEqual("""
                        "Linux"
                        """, headers["sec-ch-ua-platform"]);
        Assert.AreEqual("https://site.com", headers["origin"]);
        Assert.AreEqual("same-site", headers["sec-fetch-site"]);
        Assert.AreEqual("cors", headers["sec-fetch-mode"]);
        Assert.AreEqual("empty", headers["sec-fetch-dest"]);
        Assert.AreEqual("en-US,en;q=0.9,nl;q=0.8", headers["accept-language"]);
        Assert.AreEqual("deflate, gzip, br", headers[HeaderKeys.AcceptEncoding]);
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_ChromeOnUbuntuMultiple()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/chrome_on_ubuntu_multiple_curls.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        Assert.AreEqual(3, result.Length);

        var req1 = result[0];
        Assert.AreEqual("GET", req1.Method);
        Assert.AreEqual("https://site.com/_nuxt/fonts/fa-solid-900.3eb06c7.woff2", req1.Url);

        var headers1 = req1.Headers;
        Assert.AreEqual(2, headers1.Count);
        Assert.AreEqual("https://site.com", headers1["Origin"]);
        Assert.AreEqual("deflate, gzip, br", headers1[HeaderKeys.AcceptEncoding]);

        var req2 = result[1];
        Assert.AreEqual("GET", req2.Method);
        Assert.AreEqual("https://site.com/_nuxt/css/4cda201.css", req2.Url);

        var headers2 = req2.Headers;
        Assert.AreEqual(4, headers2.Count);
        Assert.AreEqual("site.com", headers2["authority"]);
        Assert.AreEqual("en-US,en;q=0.9,nl;q=0.8", headers2["accept-language"]);
        Assert.AreEqual("Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=", headers2["cookie"]);
        Assert.AreEqual("deflate, gzip, br", headers2[HeaderKeys.AcceptEncoding]);

        var req3 = result[2];
        Assert.AreEqual("GET", req3.Method);
        Assert.AreEqual("https://site.com/_nuxt/1d6c3a9.js", req3.Url);

        var headers3 = req3.Headers;
        Assert.AreEqual(3, headers3.Count);
        Assert.AreEqual("site.com", headers3["authority"]);
        Assert.AreEqual("Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=", headers3["cookie"]);
        Assert.AreEqual("deflate, gzip, br", headers3[HeaderKeys.AcceptEncoding]);
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_ChromeOnUbuntuMultiline()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/chrome_on_linux_body_multiline.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        var request = result[0];
        Assert.AreEqual("POST", request.Method);
        Assert.AreEqual("http://localhost:5000/moi-wiebe", request.Url);
        Assert.AreEqual("""{\n  "stringValue": "text",\n  "intValue": 3\n}""", request.Body);

        var headers = request.Headers;
        Assert.AreEqual(2, headers.Count);
        Assert.AreEqual("keep-alive", headers[HeaderKeys.Connection]);
        Assert.AreEqual("deflate, gzip, br", headers[HeaderKeys.AcceptEncoding]);
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_FormData()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/curl_with_form_data.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        var req1 = result[0];
        Assert.AreEqual("POST", req1.Method);
        Assert.AreEqual("https://api.site.com/request-path", req1.Url);
        Assert.AreEqual("param1=value1&param2=value2", req1.Body);

        var headers1 = req1.Headers;
        Assert.AreEqual(MimeTypes.MultipartFormDataMime, headers1[HeaderKeys.ContentType]);

        var req2 = result[1];
        Assert.AreEqual("POST", req2.Method);
        Assert.AreEqual("https://api.site.com/request-path", req2.Url);
        Assert.AreEqual("param1=value1&param2=value2", req2.Body);

        var headers2 = req2.Headers;
        Assert.AreEqual(MimeTypes.UrlEncodedFormMime, headers2[HeaderKeys.ContentType]);
    }

    [TestMethod]
    public void MapCurlCommandsToHttpRequest_BasicAuthFlag()
    {
        // Arrange
        var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
        var command = File.ReadAllText("Resources/cURL/curl_with_basic_auth.txt");

        // Act
        var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var req1 = result[0];
        Assert.AreEqual("GET", req1.Method);
        Assert.AreEqual("https://api.site.com/request-path", req1.Url);
        Assert.AreEqual("Basic dXNlcjpwYXNz", req1.Headers[HeaderKeys.Authorization]);
    }
}
