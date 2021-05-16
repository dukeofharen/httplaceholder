using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Stubs;
using Newtonsoft.Json;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// A class for building a <see cref="StubDto"/> in a fluent way.
    /// </summary>
    public sealed class StubBuilder
    {
        private readonly StubDto _stub = new StubDto
        {
            Conditions = new StubConditionsDto(), Response = new StubResponseDto()
        };

        private StubBuilder()
        {
        }

        public static StubBuilder Begin() => new StubBuilder();

        // Main stub variables.
        public StubBuilder WithId(string id)
        {
            _stub.Id = id;
            return this;
        }

        public StubBuilder WithPriority(int priority)
        {
            _stub.Priority = priority;
            return this;
        }

        public StubBuilder WithPriority(PriorityType priority)
        {
            _stub.Priority = (int)priority;
            return this;
        }

        public StubBuilder WithTenant(string tenant)
        {
            _stub.Tenant = tenant;
            return this;
        }

        public StubBuilder WithDescription(string description)
        {
            _stub.Description = description;
            return this;
        }

        public StubBuilder IsEnabled()
        {
            _stub.Enabled = true;
            return this;
        }

        public StubBuilder IsDisabled()
        {
            _stub.Enabled = false;
            return this;
        }

        // Stub condition variables.
        public StubBuilder WithHttpMethod(string method)
        {
            _stub.Conditions.Method = method;
            return this;
        }

        public StubBuilder WithHttpMethod(HttpMethod method)
        {
            _stub.Conditions.Method = method.Method;
            return this;
        }

        public StubBuilder WithPath(string path)
        {
            EnsureUrlConditions();
            _stub.Conditions.Url.Path = path;
            return this;
        }

        public StubBuilder WithQueryStringParameter(string key, string value)
        {
            EnsureUrlConditions();
            _stub.Conditions.Url.Query ??= new Dictionary<string, string>();
            _stub.Conditions.Url.Query.Add(key, value);
            return this;
        }

        public StubBuilder WithFullPath(string fullPath)
        {
            EnsureUrlConditions();
            _stub.Conditions.Url.FullPath = fullPath;
            return this;
        }

        public StubBuilder WithHttpsEnabled()
        {
            EnsureUrlConditions();
            _stub.Conditions.Url.IsHttps = true;
            return this;
        }

        public StubBuilder WithHttpsDisabled()
        {
            EnsureUrlConditions();
            _stub.Conditions.Url.IsHttps = false;
            return this;
        }

        public StubBuilder WithPostedBodySubstring(string bodySubstring)
        {
            var bodyConditions =
                _stub.Conditions.Body != null ? (List<string>)_stub.Conditions.Body : new List<string>();
            bodyConditions.Add(bodySubstring);
            _stub.Conditions.Body = bodyConditions;
            return this;
        }

        public StubBuilder WithPostedFormValue(string key, string value)
        {
            var formConditions = _stub.Conditions.Form != null
                ? (List<StubFormDto>)_stub.Conditions.Form
                : new List<StubFormDto>();
            formConditions.Add(new StubFormDto {Key = key, Value = value});
            _stub.Conditions.Form = formConditions;
            return this;
        }

        public StubBuilder WithRequestHeader(string key, string value)
        {
            _stub.Conditions.Headers ??= new Dictionary<string, string>();
            _stub.Conditions.Headers.Add(key, value);
            return this;
        }

        public StubBuilder WithXPathCondition(string xpath, IDictionary<string, string> namespaces = null)
        {
            var xpathConditions = _stub.Conditions.Xpath != null
                ? (List<StubXpathDto>)_stub.Conditions.Xpath
                : new List<StubXpathDto>();
            xpathConditions.Add(new StubXpathDto {QueryString = xpath, Namespaces = namespaces});
            _stub.Conditions.Xpath = xpathConditions;
            return this;
        }

        public StubBuilder WithJsonPathCondition(string jsonPath)
        {
            var jsonpathConditions = _stub.Conditions.JsonPath != null
                ? (List<string>)_stub.Conditions.JsonPath
                : new List<string>();
            jsonpathConditions.Add(jsonPath);
            _stub.Conditions.JsonPath = jsonpathConditions;
            return this;
        }

        public StubBuilder WithBasicAuthentication(string username, string password)
        {
            _stub.Conditions.BasicAuthentication = new StubBasicAuthenticationDto
            {
                Username = username, Password = password
            };
            return this;
        }

        public StubBuilder WithClientIp(string clientIp)
        {
            _stub.Conditions.ClientIp = clientIp;
            return this;
        }

        public StubBuilder WithIpInBlock(string ipStartingRange, string cidr)
        {
            _stub.Conditions.ClientIp = $"{ipStartingRange}/{cidr}";
            return this;
        }

        public StubBuilder WithHost(string hostname)
        {
            _stub.Conditions.Host = hostname;
            return this;
        }

        public StubBuilder WithJsonObject(object jsonObject)
        {
            _stub.Conditions.Json = jsonObject;
            return this;
        }

        public StubBuilder WithJsonArray(object[] jsonArray)
        {
            _stub.Conditions.Json = jsonArray;
            return this;
        }

        // Response variables.
        public StubBuilder WithHttpStatusCode(int statusCode)
        {
            _stub.Response.StatusCode = statusCode;
            return this;
        }

        public StubBuilder WithHttpStatusCode(HttpStatusCode statusCode)
        {
            _stub.Response.StatusCode = (int)statusCode;
            return this;
        }

        public StubBuilder WithContentType(string contentType)
        {
            _stub.Response.ContentType = contentType;
            return this;
        }

        public StubBuilder WithTextResponseBody(string text)
        {
            _stub.Response.Text = text;
            return this;
        }

        public StubBuilder WithBase64ResponseBody(string base64)
        {
            _stub.Response.Base64 = base64;
            return this;
        }

        public StubBuilder WithBase64ResponseBody(byte[] bytes)
        {
            _stub.Response.Base64 = Convert.ToBase64String(bytes);
            return this;
        }

        public StubBuilder WithJsonBody(string json)
        {
            _stub.Response.Json = json;
            return this;
        }

        public StubBuilder WithJsonBody(object obj)
        {
            _stub.Response.Json = JsonConvert.SerializeObject(obj);
            return this;
        }

        public StubBuilder WithXmlBody(string xml)
        {
            _stub.Response.Xml = xml;
            return this;
        }

        public StubBuilder WithHtmlBody(string html)
        {
            _stub.Response.Html = html;
            return this;
        }

        public StubBuilder WithFile(string filePath)
        {
            _stub.Response.File = filePath;
            return this;
        }

        public StubBuilder WithResponseHeader(string key, string value)
        {
            _stub.Response.Headers ??= new Dictionary<string, string>();
            _stub.Response.Headers.Add(key, value);
            return this;
        }

        public StubBuilder WithExtraDuration(int milliseconds)
        {
            _stub.Response.ExtraDuration = milliseconds;
            return this;
        }

        public StubBuilder WithTemporaryRedirect(string url)
        {
            _stub.Response.TemporaryRedirect = url;
            return this;
        }

        public StubBuilder WithPermanentRedirect(string url)
        {
            _stub.Response.PermanentRedirect = url;
            return this;
        }

        public StubBuilder WithDynamicModeEnabled()
        {
            _stub.Response.EnableDynamicMode = true;
            return this;
        }

        public StubBuilder WithDynamicModeDisabled()
        {
            _stub.Response.EnableDynamicMode = false;
            return this;
        }

        public StubBuilder WithReverseProxy(string url, bool? appendQueryString, bool? appendPath, bool? replaceRootUrl)
        {
            _stub.Response.ReverseProxy = new StubResponseReverseProxyDto
            {
                Url = url,
                AppendPath = appendPath,
                AppendQueryString = appendQueryString,
                ReplaceRootUrl = replaceRootUrl
            };
            return this;
        }

        public StubBuilder WithLineEndings(LineEndingType lineEndingType)
        {
            _stub.Response.LineEndings = lineEndingType;
            return this;
        }

        public StubBuilder WithWindowsLineEndings() => WithLineEndings(LineEndingType.Windows);

        public StubBuilder WithUnixLineEndings() => WithLineEndings(LineEndingType.Unix);

        public StubBuilder WithImage(
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
            _stub.Response.Image = new StubResponseImageDto
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

        public StubDto Build() => _stub;

        private void EnsureUrlConditions() => _stub.Conditions.Url ??= new StubUrlConditionDto();
    }
}
