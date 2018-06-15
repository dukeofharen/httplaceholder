using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Placeholder.Implementation.Implementations.ResponseWriters;
using Placeholder.Implementation.Tests.Utilities;
using Placeholder.Models;

namespace Placeholder.Implementation.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class HeadersResponseWriterFacts
   {
      private HeadersResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _writer = new HeadersResponseWriter(TestObjectFactory.GetRequestLoggerFactory());
      }

      [TestMethod]
      public async Task HeadersResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Headers = null
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.AreEqual(0, response.Headers.Count);
      }

      [TestMethod]
      public async Task HeadersResponseWriter_WriteToResponseAsync_HappyFlow()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  { "X-Api-Key", "1223" },
                  { "X-User-Secret", "abc" }
               }
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.AreEqual(2, response.Headers.Count);
         Assert.AreEqual("1223", response.Headers["X-Api-Key"]);
         Assert.AreEqual("abc", response.Headers["X-User-Secret"]);
      }
   }
}
