using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
}
