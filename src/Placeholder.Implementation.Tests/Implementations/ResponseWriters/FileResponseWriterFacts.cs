using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations.ResponseWriters;
using Placeholder.Implementation.Services;
using Placeholder.Implementation.Tests.Utilities;
using Placeholder.Models;

namespace Placeholder.Implementation.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class FileResponseWriterFacts
   {
      private Mock<IFileService> _fileServiceMock;
      private Mock<IStubContainer> _stubContainerMock;
      private FileResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _fileServiceMock = new Mock<IFileService>();
         _stubContainerMock = new Mock<IStubContainer>();
         _writer = new FileResponseWriter(
            _fileServiceMock.Object,
            TestObjectFactory.GetRequestLoggerFactory(),
            _stubContainerMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _fileServiceMock.VerifyAll();
         _stubContainerMock.VerifyAll();
      }

      [TestMethod]
      public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
      {
         // arrange
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               File = null
            }
         };

         var response = new ResponseModel();

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsNull(response.Body);
      }

      [TestMethod]
      public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileFoundDirectly()
      {
         // arrange
         var body = new byte[] { 1, 2, 3 };
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               File = @"C:\tmp\image.png"
            }
         };

         var response = new ResponseModel();

         _fileServiceMock
            .Setup(m => m.FileExists(stub.Response.File))
            .Returns(true);

         _fileServiceMock
            .Setup(m => m.ReadAllBytes(stub.Response.File))
            .Returns(body);

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.AreEqual(body, response.Body);
      }

      [TestMethod]
      public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileNotFoundDirectly_ButFoundInStubFolder()
      {
         // arrange
         string yamlFilePath = @"C:\stubs";
         string file = "image.png";
         string expectedFolder = Path.Combine(yamlFilePath, file);
         var body = new byte[] { 1, 2, 3 };
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               File = file
            }
         };

         var response = new ResponseModel();

         _stubContainerMock
            .Setup(m => m.GetStubFileDirectory())
            .Returns(yamlFilePath);

         _fileServiceMock
            .Setup(m => m.FileExists(stub.Response.File))
            .Returns(false);

         _fileServiceMock
            .Setup(m => m.FileExists(expectedFolder))
            .Returns(true);

         _fileServiceMock
            .Setup(m => m.ReadAllBytes(expectedFolder))
            .Returns(body);

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.AreEqual(body, response.Body);
      }

      [TestMethod]
      public async Task FileResponseWriter_WriteToResponseAsync_FileNotFoundDirectly_AlsoNotFoundInStubFolder_ShouldReturnNoBody()
      {
         // arrange
         string yamlFilePath = @"C:\stubs";
         string file = "image.png";
         string expectedFolder = Path.Combine(yamlFilePath, file);
         var stub = new StubModel
         {
            Response = new StubResponseModel
            {
               File = file
            }
         };

         var response = new ResponseModel();

         _stubContainerMock
            .Setup(m => m.GetStubFileDirectory())
            .Returns(yamlFilePath);

         _fileServiceMock
            .Setup(m => m.FileExists(stub.Response.File))
            .Returns(false);

         _fileServiceMock
            .Setup(m => m.FileExists(expectedFolder))
            .Returns(false);

         // act
         await _writer.WriteToResponseAsync(stub, response);

         // assert
         Assert.IsNull(response.Body);
      }
   }
}
