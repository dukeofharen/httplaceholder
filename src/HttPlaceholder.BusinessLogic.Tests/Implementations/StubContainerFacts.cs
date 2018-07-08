using HttPlaceholder.BusinessLogic.Implementations;
using HttPlaceholder.DataLogic;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations
{
   [TestClass]
   public class StubContainerFacts
   {
      private Mock<IServiceProvider> _serviceProviderMock;
      private StubContainer _container;

      [TestInitialize]
      public void Initialize()
      {
         _serviceProviderMock = new Mock<IServiceProvider>();
         _container = new StubContainer(_serviceProviderMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _serviceProviderMock.VerifyAll();
      }

      [TestMethod]
      public async Task StubContainer_GetStubsAsync_HappyFlow()
      {
         // arrange
         var stubSource1 = new Mock<IStubSource>();
         var stubSource2 = new Mock<IStubSource>();

         var stub1 = new StubModel();
         var stub2 = new StubModel();
         var stub3 = new StubModel();

         stubSource1
            .Setup(m => m.GetStubsAsync())
            .ReturnsAsync(new[] { stub1, stub2 });

         stubSource2
            .Setup(m => m.GetStubsAsync())
            .ReturnsAsync(new[] { stub3 });

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource1.Object, stubSource2.Object });

         // act
         var result = (await _container.GetStubsAsync()).ToArray();

         // assert
         Assert.AreEqual(3, result.Length);
         Assert.AreEqual(stub1, result[0]);
         Assert.AreEqual(stub2, result[1]);
         Assert.AreEqual(stub3, result[2]);
      }

      [TestMethod]
      public async Task StubContainer_AddStubAsync_HappyFlow()
      {
         // arrange
         var stub = new StubModel();
         var stubSource = new Mock<IWritableStubSource>();
         stubSource
            .Setup(m => m.AddStubAsync(stub))
            .Returns(Task.CompletedTask);

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource.Object });

         // act
         await _container.AddStubAsync(stub);

         // assert
         stubSource.Verify(m => m.AddStubAsync(stub), Times.Once);
      }

      [TestMethod]
      public async Task StubContainer_DeleteStubAsync_HappyFlow()
      {
         // arrange
         var stubId = "stubId1";
         var stubSource = new Mock<IWritableStubSource>();
         stubSource
            .Setup(m => m.DeleteStubAsync(stubId))
            .ReturnsAsync(true);

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource.Object });

         // act
         bool result = await _container.DeleteStubAsync(stubId);

         // assert
         Assert.IsTrue(result);
      }

      [TestMethod]
      public async Task StubContainer_GetStubAsync_HappyFlow()
      {
         // arrange
         var stubSource1 = new Mock<IStubSource>();
         var stubSource2 = new Mock<IStubSource>();

         var stub1 = new StubModel { Id = "stub1" };
         var stub2 = new StubModel { Id = "stub2" };
         var stub3 = new StubModel { Id = "stub3" };

         stubSource1
            .Setup(m => m.GetStubsAsync())
            .ReturnsAsync(new[] { stub1, stub2 });

         stubSource2
            .Setup(m => m.GetStubsAsync())
            .ReturnsAsync(new[] { stub3 });

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource1.Object, stubSource2.Object });

         // act
         var result = await _container.GetStubAsync("stub2");

         // assert
         Assert.AreEqual(stub2, result);
      }

      [TestMethod]
      public async Task StubContainer_AddRequestResultAsync_HappyFlow()
      {
         // arrange
         var request = new RequestResultModel();
         var stubSource = new Mock<IWritableStubSource>();
         stubSource
            .Setup(m => m.AddRequestResultAsync(request))
            .Returns(Task.CompletedTask);

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource.Object });

         // act
         await _container.AddRequestResultAsync(request);

         // assert
         stubSource.Verify(m => m.AddRequestResultAsync(request), Times.Once);
      }

      [TestMethod]
      public async Task StubContainer_GetRequestResultsAsync_HappyFlow()
      {
         // arrange
         var requests = new[] { new RequestResultModel(), new RequestResultModel() };
         var stubSource = new Mock<IWritableStubSource>();
         stubSource
            .Setup(m => m.GetRequestResultsAsync())
            .ReturnsAsync(requests);

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource.Object });

         // act
         var result = await _container.GetRequestResultsAsync();

         // assert
         Assert.AreEqual(requests, result);
      }

      [TestMethod]
      public async Task StubContainer_GetRequestResultsByStubIdAsync_HappyFlow()
      {
         // arrange
         var requests = new[] {
            new RequestResultModel { ExecutingStubId = "stub1" },
            new RequestResultModel { ExecutingStubId = "stub2" },
            new RequestResultModel { ExecutingStubId = "stub1" }
         };
         var stubSource = new Mock<IWritableStubSource>();
         stubSource
            .Setup(m => m.GetRequestResultsAsync())
            .ReturnsAsync(requests);

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource.Object });

         // act
         var result = (await _container.GetRequestResultsByStubIdAsync("stub1")).ToArray();

         // assert
         Assert.AreEqual(2, result.Length);
         Assert.AreEqual(requests[0], result[0]);
         Assert.AreEqual(requests[2], result[1]);
      }

      [TestMethod]
      public async Task StubContainer_DeleteAllRequestResultsAsync_HappyFlow()
      {
         // arrange
         var stubSource = new Mock<IWritableStubSource>();
         stubSource
            .Setup(m => m.DeleteAllRequestResultsAsync())
            .Returns(Task.CompletedTask);

         _serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IStubSource>)))
            .Returns(new[] { stubSource.Object });

         // act
         await _container.DeleteAllRequestResultsAsync();

         // assert
         stubSource.Verify(m => m.DeleteAllRequestResultsAsync(), Times.Once);
      }
   }
}
