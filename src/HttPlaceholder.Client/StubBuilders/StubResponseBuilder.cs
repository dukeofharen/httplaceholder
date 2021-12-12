using System;
using System.Collections.Generic;
using System.Net;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Stubs;
using Newtonsoft.Json;

namespace HttPlaceholder.Client.StubBuilders;

/// <summary>
/// Class for building the stub response.
/// </summary>
public sealed class StubResponseBuilder
{
    private readonly StubResponseDto _response = new();

    private StubResponseBuilder()
    {
    }

    public static StubResponseBuilder Begin() => new();

    public StubResponseBuilder WithHttpStatusCode(int statusCode)
    {
        _response.StatusCode = statusCode;
        return this;
    }

    public StubResponseBuilder WithHttpStatusCode(HttpStatusCode statusCode)
    {
        _response.StatusCode = (int)statusCode;
        return this;
    }

    public StubResponseBuilder WithContentType(string contentType)
    {
        _response.ContentType = contentType;
        return this;
    }

    public StubResponseBuilder WithTextResponseBody(string text)
    {
        _response.Text = text;
        return this;
    }

    public StubResponseBuilder WithBase64ResponseBody(string base64)
    {
        _response.Base64 = base64;
        return this;
    }

    public StubResponseBuilder WithBase64ResponseBody(byte[] bytes)
    {
        _response.Base64 = Convert.ToBase64String(bytes);
        return this;
    }

    public StubResponseBuilder WithJsonBody(string json)
    {
        _response.Json = json;
        return this;
    }

    public StubResponseBuilder WithJsonBody(object obj)
    {
        _response.Json = JsonConvert.SerializeObject(obj);
        return this;
    }

    public StubResponseBuilder WithXmlBody(string xml)
    {
        _response.Xml = xml;
        return this;
    }

    public StubResponseBuilder WithHtmlBody(string html)
    {
        _response.Html = html;
        return this;
    }

    public StubResponseBuilder WithFile(string filePath)
    {
        _response.File = filePath;
        return this;
    }

    public StubResponseBuilder WithResponseHeader(string key, string value)
    {
        _response.Headers ??= new Dictionary<string, string>();
        _response.Headers.Add(key, value);
        return this;
    }

    public StubResponseBuilder WithExtraDuration(int milliseconds)
    {
        _response.ExtraDuration = milliseconds;
        return this;
    }

    public StubResponseBuilder WithTemporaryRedirect(string url)
    {
        _response.TemporaryRedirect = url;
        return this;
    }

    public StubResponseBuilder WithPermanentRedirect(string url)
    {
        _response.PermanentRedirect = url;
        return this;
    }

    public StubResponseBuilder WithDynamicModeEnabled()
    {
        _response.EnableDynamicMode = true;
        return this;
    }

    public StubResponseBuilder WithDynamicModeDisabled()
    {
        _response.EnableDynamicMode = false;
        return this;
    }

    public StubResponseBuilder WithReverseProxy(string url, bool? appendQueryString, bool? appendPath, bool? replaceRootUrl)
    {
        _response.ReverseProxy = new StubResponseReverseProxyDto
        {
            Url = url,
            AppendPath = appendPath,
            AppendQueryString = appendQueryString,
            ReplaceRootUrl = replaceRootUrl
        };
        return this;
    }

    public StubResponseBuilder WithLineEndings(LineEndingType lineEndingType)
    {
        _response.LineEndings = lineEndingType;
        return this;
    }

    public StubResponseBuilder WithWindowsLineEndings() => WithLineEndings(LineEndingType.Windows);

    public StubResponseBuilder WithUnixLineEndings() => WithLineEndings(LineEndingType.Unix);

    public StubResponseBuilder WithImage(
        ResponseImageType imageType,
        int width,
        int height,
        string backgroundColor = "#3d3d3d",
        string text = "HttPlaceholder",
        int fontSize = 7,
        string fontColor = null,
        bool wordWrap = false,
        int jpegQuality = 95)
    {
        _response.Image = new StubResponseImageDto
        {
            Type = imageType,
            Height = height,
            Text = text,
            Width = width,
            BackgroundColor = backgroundColor,
            FontColor = fontColor,
            FontSize = fontSize,
            JpegQuality = jpegQuality,
            WordWrap = wordWrap
        };
        return this;
    }

    public StubResponseBuilder ShouldClearScenario()
    {
        EnsureScenarioDto();
        _response.Scenario.ClearState = true;
        return this;
    }

    public StubResponseBuilder SetScenarioStateTo(string state)
    {
        EnsureScenarioDto();
        _response.Scenario.SetScenarioState = state;
        return this;
    }

    public StubResponseDto Build() => _response;

    private void EnsureScenarioDto() => _response.Scenario ??= new StubResponseScenarioDto();
}