using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Newtonsoft.Json;

namespace HttPlaceholder.TestUtilities;

public static class TestObjectFactory
{
    public static IRequestLoggerFactory GetRequestLoggerFactory()
    {
        var requestLoggerMock = new Mock<IRequestLogger>();
        var requestLoggerFactoryMock = new Mock<IRequestLoggerFactory>();
        requestLoggerFactoryMock
            .Setup(m => m.GetRequestLogger())
            .Returns(requestLoggerMock.Object);
        return requestLoggerFactoryMock.Object;
    }

    public static Dictionary<object, object> Convert(StubConditionStringCheckingModel input)
    {
        var json = JsonConvert.SerializeObject(input);
        return JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
    }

    public static Dictionary<object, object> CreateStringCheckingModel(bool? present = null) =>
        Convert(new StubConditionStringCheckingModel {Present = present});

    public static (IHubContext<T> hubContext, Mock<IClientProxy> clientProxyMock) CreateHubMock<T>(string group = null)
        where T : Hub
    {
        var hubContextMock = new Mock<IHubContext<T>>();
        var hubClientsMock = new Mock<IHubClients>();
        var clientProxyMock = new Mock<IClientProxy>();

        hubContextMock
            .Setup(m => m.Clients)
            .Returns(hubClientsMock.Object);

        if (string.IsNullOrWhiteSpace(group))
        {
            hubClientsMock
                .Setup(m => m.All)
                .Returns(clientProxyMock.Object);
        }
        else
        {
            hubClientsMock
                .Setup(m => m.Group(group))
                .Returns(clientProxyMock.Object);
        }

        return (hubContextMock.Object, clientProxyMock);
    }

    public static IHttpContextAccessor CreateHttpContextAccessor(HttpContext httpContext)
    {
        var mock = new Mock<IHttpContextAccessor>();
        mock
            .Setup(m => m.HttpContext)
            .Returns(httpContext);
        return mock.Object;
    }
}
