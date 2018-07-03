using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HttPlaceholder.Implementation.Implementations.ResponseWriters;
using HttPlaceholder.Implementation.Tests.Utilities;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class Base64ResponseWriterFacts
   {
      private Base64ResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _writer = new Base64ResponseWriter(TestObjectFactory.GetRequestLoggerFactory());
      }

      [TestMethod]
      public async Task Base64ResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Base64 = null
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsNull(response.Body);
      }

      [TestMethod]
      public async Task Base64ResponseWriter_WriteToResponseAsync_HappyFlow()
      {
         // arrange
         var expectedBytes = Encoding.UTF8.GetBytes("TEST!!1!");

         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               Base64 = "VEVTVCEhMSE="
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsTrue(expectedBytes.SequenceEqual(response.Body));
      }
   }
}
