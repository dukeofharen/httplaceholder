using HttPlaceholder.Services;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Utilities
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
