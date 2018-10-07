using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using Moq;

namespace HttPlaceholder.TestUtilities
{
    public sealed class MockHttpContext : HttpContext
    {
        private readonly Mock<ConnectionInfo> _connectionInfoMock;
        private readonly Mock<HttpResponse> _httpResponseMock;
        private readonly Mock<HttpRequest> _httpRequestMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;

        private int _statusCode;

        public MockHttpContext()
        {
            _connectionInfoMock = new Mock<ConnectionInfo>();
            _httpResponseMock = new Mock<HttpResponse>();
            _httpRequestMock = new Mock<HttpRequest>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            Items = new Dictionary<object, object>();
            Session = new MockSession();

            _httpResponseMock
               .SetupSet(m => m.StatusCode = It.IsAny<int>())
               .Callback<int>(code => _statusCode = code);

            _serviceProviderMock
               .Setup(m => m.GetService(typeof(ITempDataDictionaryFactory)))
               .Returns(new MockTempDataDictionaryFactory());

            _serviceProviderMock
               .Setup(m => m.GetService(typeof(IUrlHelperFactory)))
               .Returns(new Mock<IUrlHelperFactory>().Object);

            _httpRequestMock
               .Setup(m => m.Headers)
               .Returns(new MockHeaderDictionary());

            _httpResponseMock
               .Setup(m => m.Headers)
               .Returns(new TestUtilities.MockHeaderDictionary());
        }

        public int GetStatusCode() => _statusCode;

        public void SetIp(byte[] ip)
        {
            _connectionInfoMock
               .Setup(m => m.RemoteIpAddress)
               .Returns(new IPAddress(ip));
        }

        public void SetIp(string ip)
        {
            _connectionInfoMock
               .Setup(m => m.RemoteIpAddress)
               .Returns(IPAddress.Parse(ip));
        }

        public void SetHost(string host)
        {
            _httpRequestMock
               .Setup(m => m.Host)
               .Returns(new HostString(host));
        }

        public void SetHttps(bool isHttps)
        {
            _httpRequestMock
               .Setup(m => m.IsHttps)
               .Returns(isHttps);
        }

        public void InitializeUserWithId(long id)
        {
            InitializeUserWithId(id.ToString());
        }

        public void InitializeUserWithId(string id)
        {
            User = new ClaimsPrincipal(new[]
            {
            new ClaimsIdentity(new[]
            {
               new Claim(ClaimTypes.NameIdentifier, id)
            })
         });
        }

        public void SetRequestMethod(string method)
        {
            _httpRequestMock
               .Setup(m => m.Method)
               .Returns(method);
        }

        public void SetRequestPath(string path)
        {
            var pathString = new PathString(path);
            _httpRequestMock
               .Setup(m => m.Path)
               .Returns(pathString);
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override IFeatureCollection Features { get; }

        public override HttpRequest Request => _httpRequestMock.Object;

        public override HttpResponse Response => _httpResponseMock.Object;

        public override ConnectionInfo Connection => _connectionInfoMock.Object;

        public override WebSocketManager WebSockets { get; }

        public override ClaimsPrincipal User { get; set; }

        public override IDictionary<object, object> Items { get; set; }

        public override IServiceProvider RequestServices
        {
            get => _serviceProviderMock.Object;
            set => throw new NotImplementedException();
        }

        public override CancellationToken RequestAborted { get; set; }

        public override string TraceIdentifier { get; set; }

        public override ISession Session { get; set; }

        [Obsolete("This is obsolete and will be removed in a future version. The recommended alternative is to use Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions. See https://go.microsoft.com/fwlink/?linkid=845470.")]
        public override Microsoft.AspNetCore.Http.Authentication.AuthenticationManager Authentication => throw new NotImplementedException();

        private class MockHeaderDictionary : Dictionary<string, StringValues>, IHeaderDictionary
        {
            public long? ContentLength { get; set; }
        }
    }
}