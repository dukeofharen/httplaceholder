using System.Collections.Generic;
using System.Linq;
using System.Net;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.StubBuilders;

namespace HttPlaceholder.Client.Tests.StubBuilders;

[TestClass]
public class StubResponseBuilderFacts
{
    [TestMethod]
    public void WithHttpStatusCodeAsInt()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithHttpStatusCode(400)
            .Build();

        // Assert
        Assert.AreEqual(400, response.StatusCode);
    }

    [TestMethod]
    public void WithHttpStatusCodeAsHttpStatusCode()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithHttpStatusCode(HttpStatusCode.BadRequest)
            .Build();

        // Assert
        Assert.AreEqual(400, response.StatusCode);
    }

    [TestMethod]
    public void WithContentType()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithContentType("application/json")
            .Build();

        // Assert
        Assert.AreEqual("application/json", response.ContentType);
    }

    [TestMethod]
    public void WithTextResponseBody()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithTextResponseBody("text body")
            .Build();

        // Assert
        Assert.AreEqual("text body", response.Text);
    }

    [TestMethod]
    public void WithBase64ResponseBodyAsString()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithBase64ResponseBody("base64")
            .Build();

        // Assert
        Assert.AreEqual("base64", response.Base64);
    }

    [TestMethod]
    public void WithBase64ResponseBodyAsBytes()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithBase64ResponseBody(new byte[] {1, 2, 3})
            .Build();

        // Assert
        Assert.AreEqual("AQID", response.Base64);
    }

    [TestMethod]
    public void WithJsonBodyAsString()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithJsonBody("json")
            .Build();

        // Assert
        Assert.AreEqual("json", response.Json);
    }

    [TestMethod]
    public void WithJsonBodyAsObject()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithJsonBody(new {key = "val"})
            .Build();

        // Assert
        Assert.AreEqual(@"{""key"":""val""}", response.Json);
    }

    [TestMethod]
    public void WithXmlBody()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithXmlBody("<xml></xml>")
            .Build();

        // Assert
        Assert.AreEqual("<xml></xml>", response.Xml);
    }

    [TestMethod]
    public void WithHtmlBody()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithHtmlBody("<html></html>")
            .Build();

        // Assert
        Assert.AreEqual("<html></html>", response.Html);
    }

    [TestMethod]
    public void WithFile()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithFile("/var/file.jpg")
            .Build();

        // Assert
        Assert.AreEqual("/var/file.jpg", response.File);
    }

    [TestMethod]
    public void WithTextFile()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithTextFile("/var/file.txt")
            .Build();

        // Assert
        Assert.AreEqual("/var/file.txt", response.TextFile);
    }

    [TestMethod]
    public void WithResponseHeader()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithResponseHeader("X-Header-1", "val1")
            .WithResponseHeader("X-Header-2", "val2")
            .Build();

        // Assert
        Assert.AreEqual(2, response.Headers.Count);

        Assert.AreEqual("val1", response.Headers["X-Header-1"]);
        Assert.AreEqual("val2", response.Headers["X-Header-2"]);
    }

    [TestMethod]
    public void WithExtraDuration()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithExtraDuration(15000)
            .Build();

        // Assert
        Assert.AreEqual(15000, response.ExtraDuration);
    }

    [TestMethod]
    public void WithMinimumExtraDuration()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithMinimumExtraDuration(10000)
            .Build();

        // Assert
        Assert.AreEqual(10000, ((StubExtraDurationDto)response.ExtraDuration).Min);
    }

    [TestMethod]
    public void WithExtraDurationMinMax()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithExtraDuration(10000, 15000)
            .Build();

        // Assert
        Assert.AreEqual(10000, ((StubExtraDurationDto)response.ExtraDuration).Min);
        Assert.AreEqual(15000, ((StubExtraDurationDto)response.ExtraDuration).Max);
    }

    [TestMethod]
    public void WithExtraDurationModel()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithExtraDuration(new StubExtraDurationDto {Min = 10000, Max = 15000})
            .Build();

        // Assert
        Assert.AreEqual(10000, ((StubExtraDurationDto)response.ExtraDuration).Min);
        Assert.AreEqual(15000, ((StubExtraDurationDto)response.ExtraDuration).Max);
    }

    [TestMethod]
    public void WithTemporaryRedirect()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithTemporaryRedirect("/temp-redirect")
            .Build();

        // Assert
        Assert.AreEqual("/temp-redirect", response.TemporaryRedirect);
    }

    [TestMethod]
    public void WithPermanentRedirect()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithPermanentRedirect("/permanent-redirect")
            .Build();

        // Assert
        Assert.AreEqual("/permanent-redirect", response.PermanentRedirect);
    }

    [TestMethod]
    public void WithMovedPermanentlyRedirect()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithMovedPermanentlyRedirect("/moved-permanently")
            .Build();

        // Assert
        Assert.AreEqual("/moved-permanently", response.MovedPermanently);
    }

    [TestMethod]
    public void WithDynamicModeEnabled()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithDynamicModeEnabled()
            .Build();

        // Assert
        Assert.IsTrue(response.EnableDynamicMode);
    }

    [TestMethod]
    public void WithDynamicModeDisabled()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithDynamicModeDisabled()
            .Build();

        // Assert
        Assert.IsFalse(response.EnableDynamicMode);
    }

    [TestMethod]
    public void WithReverseProxy()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithReverseProxy("http://localhost:5000", true, true, false)
            .Build();

        // Assert
        Assert.AreEqual("http://localhost:5000", response.ReverseProxy.Url);
        Assert.IsTrue(response.ReverseProxy.AppendQueryString);
        Assert.IsTrue(response.ReverseProxy.AppendPath);
        Assert.IsFalse(response.ReverseProxy.ReplaceRootUrl);
    }

    [TestMethod]
    public void WithLineEndings()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithLineEndings(LineEndingType.Windows)
            .Build();

        // Assert
        Assert.AreEqual(LineEndingType.Windows, response.LineEndings);
    }

    [TestMethod]
    public void WithWindowsLineEndings()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithWindowsLineEndings()
            .Build();

        // Assert
        Assert.AreEqual(LineEndingType.Windows, response.LineEndings);
    }

    [TestMethod]
    public void WithUnixLineEndings()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithUnixLineEndings()
            .Build();

        // Assert
        Assert.AreEqual(LineEndingType.Unix, response.LineEndings);
    }

    [TestMethod]
    public void WithImage()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .WithImage(
                ResponseImageType.Bmp,
                500,
                600,
                "#ababab",
                "stub text",
                10,
                "#000000",
                true,
                96)
            .Build();

        // Assert
        Assert.AreEqual(ResponseImageType.Bmp, response.Image.Type);
        Assert.AreEqual(500, response.Image.Width);
        Assert.AreEqual(600, response.Image.Height);
        Assert.AreEqual("#ababab", response.Image.BackgroundColor);
        Assert.AreEqual("stub text", response.Image.Text);
        Assert.AreEqual(10, response.Image.FontSize);
        Assert.AreEqual("#000000", response.Image.FontColor);
        Assert.IsTrue(response.Image.WordWrap);
        Assert.AreEqual(96, response.Image.JpegQuality);
    }

    [TestMethod]
    public void ShouldClearScenario()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .ShouldClearScenario()
            .Build();

        // Assert
        Assert.IsTrue(response.Scenario.ClearState);
    }

    [TestMethod]
    public void SetScenarioStateTo()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .SetScenarioStateTo("new-state")
            .Build();

        // Assert
        Assert.AreEqual("new-state", response.Scenario.SetScenarioState);
    }

    [TestMethod]
    public void AbortConnection()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .AbortConnection()
            .Build();

        // Assert
        Assert.IsTrue(response.AbortConnection);
    }

    [TestMethod]
    public void StringReplace()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .StringReplace("look for", "replace with", false)
            .Build();

        // Assert
        var replace = (List<StubResponseReplaceDto>)response.Replace;
        Assert.AreEqual(1, replace.Count);

        var dto = replace.Single();
        Assert.AreEqual("look for", dto.Text);
        Assert.AreEqual("replace with", dto.ReplaceWith);
        Assert.IsFalse(dto.IgnoreCase);
    }

    [TestMethod]
    public void RegexReplace()
    {
        // Act
        var response = StubResponseBuilder.Begin()
            .RegexReplace("(.*)", "replace with")
            .Build();

        // Assert
        var replace = (List<StubResponseReplaceDto>)response.Replace;
        Assert.AreEqual(1, replace.Count);

        var dto = replace.Single();
        Assert.AreEqual("(.*)", dto.Regex);
        Assert.AreEqual("replace with", dto.ReplaceWith);
    }
}
