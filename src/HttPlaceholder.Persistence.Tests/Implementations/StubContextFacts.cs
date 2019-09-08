using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Persistence.Tests.Implementations
{
    [TestClass]
    public class StubContextFacts
    {
        private IList<IStubSource> _stubSources = new List<IStubSource>();
        private StubContext _context;

        [TestInitialize]
        public void Initialize()
        {
            _context = new StubContext(_stubSources);
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

            _stubSources.Add(stubSource1.Object);
            _stubSources.Add(stubSource2.Object);

            // act
            var result = (await _context.GetStubsAsync()).ToArray();

            // assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(stub1, result[0].Stub);
            Assert.AreEqual(stub2, result[1].Stub);
            Assert.AreEqual(stub3, result[2].Stub);
        }

        [TestMethod]
        public async Task StubContainer_GetStubsAsync_ByTenant_HappyFlow()
        {
            // arrange
            var stubSource1 = new Mock<IStubSource>();
            var stubSource2 = new Mock<IStubSource>();

            var stub1 = new StubModel
            {
                Tenant = "tenant1"
            };
            var stub2 = new StubModel
            {
                Tenant = "tenant2"
            };
            var stub3 = new StubModel
            {
                Tenant = "TENaNT1"
            };

            stubSource1
               .Setup(m => m.GetStubsAsync())
               .ReturnsAsync(new[] { stub1, stub2 });

            stubSource2
               .Setup(m => m.GetStubsAsync())
               .ReturnsAsync(new[] { stub3 });

            _stubSources.Add(stubSource1.Object);
            _stubSources.Add(stubSource2.Object);

            // act
            var result = (await _context.GetStubsAsync("tenant1")).ToArray();

            // assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(stub1, result[0].Stub);
            Assert.AreEqual(stub3, result[1].Stub);
        }

        [TestMethod]
        public async Task StubContainer_AddStubAsync_StubIdAlreadyAddedToReadOnlyStubSource_ShouldThrowConflictException()
        {
            // arrange
            var stubToBeAdded = new StubModel
            {
                Id = "conflicted"
            };
            var stub = new StubModel
            {
                Id = "COnflicted"
            };
            var writableStubSource = new Mock<IWritableStubSource>();
            var readOnlyStubSource = new Mock<IStubSource>();
            readOnlyStubSource
               .Setup(m => m.GetStubsAsync())
               .ReturnsAsync(new[]
               {
               stub
               });

            _stubSources.Add(writableStubSource.Object);
            _stubSources.Add(readOnlyStubSource.Object);

            // act / assert
            await Assert.ThrowsExceptionAsync<ConflictException>(() => _context.AddStubAsync(stubToBeAdded));
        }

        [TestMethod]
        public async Task StubContainer_AddStubAsync_NoIdSet_ShouldAssignRandomString()
        {
            // arrange
            var stubToBeAdded = new StubModel
            {
                Conditions = new StubConditionsModel
                {
                    Body = new string[] { "test" }
                }
            };
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
               .Setup(m => m.AddStubAsync(stubToBeAdded))
               .Returns(Task.CompletedTask);

            var stub = new StubModel
            {
                Id = "existing-stub"
            };
            var readOnlyStubSource = new Mock<IStubSource>();
            readOnlyStubSource
               .Setup(m => m.GetStubsAsync())
               .ReturnsAsync(new[]
               {
               stub
               });

            _stubSources.Add(stubSource.Object);
            _stubSources.Add(readOnlyStubSource.Object);

            // act
            await _context.AddStubAsync(stubToBeAdded);

            // assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(stubToBeAdded.Id));
        }

        [TestMethod]
        public async Task StubContainer_AddStubAsync_HappyFlow()
        {
            // arrange
            var stubToBeAdded = new StubModel
            {
                Id = "new-stub-02"
            };
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
               .Setup(m => m.AddStubAsync(stubToBeAdded))
               .Returns(Task.CompletedTask);

            var stub = new StubModel
            {
                Id = "new-stub-01"
            };
            var readOnlyStubSource = new Mock<IStubSource>();
            readOnlyStubSource
               .Setup(m => m.GetStubsAsync())
               .ReturnsAsync(new[]
               {
               stub
               });

            _stubSources.Add(stubSource.Object);
            _stubSources.Add(readOnlyStubSource.Object);

            // act
            await _context.AddStubAsync(stubToBeAdded);

            // assert
            stubSource.Verify(m => m.AddStubAsync(stubToBeAdded), Times.Once);
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

            _stubSources.Add(stubSource.Object);

            // act
            bool result = await _context.DeleteStubAsync(stubId);

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

            _stubSources.Add(stubSource1.Object);
            _stubSources.Add(stubSource2.Object);

            // act
            var result = await _context.GetStubAsync("stub2");

            // assert
            Assert.AreEqual(stub2, result.Stub);
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

            _stubSources.Add(stubSource.Object);

            // act
            await _context.AddRequestResultAsync(request);

            // assert
            stubSource.Verify(m => m.AddRequestResultAsync(request), Times.Once);
            stubSource.Verify(m => m.CleanOldRequestResultsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task StubContainer_GetRequestResultsAsync_HappyFlow()
        {
            // arrange
            var request1 = new RequestResultModel
            {
                RequestBeginTime = DateTime.Now.AddSeconds(-2)
            };
            var request2 = new RequestResultModel
            {
                RequestBeginTime = DateTime.Now.AddSeconds(-1)
            };
            var requests = new[] { request1, request2 };
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
               .Setup(m => m.GetRequestResultsAsync())
               .ReturnsAsync(requests);

            _stubSources.Add(stubSource.Object);

            // act
            var result = (await _context.GetRequestResultsAsync()).ToArray();

            // assert
            Assert.AreEqual(request2, result[0]);
            Assert.AreEqual(request1, result[1]);
        }

        [TestMethod]
        public async Task StubContainer_GetRequestResultsByStubIdAsync_HappyFlow()
        {
            // arrange
            var request1 = new RequestResultModel
            {
                ExecutingStubId = "stub1",
                RequestBeginTime = DateTime.Now.AddSeconds(-2)
            };
            var request2 = new RequestResultModel
            {
                ExecutingStubId = "stub2",
                RequestBeginTime = DateTime.Now.AddSeconds(-2)
            };
            var request3 = new RequestResultModel
            {
                ExecutingStubId = "stub1",
                RequestBeginTime = DateTime.Now.AddSeconds(-1)
            };
            var requests = new[] {
            request1,
            request2,
            request3
         };
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
               .Setup(m => m.GetRequestResultsAsync())
               .ReturnsAsync(requests);

            _stubSources.Add(stubSource.Object);

            // act
            var result = (await _context.GetRequestResultsByStubIdAsync("stub1")).ToArray();

            // assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(requests[0], result[1]);
            Assert.AreEqual(requests[2], result[0]);
        }

        [TestMethod]
        public async Task StubContainer_DeleteAllRequestResultsAsync_HappyFlow()
        {
            // arrange
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
               .Setup(m => m.DeleteAllRequestResultsAsync())
               .Returns(Task.CompletedTask);

            _stubSources.Add(stubSource.Object);

            // act
            await _context.DeleteAllRequestResultsAsync();

            // assert
            stubSource.Verify(m => m.DeleteAllRequestResultsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task StubContainer_DeleteAllStubsAsync_Tenant_HappyFlow()
        {
            // arrange
            var tenant = "tenant1";
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel
            {
                Id = "stub1",
                Tenant = tenant
            };
            var stub2 = new StubModel
            {
                Id = "stub2",
                Tenant = $"{tenant}bla"
            };
            var stub3 = new StubModel
            {
                Id = "stub3",
                Tenant = tenant.ToUpper()
            };

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[]
                {
                    stub1,
                    stub2,
                    stub3
                });

            _stubSources.Add(stubSource.Object);

            // act
            await _context.DeleteAllStubsAsync(tenant);

            // assert
            stubSource.Verify(m => m.DeleteStubAsync(stub1.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub2.Id), Times.Never);
            stubSource.Verify(m => m.DeleteStubAsync(stub3.Id), Times.Once);
        }

        [TestMethod]
        public async Task StubContainer_DeleteAllStubsAsync_HappyFlow()
        {
            // arrange
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel
            {
                Id = "stub1",
                Tenant = "tenant1"
            };
            var stub2 = new StubModel
            {
                Id = "stub2",
                Tenant = "tenant2"
            };
            var stub3 = new StubModel
            {
                Id = "stub3",
                Tenant = "tenant1"
            };

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[]
                {
                    stub1,
                    stub2,
                    stub3
                });

            _stubSources.Add(stubSource.Object);

            // act
            await _context.DeleteAllStubsAsync();

            // assert
            stubSource.Verify(m => m.DeleteStubAsync(stub1.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub2.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub3.Id), Times.Once);
        }

        [TestMethod]
        public async Task StubContainer_UpdateAllStubsAsync_HappyFlow()
        {
            // arrange
            var tenant1 = "tenant1";
            var tenant2 = "tenant2";
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel
            {
                Id = "stub1",
                Tenant = tenant1
            };
            var stub2 = new StubModel
            {
                Id = "stub2",
                Tenant = tenant2
            };
            var stub3 = new StubModel
            {
                Id = "stub3",
                Tenant = tenant1.ToUpper()
            };

            var newStubs = new[]
            {
                new StubModel
                {
                    Id = stub2.Id
                },
                new StubModel
                {
                    Id = stub3.Id
                }
            };

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[]
                {
                    stub1,
                    stub2,
                    stub3
                });

            _stubSources.Add(stubSource.Object);

            // act
            await _context.UpdateAllStubs(tenant1, newStubs);

            // assert
            stubSource.Verify(m => m.DeleteStubAsync(stub1.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub2.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub3.Id), Times.Once);

            stubSource.Verify(m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub1.Id)), Times.Never);
            stubSource.Verify(m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub2.Id)), Times.Once);
            stubSource.Verify(m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub3.Id)), Times.Once);

            Assert.IsTrue(newStubs.All(s => s.Tenant == tenant1));
        }

        [TestMethod]
        public async Task StubContainer_GetTenantNamesAsync_HappyFlow()
        {
            // arrange
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel
            {
                Id = "stub1",
                Tenant = "tenant-1"
            };
            var stub2 = new StubModel
            {
                Id = "stub2",
                Tenant = "tenant-2"
            };
            var stub3 = new StubModel
            {
                Id = "stub3",
                Tenant = "tenant-2"
            };
            var stub4 = new StubModel
            {
                Id = "stub4",
                Tenant = null
            };
            var stub5 = new StubModel
            {
                Id = "stub5",
                Tenant = string.Empty
            };

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[]
                {
                    stub1,
                    stub2,
                    stub3,
                    stub4,
                    stub5
                });

            _stubSources.Add(stubSource.Object);

            // act
            var result = await _context.GetTenantNamesAsync();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("tenant-1", result.ElementAt(0));
            Assert.AreEqual("tenant-2", result.ElementAt(1));
        }

        [TestMethod]
        public async Task StubContainer_PrepareAsync_HappyFlow()
        {
            // arrange
            var stubSource1 = new Mock<IStubSource>();
            var stubSource2 = new Mock<IStubSource>();

            _stubSources.Add(stubSource1.Object);
            _stubSources.Add(stubSource2.Object);

            // act
            await _context.PrepareAsync();

            // assert
            stubSource1.Verify(m => m.PrepareStubSourceAsync(), Times.Once);
            stubSource2.Verify(m => m.PrepareStubSourceAsync(), Times.Once);
        }
    }
}
