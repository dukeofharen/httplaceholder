using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Client.Utilities;

namespace HttPlaceholder.Client.Tests.Utilities;

[TestClass]
public class HttpClientExtensionsFacts
{
    [TestMethod]
    public void ApplyConfiguration_NoAuthSet_ShouldOnlySetBaseUrl()
    {
        // Arrange
        var config = new HttPlaceholderClientConfiguration {RootUrl = "http://localhost:5000"};
        var httpClient = new HttpClient();

        // Act
        httpClient.ApplyConfiguration(config);

        // Assert
        Assert.AreEqual("http://localhost:5000/", httpClient.BaseAddress.OriginalString);
        Assert.IsFalse(httpClient.DefaultRequestHeaders.Any(h => h.Key == "Authorization"));
    }

    [TestMethod]
    public void ApplyConfiguration_AuthSet_ShouldSetBaseUrlAndAuth()
    {
        // Arrange
        var config = new HttPlaceholderClientConfiguration
        {
            RootUrl = "http://localhost:5000", Username = "username", Password = "password"
        };
        var httpClient = new HttpClient();

        // Act
        httpClient.ApplyConfiguration(config);

        // Assert
        Assert.AreEqual("http://localhost:5000/", httpClient.BaseAddress.OriginalString);

        var authHeader = httpClient.DefaultRequestHeaders.Single(h => h.Key == "Authorization");
        var base64Decoded =
            Encoding.UTF8.GetString(
                Convert.FromBase64String(authHeader.Value.Single().Replace("Basic ", string.Empty)));
        var parts = base64Decoded.Split(':');
        Assert.AreEqual("username", parts[0]);
        Assert.AreEqual("password", parts[1]);
    }

    [TestMethod]
    public void ApplyConfiguration_DefaultHttpHeadersSet_ShouldAddHeaders()
    {
        // Arrange
        var config = new HttPlaceholderClientConfiguration
        {
            RootUrl = "http://localhost:5000",
            DefaultHttpHeaders = new Dictionary<string, string>
            {
                {"Header1", "Value1"}
            }
        };
        var httpClient = new HttpClient();

        // Act
        httpClient.ApplyConfiguration(config);

        // Assert
        Assert.AreEqual("http://localhost:5000/", httpClient.BaseAddress.OriginalString);
        Assert.AreEqual("Value1", httpClient.DefaultRequestHeaders.Single(h => h.Key == "Header1").Value.Single());
    }
}
