using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations;
using Placeholder.Implementation.Services;
using Placeholder.Implementation.Tests.Utilities;
using Placeholder.Models;

namespace Placeholder.Implementation.Tests.Implementations
{
   [TestClass]
   public class StubResponseGeneratorFacts
   {
      private Mock<IFileService> _fileServiceMock;
      private Mock<IStubContainer> _stubContainerMock;
      private StubResponseGenerator _generator;

      [TestInitialize]
      public void Initialize()
      {
         _fileServiceMock = new Mock<IFileService>();
         _stubContainerMock = new Mock<IStubContainer>();
         _generator = new StubResponseGenerator(
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
      public void StubResponseGenerator_GenerateResponse_HappyFlow()
      {
         // arrange
         var stub = new StubModel
         {
            Id = "123",
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123"}
               },
               StatusCode = 201,
               Text = "321"
            }
         };

         // act
         var response = _generator.GenerateResponse(stub);

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual("321", Encoding.UTF8.GetString(response.Body));
         Assert.AreEqual("123", response.Headers["X-Api-Key"]);
         Assert.AreEqual(201, response.StatusCode);
      }

      [TestMethod]
      public void StubResponseGenerator_GenerateResponse_HappyFlow_Base64()
      {
         // arrange
         var stub = new StubModel
         {
            Id = "123",
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123"}
               },
               StatusCode = 201,
               Base64 = "VGhpcyBpcyB0aGUgY29udGVudCE="
            }
         };

         // act
         var response = _generator.GenerateResponse(stub);

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual("This is the content!", Encoding.UTF8.GetString(response.Body));
         Assert.AreEqual("123", response.Headers["X-Api-Key"]);
         Assert.AreEqual(201, response.StatusCode);
      }

      [TestMethod]
      public void StubResponseGenerator_GenerateResponse_HappyFlow_File_FoundInFirstGo()
      {
         // arrange
         string expectedFileContents = "File contents yo!";
         string filePath = @"C:\files\file.txt";
         var stub = new StubModel
         {
            Id = "123",
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123"}
               },
               StatusCode = 201,
               File = filePath
            }
         };

         _fileServiceMock
            .Setup(m => m.FileExists(filePath))
            .Returns(true);
         _fileServiceMock
            .Setup(m => m.ReadAllBytes(filePath))
            .Returns(Encoding.UTF8.GetBytes(expectedFileContents));

         // act
         var response = _generator.GenerateResponse(stub);

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual(expectedFileContents, Encoding.UTF8.GetString(response.Body));
         Assert.AreEqual("123", response.Headers["X-Api-Key"]);
         Assert.AreEqual(201, response.StatusCode);
      }

      [TestMethod]
      public void StubResponseGenerator_GenerateResponse_HappyFlow_File_FoundInYamlFileFolder()
      {
         // arrange
         string expectedFileContents = "File contents yo!";
         string yamlFileFolder = @"C:\stubs";
         string filePath = "file.txt";
         string expectedFullFilePath = @"C:\stubs\file.txt";
         var stub = new StubModel
         {
            Id = "123",
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123"}
               },
               StatusCode = 201,
               File = filePath
            }
         };

         _fileServiceMock
            .Setup(m => m.FileExists(expectedFullFilePath))
            .Returns(true);
         _fileServiceMock
            .Setup(m => m.ReadAllBytes(expectedFullFilePath))
            .Returns(Encoding.UTF8.GetBytes(expectedFileContents));

         _stubContainerMock
            .Setup(m => m.GetStubFileDirectory())
            .Returns(yamlFileFolder);

         // act
         var response = _generator.GenerateResponse(stub);

         // assert
         Assert.IsNotNull(response);
         Assert.AreEqual(expectedFileContents, Encoding.UTF8.GetString(response.Body));
         Assert.AreEqual("123", response.Headers["X-Api-Key"]);
         Assert.AreEqual(201, response.StatusCode);
      }

      [TestMethod]
      public void StubResponseGenerator_GenerateResponse_HappyFlow_File_NotFound_ShouldReturnNoBody()
      {
         // arrange
         string yamlFileFolder = @"C:\stubs";
         string filePath = "file.txt";
         string expectedFullFilePath = @"C:\stubs\file.txt";
         var stub = new StubModel
         {
            Id = "123",
            Response = new StubResponseModel
            {
               Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123"}
               },
               StatusCode = 201,
               File = filePath
            }
         };

         _fileServiceMock
            .Setup(m => m.FileExists(expectedFullFilePath))
            .Returns(false);

         _stubContainerMock
            .Setup(m => m.GetStubFileDirectory())
            .Returns(yamlFileFolder);

         // act
         var response = _generator.GenerateResponse(stub);

         // assert
         Assert.IsNotNull(response);
         Assert.IsNull(response.Body);
         Assert.AreEqual("123", response.Headers["X-Api-Key"]);
         Assert.AreEqual(201, response.StatusCode);
      }
   }
}
