using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
      private Mock<IServiceProvider> _serviceProviderMock;
      private StubResponseGenerator _generator;

      [TestInitialize]
      public void Initialize()
      {
         _serviceProviderMock = new Mock<IServiceProvider>();
         _generator = new StubResponseGenerator(
            TestObjectFactory.GetRequestLoggerFactory(),
            _serviceProviderMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _serviceProviderMock.VerifyAll();
      }

      [TestMethod]
      public async Task StubResponseGenerator_GenerateResponseAsync_HappyFlow()
      {
         // arrange
         var stub = new StubModel();
         var responseWriterMock1 = new Mock<IResponseWriter>();
         var responseWriterMock2 = new Mock<IResponseWriter>();

         responseWriterMock1
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
            .Callback<StubModel, ResponseModel>((s, r) => r.StatusCode = 401)
            .Returns(Task.CompletedTask);

         responseWriterMock2
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
            .Callback<StubModel, ResponseModel>((s, r) => r.Headers.Add("X-Api-Key", "12345"))
            .Returns(Task.CompletedTask);

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IResponseWriter>)))
            .Returns(new[] { responseWriterMock1.Object, responseWriterMock2.Object });

         // act
         var result = await _generator.GenerateResponseAsync(stub);

         // assert
         Assert.AreEqual(401, result.StatusCode);
         Assert.AreEqual("12345", result.Headers["X-Api-Key"]);
      }
   }
}
