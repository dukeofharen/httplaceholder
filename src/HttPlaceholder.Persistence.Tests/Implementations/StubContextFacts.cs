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
        private readonly IList<IStubSource> _stubSources = new List<IStubSource>();
        private StubContext _context;

        [TestInitialize]
        public void Initialize() => _context = new StubContext(_stubSources);

        [TestMethod]
        public async Task GetStubsAsync_HappyFlow()
        {
            // arrange
            var stubSource1 = new Mock<IStubSource>();
            var stubSource2 = new Mock<IStubSource>();

            var stub1 = new StubModel();
            var stub2 = new StubModel();
            var stub3 = new StubModel();

            stubSource1
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub1, stub2});

            stubSource2
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub3});

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
        public async Task GetStubsOverviewAsync_HappyFlow()
        {
            // arrange
            var stubSource1 = new Mock<IStubSource>();
            var stubSource2 = new Mock<IStubSource>();

            var stub1 = new StubOverviewModel();
            var stub2 = new StubOverviewModel();
            var stub3 = new StubOverviewModel();

            stubSource1
                .Setup(m => m.GetStubsOverviewAsync())
                .ReturnsAsync(new[] {stub1, stub2});

            stubSource2
                .Setup(m => m.GetStubsOverviewAsync())
                .ReturnsAsync(new[] {stub3});

            _stubSources.Add(stubSource1.Object);
            _stubSources.Add(stubSource2.Object);

            // act
            var result = (await _context.GetStubsOverviewAsync()).ToArray();

            // assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(stub1, result[0].Stub);
            Assert.AreEqual(stub2, result[1].Stub);
            Assert.AreEqual(stub3, result[2].Stub);
        }

        [TestMethod]
        public async Task GetStubsAsync_ByTenant_HappyFlow()
        {
            // arrange
            var stubSource1 = new Mock<IStubSource>();
            var stubSource2 = new Mock<IStubSource>();

            var stub1 = new StubModel {Tenant = "tenant1"};
            var stub2 = new StubModel {Tenant = "tenant2"};
            var stub3 = new StubModel {Tenant = "TENaNT1"};

            stubSource1
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub1, stub2});

            stubSource2
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub3});

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
        public async Task
            AddStubAsync_StubIdAlreadyAddedToReadOnlyStubSource_ShouldThrowConflictException()
        {
            // arrange
            var stubToBeAdded = new StubModel {Id = "conflicted"};
            var stub = new StubModel {Id = "COnflicted"};
            var writableStubSource = new Mock<IWritableStubSource>();
            var readOnlyStubSource = new Mock<IStubSource>();
            readOnlyStubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub});

            _stubSources.Add(writableStubSource.Object);
            _stubSources.Add(readOnlyStubSource.Object);

            // act / assert
            await Assert.ThrowsExceptionAsync<ConflictException>(() => _context.AddStubAsync(stubToBeAdded));
        }

        [TestMethod]
        public async Task AddStubAsync_NoIdSet_ShouldAssignHashedStubAsId()
        {
            // arrange
            var stubToBeAdded = new StubModel {Conditions = new StubConditionsModel {Body = new[] {"test"}}};
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
                .Setup(m => m.AddStubAsync(stubToBeAdded))
                .Returns(Task.CompletedTask);

            var stub = new StubModel {Id = "existing-stub"};
            var readOnlyStubSource = new Mock<IStubSource>();
            readOnlyStubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub});

            _stubSources.Add(stubSource.Object);
            _stubSources.Add(readOnlyStubSource.Object);

            // act
            await _context.AddStubAsync(stubToBeAdded);

            // assert
            Assert.AreEqual("stub-d7c4d5ae267648eb6fdab7427d1030ac", stubToBeAdded.Id);
        }

        [TestMethod]
        public async Task AddStubAsync_HappyFlow()
        {
            // arrange
            var stubToBeAdded = new StubModel {Id = "new-stub-02"};
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
                .Setup(m => m.AddStubAsync(stubToBeAdded))
                .Returns(Task.CompletedTask);

            var stub = new StubModel {Id = "new-stub-01"};
            var readOnlyStubSource = new Mock<IStubSource>();
            readOnlyStubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub});

            _stubSources.Add(stubSource.Object);
            _stubSources.Add(readOnlyStubSource.Object);

            // act
            await _context.AddStubAsync(stubToBeAdded);

            // assert
            stubSource.Verify(m => m.AddStubAsync(stubToBeAdded), Times.Once);
        }

        [TestMethod]
        public async Task DeleteStubAsync_HappyFlow()
        {
            // arrange
            const string stubId = "stubId1";
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
                .Setup(m => m.DeleteStubAsync(stubId))
                .ReturnsAsync(true);

            _stubSources.Add(stubSource.Object);

            // act
            var result = await _context.DeleteStubAsync(stubId);

            // assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetStubAsync_HappyFlow()
        {
            // arrange
            var stubSource1 = new Mock<IStubSource>();
            var stubSource2 = new Mock<IStubSource>();

            var stub1 = new StubModel {Id = "stub1"};
            var stub2 = new StubModel {Id = "stub2"};

            stubSource1
                .Setup(m => m.GetStubAsync(stub2.Id))
                .ReturnsAsync(stub2);
            stubSource2
                .Setup(m => m.GetStubAsync(stub1.Id))
                .ReturnsAsync(stub1);

            _stubSources.Add(stubSource1.Object);
            _stubSources.Add(stubSource2.Object);

            // act
            var result = await _context.GetStubAsync("stub2");

            // assert
            Assert.AreEqual(stub2, result.Stub);
        }

        [TestMethod]
        public async Task AddRequestResultAsync_HappyFlow()
        {
            // arrange
            var stubSource = new Mock<IWritableStubSource>();

            var stub = new StubModel {Id = "stub1", Tenant = "tenant1"};
            stubSource
                .Setup(m => m.GetStubAsync(stub.Id))
                .ReturnsAsync(stub);

            var request = new RequestResultModel {ExecutingStubId = stub.Id};
            stubSource
                .Setup(m => m.AddRequestResultAsync(request))
                .Returns(Task.CompletedTask);

            _stubSources.Add(stubSource.Object);

            // act
            await _context.AddRequestResultAsync(request);

            // assert
            stubSource.Verify(m => m.AddRequestResultAsync(request), Times.Once);
            stubSource.Verify(m => m.CleanOldRequestResultsAsync(), Times.Once);

            Assert.AreEqual(stub.Tenant, request.StubTenant);
        }

        [TestMethod]
        public async Task GetRequestResultsAsync_HappyFlow()
        {
            // arrange
            var request1 = new RequestResultModel {RequestBeginTime = DateTime.Now.AddSeconds(-2)};
            var request2 = new RequestResultModel {RequestBeginTime = DateTime.Now.AddSeconds(-1)};
            var requests = new[] {request1, request2};
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
        public async Task GetRequestResultsOverviewAsync_HappyFlow()
        {
            // arrange
            var request1 = new RequestOverviewModel {RequestEndTime = DateTime.Now.AddSeconds(-2)};
            var request2 = new RequestOverviewModel {RequestEndTime = DateTime.Now.AddSeconds(-1)};
            var requests = new[] {request1, request2};
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
                .Setup(m => m.GetRequestResultsOverviewAsync())
                .ReturnsAsync(requests);

            _stubSources.Add(stubSource.Object);

            // act
            var result = (await _context.GetRequestResultsOverviewAsync()).ToArray();

            // assert
            Assert.AreEqual(request2, result[0]);
            Assert.AreEqual(request1, result[1]);
        }

        [TestMethod]
        public async Task GetRequestResultAsync_HappyFlow()
        {
            // arrange
            var correlationId = Guid.NewGuid().ToString();
            var request = new RequestResultModel {CorrelationId = correlationId};
            var stubSource = new Mock<IWritableStubSource>();
            stubSource
                .Setup(m => m.GetRequestAsync(correlationId))
                .ReturnsAsync(request);

            _stubSources.Add(stubSource.Object);

            // act
            var result = await _context.GetRequestResultAsync(correlationId);

            // assert
            Assert.AreEqual(request, result);
        }

        [TestMethod]
        public async Task GetRequestResultsByStubIdAsync_HappyFlow()
        {
            // arrange
            var request1 = new RequestResultModel
            {
                ExecutingStubId = "stub1", RequestBeginTime = DateTime.Now.AddSeconds(-2)
            };
            var request2 = new RequestResultModel
            {
                ExecutingStubId = "stub2", RequestBeginTime = DateTime.Now.AddSeconds(-2)
            };
            var request3 = new RequestResultModel
            {
                ExecutingStubId = "stub1", RequestBeginTime = DateTime.Now.AddSeconds(-1)
            };
            var requests = new[] {request1, request2, request3};
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
        public async Task DeleteAllRequestResultsAsync_HappyFlow()
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
        public async Task DeleteAllStubsAsync_Tenant_HappyFlow()
        {
            // arrange
            const string tenant = "tenant1";
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel {Id = "stub1", Tenant = tenant};
            var stub2 = new StubModel {Id = "stub2", Tenant = $"{tenant}bla"};
            var stub3 = new StubModel {Id = "stub3", Tenant = tenant.ToUpper()};

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub1, stub2, stub3});

            _stubSources.Add(stubSource.Object);

            // act
            await _context.DeleteAllStubsAsync(tenant);

            // assert
            stubSource.Verify(m => m.DeleteStubAsync(stub1.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub2.Id), Times.Never);
            stubSource.Verify(m => m.DeleteStubAsync(stub3.Id), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAllStubsAsync_HappyFlow()
        {
            // arrange
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel {Id = "stub1", Tenant = "tenant1"};
            var stub2 = new StubModel {Id = "stub2", Tenant = "tenant2"};
            var stub3 = new StubModel {Id = "stub3", Tenant = "tenant1"};

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub1, stub2, stub3});

            _stubSources.Add(stubSource.Object);

            // act
            await _context.DeleteAllStubsAsync();

            // assert
            stubSource.Verify(m => m.DeleteStubAsync(stub1.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub2.Id), Times.Once);
            stubSource.Verify(m => m.DeleteStubAsync(stub3.Id), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAllStubsAsync_HappyFlow()
        {
            // arrange
            const string tenant1 = "tenant1";
            const string tenant2 = "tenant2";
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel {Id = "stub1", Tenant = tenant1};
            var stub2 = new StubModel {Id = "stub2", Tenant = tenant2};
            var stub3 = new StubModel {Id = "stub3", Tenant = tenant1.ToUpper()};

            var newStubs = new[] {new StubModel {Id = stub2.Id}, new StubModel {Id = stub3.Id}};

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub1, stub2, stub3});

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
        public async Task GetTenantNamesAsync_HappyFlow()
        {
            // arrange
            var stubSource = new Mock<IWritableStubSource>();

            var stub1 = new StubModel {Id = "stub1", Tenant = "tenant-2"};
            var stub2 = new StubModel {Id = "stub2", Tenant = "tenant-1"};
            var stub3 = new StubModel {Id = "stub3", Tenant = "tenant-1"};
            var stub4 = new StubModel {Id = "stub4", Tenant = null};
            var stub5 = new StubModel {Id = "stub5", Tenant = string.Empty};

            stubSource
                .Setup(m => m.GetStubsAsync())
                .ReturnsAsync(new[] {stub1, stub2, stub3, stub4, stub5});

            _stubSources.Add(stubSource.Object);

            // act
            var result = await _context.GetTenantNamesAsync();

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("tenant-1", result.ElementAt(0));
            Assert.AreEqual("tenant-2", result.ElementAt(1));
        }

        [TestMethod]
        public async Task PrepareAsync_HappyFlow()
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
