using System.Threading.Tasks;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HttPlaceholder.Implementation.Implementations.ResponseWriters;
using HttPlaceholder.Implementation.Tests.Utilities;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class ExtraDurationResponseWriterFacts
   {
      private Mock<IAsyncService> _asyncServiceMock;
      private ExtraDurationResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _asyncServiceMock = new Mock<IAsyncService>();
         _writer = new ExtraDurationResponseWriter(
            _asyncServiceMock.Object,
            TestObjectFactory.GetRequestLoggerFactory());
      }

      [TestCleanup]
      public void Cleanup()
      {
         _asyncServiceMock.VerifyAll();
      }

      [TestMethod]
      public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               ExtraDuration = null
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         _asyncServiceMock.Verify(m => m.DelayAsync(It.IsAny<int>()), Times.Never);
      }

      [TestMethod]
      public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               ExtraDuration = 10
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         _asyncServiceMock.Verify(m => m.DelayAsync(stub.Response.ExtraDuration.Value), Times.Once);
      }
   }
}
