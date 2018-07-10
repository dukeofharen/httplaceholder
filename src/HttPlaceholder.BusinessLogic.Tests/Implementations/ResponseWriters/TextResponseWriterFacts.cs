using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HttPlaceholder.BusinessLogic.Implementations.ResponseWriters;
using HttPlaceholder.BusinessLogic.Tests.Utilities;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class TextResponseWriterFacts
   {
      private TextResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _writer = new TextResponseWriter(TestObjectFactory.GetRequestLoggerFactory());
      }

      [TestMethod]
      public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Text = null
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsNull(response.Body);
      }

      [TestMethod]
      public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow()
      {
         // arrange
         string text = "bla123";
         var expectedBody = Encoding.UTF8.GetBytes(text);
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Text = text
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
         Assert.AreEqual("text/plain", response.Headers["Content-Type"]);
      }

      [TestMethod]
      public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
      {
         // arrange
         string text = "bla123";
         var expectedBody = Encoding.UTF8.GetBytes(text);
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Text = text
            }
         };

         var response = new ResponseModel();
         response.Headers.Add("Content-Type", "text/xml");

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
         Assert.AreEqual("text/xml", response.Headers["Content-Type"]);
      }
   }
}
