using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace HttPlaceholder.TestUtilities.Http;

public class MockHttpContext : HttpContext
{
    private string _actualRedirectUrl;
    private int _statusCode;

    public MockHttpContext()
    {
        ConnectionInfoMock = new Mock<ConnectionInfo>();
        HttpResponseMock = new Mock<HttpResponse>();
        HttpRequestMock = new Mock<HttpRequest>();
        ServiceProviderMock = new Mock<IServiceProvider>();
        Items = new Dictionary<object, object>();
        Session = new MockSession();

        HttpResponseMock
            .SetupSet(m => m.StatusCode = It.IsAny<int>())
            .Callback<int>(code => _statusCode = code);

        ServiceProviderMock
            .Setup(m => m.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns(new MockTempDataDictionaryFactory());

        ServiceProviderMock
            .Setup(m => m.GetService(typeof(IUrlHelperFactory)))
            .Returns(new Mock<IUrlHelperFactory>().Object);

        HttpRequestMock
            .Setup(m => m.Headers)
            .Returns(new MockHeaderDictionary());

        HttpResponseMock
            .Setup(m => m.Headers)
            .Returns(new MockHeaderDictionary());

        HttpResponseMock
            .Setup(m => m.Redirect(It.IsAny<string>()))
            .Callback<string>(u => _actualRedirectUrl = u);
    }

    public Mock<ConnectionInfo> ConnectionInfoMock { get; }

    public Mock<HttpRequest> HttpRequestMock { get; }

    public Mock<HttpResponse> HttpResponseMock { get; }

    public Mock<IServiceProvider> ServiceProviderMock { get; }

    public int GetStatusCode() => _statusCode;

    public string GetRedirectUrl() => _actualRedirectUrl;

    public void SetIp(byte[] ip) =>
        ConnectionInfoMock
            .Setup(m => m.RemoteIpAddress)
            .Returns(new IPAddress(ip));

    public void SetIp(string ip) =>
        ConnectionInfoMock
            .Setup(m => m.RemoteIpAddress)
            .Returns(IPAddress.Parse(ip));

    public void SetHost(string host) =>
        HttpRequestMock
            .Setup(m => m.Host)
            .Returns(new HostString(host));

    public void SetRequestMethod(string method) =>
        HttpRequestMock
            .Setup(m => m.Method)
            .Returns(method);

    public void SetQueryString(string queryString) =>
        HttpRequestMock
            .Setup(m => m.QueryString)
            .Returns(new QueryString(queryString));

    public void SetRequestPath(string path)
    {
        var pathString = new PathString(path);
        HttpRequestMock
            .Setup(m => m.Path)
            .Returns(pathString);
    }

    public void SetHttps(bool isHttps) =>
        HttpRequestMock
            .Setup(m => m.IsHttps)
            .Returns(isHttps);

    public override void Abort() => throw new NotImplementedException();

    public void VerifyAll()
    {
        ConnectionInfoMock.VerifyAll();
        HttpRequestMock.VerifyAll();
        HttpResponseMock.VerifyAll();
    }

    public override IFeatureCollection Features { get; }

    public override HttpRequest Request => HttpRequestMock.Object;

    public override HttpResponse Response => HttpResponseMock.Object;

    public override ConnectionInfo Connection => ConnectionInfoMock.Object;

    public override WebSocketManager WebSockets { get; }

    public override ClaimsPrincipal User { get; set; }

    public sealed override IDictionary<object, object> Items { get; set; }

    public override IServiceProvider RequestServices
    {
        get => ServiceProviderMock.Object;
        set => throw new NotImplementedException();
    }

    public override CancellationToken RequestAborted { get; set; }

    public override string TraceIdentifier { get; set; }

    public sealed override ISession Session { get; set; }
}
