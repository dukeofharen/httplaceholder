using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HttPlaceholder.DataLogic;
using HttPlaceholder.BusinessLogic.Implementations.ResponseWriters;
using HttPlaceholder.BusinessLogic.Tests.Utilities;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ResponseWriters
{
   [TestClass]
   public class FileResponseWriterFacts
   {
      private Mock<IFileService> _fileServiceMock;
      private Mock<IStubContainer> _stubContainerMock;
      private Mock<IStubRootPathResolver> _stubRootPathResolverMock;
      private FileResponseWriter _writer;

      [TestInitialize]
      public void Initialize()
      {
         _fileServiceMock = new Mock<IFileService>();
         _stubContainerMock = new Mock<IStubContainer>();
         _stubRootPathResolverMock = new Mock<IStubRootPathResolver>();
         _writer = new FileResponseWriter(
            _fileServiceMock.Object,
            TestObjectFactory.GetRequestLoggerFactory(),
            _stubContainerMock.Object,
            _stubRootPathResolverMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _fileServiceMock.VerifyAll();
         _stubContainerMock.VerifyAll();
         _stubRootPathResolverMock.VerifyAll();
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

         _stubRootPathResolverMock
            .Setup(m => m.GetStubRootPath())
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

         _stubRootPathResolverMock
            .Setup(m => m.GetStubRootPath())
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
