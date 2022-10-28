using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Persistence.Implementations;

namespace HttPlaceholder.Persistence.Tests.Implementations;

[TestClass]
public class StubContextFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly SettingsModel _settings = new() {Storage = new StorageSettingsModel()};
    private readonly IList<IStubSource> _stubSources = new List<IStubSource>();

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use<IEnumerable<IStubSource>>(_stubSources);
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

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
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub1, stub2});

        stubSource2
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub3});

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetStubsAsync(CancellationToken.None)).ToArray();

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
            .Setup(m => m.GetStubsOverviewAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub1, stub2});

        stubSource2
            .Setup(m => m.GetStubsOverviewAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub3});

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetStubsOverviewAsync(CancellationToken.None)).ToArray();

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
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub1, stub2});

        stubSource2
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub3});

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetStubsAsync("tenant1", CancellationToken.None)).ToArray();

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
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub});

        _stubSources.Add(writableStubSource.Object);
        _stubSources.Add(readOnlyStubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act / assert
        await Assert.ThrowsExceptionAsync<ConflictException>(() =>
            context.AddStubAsync(stubToBeAdded, CancellationToken.None));
    }

    [TestMethod]
    public async Task AddStubAsync_HappyFlow()
    {
        // arrange
        var stubToBeAdded = new StubModel {Id = "new-stub-02"};
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.AddStubAsync(stubToBeAdded, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var stub = new StubModel {Id = "new-stub-01"};
        var readOnlyStubSource = new Mock<IStubSource>();
        readOnlyStubSource
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub});

        _stubSources.Add(stubSource.Object);
        _stubSources.Add(readOnlyStubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddStubAsync(stubToBeAdded, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.AddStubAsync(stubToBeAdded, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteStubAsync_HappyFlow()
    {
        // arrange
        const string stubId = "stubId1";
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.DeleteStubAsync(stubId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.DeleteStubAsync(stubId, CancellationToken.None);

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
            .Setup(m => m.GetStubAsync(stub2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub2);
        stubSource2
            .Setup(m => m.GetStubAsync(stub1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub1);

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.GetStubAsync("stub2", CancellationToken.None);

        // assert
        Assert.AreEqual(stub2, result.Stub);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow()
    {
        // arrange
        var stubSource = new Mock<IWritableStubSource>();
        _settings.Storage.StoreResponses = true;

        var stub = new StubModel {Id = "stub1", Tenant = "tenant1"};
        stubSource
            .Setup(m => m.GetStubAsync(stub.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub);

        var request = new RequestResultModel {ExecutingStubId = stub.Id};
        var response = new ResponseModel();
        stubSource
            .Setup(m => m.AddRequestResultAsync(request, response, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddRequestResultAsync(request, response, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.AddRequestResultAsync(request, response, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.CleanOldRequestResultsAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.AreEqual(stub.Tenant, request.StubTenant);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow_DoNotStoreResponse()
    {
        // arrange
        var stubSource = new Mock<IWritableStubSource>();
        _settings.Storage.StoreResponses = false;

        var stub = new StubModel {Id = "stub1", Tenant = "tenant1"};
        stubSource
            .Setup(m => m.GetStubAsync(stub.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub);

        var request = new RequestResultModel {ExecutingStubId = stub.Id};
        var response = new ResponseModel();
        stubSource
            .Setup(m => m.AddRequestResultAsync(request, response, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddRequestResultAsync(request, response, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.AddRequestResultAsync(request, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_CleanOldRequestsJobEnabled_ShouldNotCallCleanOldRequestResultsAsync()
    {
        // arrange
        _settings.Storage.CleanOldRequestsInBackgroundJob = true;
        _settings.Storage.StoreResponses = true;

        var stubSource = new Mock<IWritableStubSource>();

        var stub = new StubModel {Id = "stub1", Tenant = "tenant1"};
        stubSource
            .Setup(m => m.GetStubAsync(stub.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub);

        var request = new RequestResultModel {ExecutingStubId = stub.Id};
        var response = new ResponseModel();
        stubSource
            .Setup(m => m.AddRequestResultAsync(request, response, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var requestNotifyMock = _mocker.GetMock<IRequestNotify>();
        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddRequestResultAsync(request, response, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.AddRequestResultAsync(request, response, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.CleanOldRequestResultsAsync(It.IsAny<CancellationToken>()), Times.Never);
        requestNotifyMock.Verify(m => m.NewRequestReceivedAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task CleanOldRequestResultsAsync_HappyFlow()
    {
        // arrange
        var stubSource = new Mock<IWritableStubSource>();
        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.CleanOldRequestResultsAsync(CancellationToken.None);

        // assert
        stubSource.Verify(m => m.CleanOldRequestResultsAsync(It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(m => m.GetRequestResultsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(requests);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetRequestResultsAsync(It.IsAny<CancellationToken>())).ToArray();

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
            .Setup(m => m.GetRequestResultsOverviewAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(requests);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetRequestResultsOverviewAsync(CancellationToken.None)).ToArray();

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
            .Setup(m => m.GetRequestAsync(correlationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.GetRequestResultAsync(correlationId, CancellationToken.None);

        // assert
        Assert.AreEqual(request, result);
    }

    [TestMethod]
    public async Task GetResponseAsync_HappyFlow()
    {
        // arrange
        var correlationId = Guid.NewGuid().ToString();
        var response = new ResponseModel();
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.GetResponseAsync(correlationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.GetResponseAsync(correlationId, CancellationToken.None);

        // assert
        Assert.AreEqual(response, result);
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
            .Setup(m => m.GetRequestResultsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(requests);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetRequestResultsByStubIdAsync("stub1", CancellationToken.None)).ToArray();

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
            .Setup(m => m.DeleteAllRequestResultsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.DeleteAllRequestResultsAsync(CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteAllRequestResultsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteRequestAsync_HappyFlow()
    {
        // arrange
        var correlationId = Guid.NewGuid().ToString();
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.DeleteRequestAsync(correlationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.DeleteRequestAsync(correlationId, CancellationToken.None);

        // assert
        Assert.IsTrue(result);
        stubSource.Verify(m => m.DeleteRequestAsync(correlationId, It.IsAny<CancellationToken>()));
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
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub1, stub2, stub3});

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.DeleteAllStubsAsync(tenant, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteStubAsync(stub1.Id, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub2.Id, It.IsAny<CancellationToken>()), Times.Never);
        stubSource.Verify(m => m.DeleteStubAsync(stub3.Id, It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub1, stub2, stub3});

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.DeleteAllStubsAsync(CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteStubAsync(stub1.Id, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub2.Id, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub3.Id, It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub1, stub2, stub3});

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.UpdateAllStubs(tenant1, newStubs, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteStubAsync(stub1.Id, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub2.Id, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub3.Id, It.IsAny<CancellationToken>()), Times.Once);

        stubSource.Verify(m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub1.Id), It.IsAny<CancellationToken>()),
            Times.Never);
        stubSource.Verify(m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub2.Id), It.IsAny<CancellationToken>()),
            Times.Once);
        stubSource.Verify(m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub3.Id), It.IsAny<CancellationToken>()),
            Times.Once);

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
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {stub1, stub2, stub3, stub4, stub5});

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetTenantNamesAsync(CancellationToken.None)).ToArray();

        // assert
        Assert.AreEqual(2, result.Length);
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

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.PrepareAsync(CancellationToken.None);

        // assert
        stubSource1.Verify(m => m.PrepareStubSourceAsync(It.IsAny<CancellationToken>()), Times.Once);
        stubSource2.Verify(m => m.PrepareStubSourceAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
