using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HttPlaceholder.Implementation.Implementations.ResponseWriters;
using HttPlaceholder.Implementation.Tests.Utilities;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class StatusCodeResponseWriterFacts
   {
      private StatusCodeResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _writer = new StatusCodeResponseWriter(TestObjectFactory.GetRequestLoggerFactory());
      }

      [TestMethod]
      public async Task StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               StatusCode = null
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.AreEqual(200, response.StatusCode);
      }

      [TestMethod]
      public async Task StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               StatusCode = 409
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.AreEqual(409, response.StatusCode);
      }
   }
}
