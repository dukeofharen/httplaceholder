using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Middleware;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using HttPlaceholder.TestUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Tests.Middleware
{
    [TestClass]
    public class StubHandlingMiddlewareFacts
    {
        private MockHttpContext _httpContext;
        private bool _nextCalled;
        private RequestDelegate _requestDelegate;
        private Mock<IClientIpResolver> _clientIpResolver;
        private Mock<IConfigurationService> _configurationServiceMock;
        private Mock<IHttpContextService> _httpContextServiceMock;
        private Mock<ILogger<StubHandlingMiddleware>> _loggerMock;
        private Mock<IRequestLoggerFactory> _requestLoggerFactoryMock;
        private Mock<IRequestLogger> _requestLoggerMock;
        private Mock<IStubContainer> _stubContainerMock;
        private Mock<IStubRequestExecutor> _stubRequestExecutorMock;
        private StubHandlingMiddleware _middleware;

        [TestInitialize]
        public void Initialize()
        {
            _httpContext = new MockHttpContext();
            _requestDelegate = async context =>
            {
                await Task.CompletedTask;
                _nextCalled = true;
            };
            _clientIpResolver = new Mock<IClientIpResolver>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            _httpContextServiceMock = new Mock<IHttpContextService>();
            _loggerMock = new Mock<ILogger<StubHandlingMiddleware>>();
            _requestLoggerFactoryMock = new Mock<IRequestLoggerFactory>();
            _requestLoggerMock = new Mock<IRequestLogger>();
            _stubContainerMock = new Mock<IStubContainer>();
            _stubRequestExecutorMock = new Mock<IStubRequestExecutor>();

            _middleware = new StubHandlingMiddleware(
                _requestDelegate,
                _clientIpResolver.Object,
                _configurationServiceMock.Object,
                _httpContextServiceMock.Object,
                _loggerMock.Object,
                _requestLoggerFactoryMock.Object,
                _stubContainerMock.Object,
                _stubRequestExecutorMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _clientIpResolver.VerifyAll();
            _configurationServiceMock.VerifyAll();
            _httpContextServiceMock.VerifyAll();
            _loggerMock.VerifyAll();
            _requestLoggerFactoryMock.VerifyAll();
            _stubContainerMock.VerifyAll();
            _stubRequestExecutorMock.VerifyAll();
        }

        [TestMethod]
        public async Task StubHandlingMiddleware_Invoke_PathContainsSegmentToIgnore_ShouldContinue()
        {
            // arrange
            string path = "/ph-ui";

            _httpContextServiceMock
                .Setup(m => m.Path)
                .Returns(path);

            // act
            await _middleware.Invoke(_httpContext);

            // assert
            Assert.IsTrue(_nextCalled);
        }

        [TestMethod]
        public async Task StubHandlingMiddleware_Invoke_HappyFlow()
        {
            // arrange
            string path = "/stub";
            string method = "POST";
            string displayUrl = "http://localhost:5000/stub";
            string body = "BLA";
            string ip = "127.0.0.1";
            var headers = new Dictionary<string, string>
            {
                { "X-Api-Key", "123" }
            };

            var response = new ResponseModel
            {
                Body = new byte[] { 1, 2, 3 },
                Headers = new Dictionary<string, string>
                {
                    { "Header1", "Value1" },
                    { "Header2", "Value2" }
                },
                StatusCode = 201
            };

            var requestResult = new RequestResultModel();
            var config = new Dictionary<string, string>
            {
                { Constants.ConfigKeys.EnableRequestLogging, "true" }
            };

            _httpContextServiceMock
                .Setup(m => m.Path)
                .Returns(path);
            _requestLoggerFactoryMock
                .Setup(m => m.GetRequestLogger())
                .Returns(_requestLoggerMock.Object);
            _requestLoggerMock
                .Setup(m => m.SetCorrelationId(It.IsAny<string>()));
            _httpContextServiceMock
                .Setup(m => m.EnableRewind());
            _httpContextServiceMock
                .Setup(m => m.Method)
                .Returns(method);
            _httpContextServiceMock
                .Setup(m => m.DisplayUrl)
                .Returns(displayUrl);
            _httpContextServiceMock
                .Setup(m => m.GetBody())
                .Returns(body);
            _clientIpResolver
                .Setup(m => m.GetClientIp())
                .Returns(ip);
            _httpContextServiceMock
                .Setup(m => m.GetHeaders())
                .Returns(headers);
            _requestLoggerMock
                .Setup(m => m.LogRequestParameters(method, displayUrl, body, ip, headers));
            _httpContextServiceMock
                .Setup(m => m.ClearResponse());
            _httpContextServiceMock
                .Setup(m => m.TryAddHeader("X-HttPlaceholder-Correlation", It.IsAny<StringValues>()))
                .Returns(true);
            _stubRequestExecutorMock
                .Setup(m => m.ExecuteRequestAsync())
                .ReturnsAsync(response);
            _httpContextServiceMock
                .Setup(m => m.SetStatusCode(response.StatusCode));
            foreach(var header in response.Headers)
            {
                _httpContextServiceMock
                    .Setup(m => m.AddHeader(header.Key, header.Value));
            }

            _httpContextServiceMock
                .Setup(m => m.WriteAsync(response.Body))
                .Returns(Task.CompletedTask);
            _requestLoggerMock
                .Setup(m => m.GetResult())
                .Returns(requestResult);
            _configurationServiceMock
                .Setup(m => m.GetConfiguration())
                .Returns(config);
            _stubContainerMock
                .Setup(m => m.AddRequestResultAsync(requestResult))
                .Returns(Task.CompletedTask);

            // act
            await _middleware.Invoke(_httpContext);

            // assert
            Assert.IsFalse(_nextCalled);
        }
    }
}
