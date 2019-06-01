using HttPlaceholder.Application.StubExecution;
using Moq;

namespace HttPlaceholder.TestUtilities
{
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
    }
}
