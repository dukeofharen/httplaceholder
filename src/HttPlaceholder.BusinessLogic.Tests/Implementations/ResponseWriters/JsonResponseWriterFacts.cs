using HttPlaceholder.BusinessLogic.Implementations.ResponseWriters;
using HttPlaceholder.BusinessLogic.Tests.Utilities;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class JsonResponseWriterFacts
   {
      private JsonResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _writer = new JsonResponseWriter();
      }

      [TestMethod]
      public async Task JsonResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Json = null
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsNull(response.Body);
      }

      [TestMethod]
      public async Task JsonResponseWriter_WriteToResponseAsync_HappyFlow()
      {
         // arrange
         string responseText = "{}";
         var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Json = responseText
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
         Assert.AreEqual("application/json", response.Headers["Content-Type"]);
      }

      [TestMethod]
      public async Task JsonResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
      {
         // arrange
         string responseText = "{}";
         var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Json = responseText
            }
         };

         var response = new ResponseModel();
         response.Headers.Add("Content-Type", "text/plain");

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
         Assert.AreEqual("text/plain", response.Headers["Content-Type"]);
      }
   }
}
