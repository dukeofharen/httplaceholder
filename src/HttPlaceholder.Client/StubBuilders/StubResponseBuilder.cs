using System;
using System.Collections.Generic;
using System.Net;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Stubs;
using Newtonsoft.Json;

namespace HttPlaceholder.Client.StubBuilders;

/// <summary>
///     Class for building the stub response.
/// </summary>
public sealed class StubResponseBuilder
{
    private readonly StubResponseDto _response = new();

    private StubResponseBuilder()
    {
    }

    /// <summary>
    ///     Creates a new <see cref="StubResponseBuilder" /> instance.
    /// </summary>
    /// <returns>A <see cref="StubResponseBuilder" /> instance.</returns>
    public static StubResponseBuilder Begin() => new();

    /// <summary>
    ///     Sets the HTTP status code to the response definition.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithHttpStatusCode(int statusCode)
    {
        _response.StatusCode = statusCode;
        return this;
    }

    /// <summary>
    ///     Sets the HTTP status code to the response definition.
    /// </summary>
    /// <param name="statusCode">The HTTP status code as <see cref="HttpStatusCode" />.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithHttpStatusCode(HttpStatusCode statusCode)
    {
        _response.StatusCode = (int)statusCode;
        return this;
    }

    /// <summary>
    ///     Sets the content type of the request body to the response definition.
    /// </summary>
    /// <param name="contentType">The content type as MIME type.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithContentType(string contentType)
    {
        _response.ContentType = contentType;
        return this;
    }

    /// <summary>
    ///     Sets the response body as plain text to the response definition.
    /// </summary>
    /// <param name="text">The response text.</param>
    /// <returns></returns>
    public StubResponseBuilder WithTextResponseBody(string text)
    {
        _response.Text = text;
        return this;
    }

    /// <summary>
    ///     Sets the response body as base64 to the response definition.
    /// </summary>
    /// <param name="base64">The response as base64.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithBase64ResponseBody(string base64)
    {
        _response.Base64 = base64;
        return this;
    }

    /// <summary>
    ///     Sets the response body as base64 to the response definition.
    /// </summary>
    /// <param name="bytes">The response body as byte array.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithBase64ResponseBody(byte[] bytes)
    {
        _response.Base64 = Convert.ToBase64String(bytes);
        return this;
    }

    /// <summary>
    ///     Sets the response body as JSON to the response definition.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithJsonBody(string json)
    {
        _response.Json = json;
        return this;
    }

    /// <summary>
    ///     Sets the response body as JSON to the response definition.
    /// </summary>
    /// <param name="obj">The JSON as object. The object will be serialized as JSON.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithJsonBody(object obj)
    {
        _response.Json = JsonConvert.SerializeObject(obj);
        return this;
    }

    /// <summary>
    ///     Sets the response body as XML to the response definition.
    /// </summary>
    /// <param name="xml">The XML string.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithXmlBody(string xml)
    {
        _response.Xml = xml;
        return this;
    }

    /// <summary>
    ///     Sets the response body as HTML to the response definition.
    /// </summary>
    /// <param name="html">The HTML string.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithHtmlBody(string html)
    {
        _response.Html = html;
        return this;
    }

    /// <summary>
    ///     Sets a file path to return to the response definition.
    /// </summary>
    /// <param name="filePath">The file path of the file to return.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithFile(string filePath)
    {
        _response.File = filePath;
        return this;
    }

    /// <summary>
    ///     Sets a response header to be returned to the response definition.
    ///     This method can be called multiple times to add multiple response headers to be returned.
    /// </summary>
    /// <param name="key">The response header key.</param>
    /// <param name="value">The response header value.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithResponseHeader(string key, string value)
    {
        _response.Headers ??= new Dictionary<string, string>();
        _response.Headers.Add(key, value);
        return this;
    }

    /// <summary>
    ///     Adds a number of milliseconds the stub should wait extra before returning.
    /// </summary>
    /// <param name="milliseconds">The number of milliseconds to wait.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithExtraDuration(int milliseconds)
    {
        _response.ExtraDuration = milliseconds;
        return this;
    }

    /// <summary>
    ///     Adds a number of milliseconds the stub should wait extra before returning.
    /// </summary>
    /// <param name="min">The minimum number of milliseconds to wait.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithMinimumExtraDuration(int min)
    {
        _response.ExtraDuration = new StubExtraDurationDto {Min = min};
        return this;
    }

    /// <summary>
    ///     Adds a number of milliseconds the stub should wait extra before returning.
    /// </summary>
    /// <param name="min">The minimum number of milliseconds to wait.</param>
    /// <param name="max">The maximum number of milliseconds to wait.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithExtraDuration(int min, int max)
    {
        _response.ExtraDuration = new StubExtraDurationDto {Min = min, Max = max};
        return this;
    }

    /// <summary>
    ///     Adds a number of milliseconds the stub should wait extra before returning.
    /// </summary>
    /// <param name="dto">The <see cref="StubExtraDurationDto" /> that contains the min and max values.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithExtraDuration(StubExtraDurationDto dto)
    {
        _response.ExtraDuration = dto;
        return this;
    }

    /// <summary>
    ///     Adds a temporary redirect to the response definition.
    /// </summary>
    /// <param name="url">The URL to redirect to.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithTemporaryRedirect(string url)
    {
        _response.TemporaryRedirect = url;
        return this;
    }

    /// <summary>
    ///     Adds a permanent redirect to the response definition.
    /// </summary>
    /// <param name="url">The URL to redirect to.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithPermanentRedirect(string url)
    {
        _response.PermanentRedirect = url;
        return this;
    }

    /// <summary>
    ///     Enables the dynamic mode for the response definition.
    /// </summary>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithDynamicModeEnabled()
    {
        _response.EnableDynamicMode = true;
        return this;
    }

    /// <summary>
    ///     Disables the dynamic mode for the response definition.
    /// </summary>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithDynamicModeDisabled()
    {
        _response.EnableDynamicMode = false;
        return this;
    }

    /// <summary>
    ///     Adds a reverse proxy to the response definition.
    /// </summary>
    /// <param name="url">The URL to proxy to.</param>
    /// <param name="appendQueryString">If set to true, appends the query string to the proxied URL.</param>
    /// <param name="appendPath">
    ///     If set to true, appends the path that appears "after" the configured "path" condition to the
    ///     proxied URL.
    /// </param>
    /// <param name="replaceRootUrl">
    ///     If set to true, will replace the root URL of the proxied response with the URL of
    ///     HttPlaceholder.
    /// </param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithReverseProxy(string url, bool? appendQueryString, bool? appendPath,
        bool? replaceRootUrl)
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

    /// <summary>
    ///     Configure the response definition to return the response with a specific type of line endings.
    /// </summary>
    /// <param name="lineEndingType">The <see cref="LineEndingType" />.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithLineEndings(LineEndingType lineEndingType)
    {
        _response.LineEndings = lineEndingType;
        return this;
    }

    /// <summary>
    ///     Configure the response definition to return the stub with Windows line endings.
    /// </summary>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithWindowsLineEndings() => WithLineEndings(LineEndingType.Windows);

    /// <summary>
    ///     Configure the response definition to return the stub with Unix line endings.
    /// </summary>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder WithUnixLineEndings() => WithLineEndings(LineEndingType.Unix);

    /// <summary>
    ///     Configure the response definition to return a stub image.
    /// </summary>
    /// <param name="imageType">A <see cref="ResponseImageType" /> that determines the type of image to be returned.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="backgroundColor">The background color as HTML color code (e.g. '#123456').</param>
    /// <param name="text">The text that should be written to the image.</param>
    /// <param name="fontSize">The font size of the drawn text.</param>
    /// <param name="fontColor">The font color as HTML color code (e.g. '#123456').</param>
    /// <param name="wordWrap">If set to true, draw the text across the image.</param>
    /// <param name="jpegQuality">
    ///     A value between 1 and 100 to set the JPEG image. Of course only used if imageType is
    ///     <see cref="ResponseImageType.Jpeg" />.
    /// </param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
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

    /// <summary>
    ///     Determines whether the scenario should be cleared if the response has been executed.
    /// </summary>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder ShouldClearScenario()
    {
        EnsureScenarioDto();
        _response.Scenario.ClearState = true;
        return this;
    }

    /// <summary>
    ///     Determines what the scenario state should be set to after the response has been executed.
    /// </summary>
    /// <param name="state">The state the scenario should be set to.</param>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder SetScenarioStateTo(string state)
    {
        EnsureScenarioDto();
        _response.Scenario.SetScenarioState = state;
        return this;
    }

    /// <summary>
    ///     Determines that the connection should be reset if the stub was hit.
    /// </summary>
    /// <returns>The current <see cref="StubResponseBuilder" />.</returns>
    public StubResponseBuilder AbortConnection()
    {
        _response.AbortConnection = true;
        return this;
    }

    /// <summary>
    ///     Builds the response definition.
    /// </summary>
    /// <returns>The built <see cref="StubResponseDto" />.</returns>
    public StubResponseDto Build() => _response;

    private void EnsureScenarioDto() => _response.Scenario ??= new StubResponseScenarioDto();
}
