using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Common;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.TestUtilities.Options;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class StubContextFacts
{
    private const string DistrubutionKey = "username";
    private readonly AutoMocker _mocker = new();
    private readonly SettingsModel _settings = new() { Storage = new StorageSettingsModel() };
    private readonly IList<IStubSource> _stubSources = new List<IStubSource>();

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use<IEnumerable<IStubSource>>(_stubSources);
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
        _mocker.GetMock<IStubRequestContext>()
            .Setup(m => m.DistributionKey)
            .Returns(DistrubutionKey);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _mocker.GetMock<IRequestNotify>().VerifyAll();
        _mocker.GetMock<IStubNotify>().VerifyAll();
    }

    [TestMethod]
    public async Task GetStubsAsync_HappyFlow()
    {
        // arrange
        var stubSource1 = new Mock<IStubSource>();
        var stubSource2 = new Mock<IStubSource>();

        var stub1 = GetTuple(new StubModel(), "file1.yml");
        var stub2 = GetTuple(new StubModel(), "file1.yml");
        var stub3 = GetTuple(new StubModel(), "file2.yml");

        stubSource1
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { stub1, stub2 });

        stubSource2
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { stub3 });

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetStubsAsync(CancellationToken.None)).ToArray();

        // assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(stub1.Stub, result[0].Stub);
        Assert.AreEqual("file1.yml", result[0].Metadata.Filename);
        Assert.AreEqual(stub2.Stub, result[1].Stub);
        Assert.AreEqual("file1.yml", result[1].Metadata.Filename);
        Assert.AreEqual(stub3.Stub, result[2].Stub);
        Assert.AreEqual("file2.yml", result[2].Metadata.Filename);
    }

    [TestMethod]
    public async Task GetStubsOverviewAsync_HappyFlow()
    {
        // arrange
        var stubSource1 = new Mock<IStubSource>();
        var stubSource2 = new Mock<IStubSource>();

        var stub1 = GetTuple(new StubOverviewModel(), "file1.yml");
        var stub2 = GetTuple(new StubOverviewModel(), "file1.yml");
        var stub3 = GetTuple(new StubOverviewModel(), "file2.yml");

        stubSource1
            .Setup(m => m.GetStubsOverviewAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { stub1, stub2 });

        stubSource2
            .Setup(m => m.GetStubsOverviewAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { stub3 });

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetStubsOverviewAsync(CancellationToken.None)).ToArray();

        // assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(stub1.Stub, result[0].Stub);
        Assert.AreEqual("file1.yml", result[0].Metadata.Filename);
        Assert.AreEqual(stub2.Stub, result[1].Stub);
        Assert.AreEqual("file1.yml", result[1].Metadata.Filename);
        Assert.AreEqual(stub3.Stub, result[2].Stub);
        Assert.AreEqual("file2.yml", result[2].Metadata.Filename);
    }

    [TestMethod]
    public async Task GetStubsAsync_ByTenant_HappyFlow()
    {
        // arrange
        var stubSource1 = new Mock<IStubSource>();
        var stubSource2 = new Mock<IStubSource>();

        var stub1 = GetTuple(new StubModel { Tenant = "tenant1" }, "file1.yml");
        var stub2 = GetTuple(new StubModel { Tenant = "tenant2" }, "file1.yml");
        var stub3 = GetTuple(new StubModel { Tenant = "TENaNT1" }, "file2.yml");

        stubSource1
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { stub1, stub2 });

        stubSource2
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { stub3 });

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetStubsAsync("tenant1", CancellationToken.None)).ToArray();

        // assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(stub1.Stub, result[0].Stub);
        Assert.AreEqual("file1.yml", result[0].Metadata.Filename);
        Assert.AreEqual(stub3.Stub, result[1].Stub);
        Assert.AreEqual("file2.yml", result[1].Metadata.Filename);
    }

    [TestMethod]
    public async Task
        AddStubAsync_StubIdAlreadyAddedToReadOnlyStubSource_ShouldThrowConflictException()
    {
        // arrange
        var stubToBeAdded = new StubModel { Id = "conflicted" };
        var stub = new StubModel { Id = "COnflicted" };
        var writableStubSource = new Mock<IWritableStubSource>();
        var readOnlyStubSource = new Mock<IStubSource>();
        readOnlyStubSource
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { GetTuple(stub) });

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
        var stubToBeAdded = new StubModel { Id = "new-stub-02" };
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.AddStubAsync(stubToBeAdded, DistrubutionKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var stub = new StubModel { Id = "new-stub-01" };
        var readOnlyStubSource = new Mock<IStubSource>();
        readOnlyStubSource
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { GetTuple(stub) });

        _stubSources.Add(stubSource.Object);
        _stubSources.Add(readOnlyStubSource.Object);

        var stubNotifyMock = _mocker.GetMock<IStubNotify>();
        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddStubAsync(stubToBeAdded, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.AddStubAsync(stubToBeAdded, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
        stubNotifyMock.Verify(m => m.StubAddedAsync(It.Is<FullStubOverviewModel>(s => s.Stub.Id == stubToBeAdded.Id),
            DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task DeleteStubAsync_HappyFlow()
    {
        // arrange
        const string stubId = "stubId1";
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.DeleteStubAsync(stubId, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _stubSources.Add(stubSource.Object);

        var stubNotifyMock = _mocker.GetMock<IStubNotify>();
        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.DeleteStubAsync(stubId, CancellationToken.None);

        // assert
        Assert.IsTrue(result);
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stubId, DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task GetStubAsync_HappyFlow()
    {
        // arrange
        var stubSource1 = new Mock<IStubSource>();
        var stubSource2 = new Mock<IStubSource>();

        var stub1 = GetTuple(new StubModel { Id = "stub1" }, "file1.yml");
        var stub2 = GetTuple(new StubModel { Id = "stub2" }, "file2.yml");

        stubSource1
            .Setup(m => m.GetStubAsync(stub2.Stub.Id, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub2);
        stubSource2
            .Setup(m => m.GetStubAsync(stub1.Stub.Id, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stub1);

        _stubSources.Add(stubSource1.Object);
        _stubSources.Add(stubSource2.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.GetStubAsync("stub2", CancellationToken.None);

        // assert
        Assert.AreEqual(stub2.Stub, result.Stub);
        Assert.AreEqual("file2.yml", result.Metadata.Filename);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow()
    {
        // arrange
        var stubSource = new Mock<IWritableStubSource>();
        _settings.Storage.StoreResponses = true;

        var stub = new StubModel { Id = "stub1", Tenant = "tenant1" };
        stubSource
            .Setup(m => m.GetStubAsync(stub.Id, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTuple(stub));

        var request = new RequestResultModel { ExecutingStubId = stub.Id };
        var response = new ResponseModel();
        stubSource
            .Setup(m => m.AddRequestResultAsync(request, response, DistrubutionKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddRequestResultAsync(request, response, CancellationToken.None);

        // assert
        stubSource.Verify(
            m => m.AddRequestResultAsync(request, response, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
        stubSource.Verify(m => m.CleanOldRequestResultsAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.AreEqual(stub.Tenant, request.StubTenant);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow_DoNotStoreResponse()
    {
        // arrange
        var stubSource = new Mock<IWritableStubSource>();
        _settings.Storage.StoreResponses = false;

        var stub = new StubModel { Id = "stub1", Tenant = "tenant1" };
        stubSource
            .Setup(m => m.GetStubAsync(stub.Id, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTuple(stub));

        var request = new RequestResultModel { ExecutingStubId = stub.Id };
        var response = new ResponseModel();
        stubSource
            .Setup(m => m.AddRequestResultAsync(request, response, DistrubutionKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddRequestResultAsync(request, response, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.AddRequestResultAsync(request, null, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_CleanOldRequestsJobEnabled_ShouldNotCallCleanOldRequestResultsAsync()
    {
        // arrange
        _settings.Storage.CleanOldRequestsInBackgroundJob = true;
        _settings.Storage.StoreResponses = true;

        var stubSource = new Mock<IWritableStubSource>();

        var stub = new StubModel { Id = "stub1", Tenant = "tenant1" };
        stubSource
            .Setup(m => m.GetStubAsync(stub.Id, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetTuple(stub));

        var request = new RequestResultModel { ExecutingStubId = stub.Id };
        var response = new ResponseModel();
        stubSource
            .Setup(m => m.AddRequestResultAsync(request, response, DistrubutionKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var requestNotifyMock = _mocker.GetMock<IRequestNotify>();
        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.AddRequestResultAsync(request, response, CancellationToken.None);

        // assert
        stubSource.Verify(
            m => m.AddRequestResultAsync(request, response, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
        stubSource.Verify(m => m.CleanOldRequestResultsAsync(It.IsAny<CancellationToken>()),
            Times.Never);
        requestNotifyMock.Verify(
            m => m.NewRequestReceivedAsync(request, DistrubutionKey, It.IsAny<CancellationToken>()), Times.Once);
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
        stubSource.Verify(m => m.CleanOldRequestResultsAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task GetRequestResultsAsync_HappyFlow()
    {
        // arrange
        var request1 = new RequestResultModel { RequestBeginTime = DateTime.Now.AddSeconds(-2) };
        var request2 = new RequestResultModel { RequestBeginTime = DateTime.Now.AddSeconds(-1) };
        var requests = new[] { request1, request2 };
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.GetRequestResultsAsync(null, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(requests);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetRequestResultsAsync(null, It.IsAny<CancellationToken>())).ToArray();

        // assert
        Assert.AreEqual(request1, result[0]);
        Assert.AreEqual(request2, result[1]);
    }

    [TestMethod]
    public async Task GetRequestResultsOverviewAsync_HappyFlow()
    {
        // arrange
        var request1 = new RequestOverviewModel { RequestEndTime = DateTime.Now.AddSeconds(-2) };
        var request2 = new RequestOverviewModel { RequestEndTime = DateTime.Now.AddSeconds(-1) };
        var requests = new[] { request1, request2 };
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.GetRequestResultsOverviewAsync(null, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(requests);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = (await context.GetRequestResultsOverviewAsync(null, CancellationToken.None)).ToArray();

        // assert
        Assert.AreEqual(request2, result[1]);
        Assert.AreEqual(request1, result[0]);
    }

    [TestMethod]
    public async Task GetRequestResultAsync_HappyFlow()
    {
        // arrange
        var correlationId = Guid.NewGuid().ToString();
        var request = new RequestResultModel { CorrelationId = correlationId };
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.GetRequestAsync(correlationId, DistrubutionKey, It.IsAny<CancellationToken>()))
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
            .Setup(m => m.GetResponseAsync(correlationId, DistrubutionKey, It.IsAny<CancellationToken>()))
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
        var requests = new[] { request1, request2, request3 };
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.GetRequestResultsAsync(null, DistrubutionKey, It.IsAny<CancellationToken>()))
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
            .Setup(m => m.DeleteAllRequestResultsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.DeleteAllRequestResultsAsync(CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteAllRequestResultsAsync(DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeleteRequestAsync_HappyFlow()
    {
        // arrange
        var correlationId = Guid.NewGuid().ToString();
        var stubSource = new Mock<IWritableStubSource>();
        stubSource
            .Setup(m => m.DeleteRequestAsync(correlationId, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _stubSources.Add(stubSource.Object);

        var context = _mocker.CreateInstance<StubContext>();

        // act
        var result = await context.DeleteRequestAsync(correlationId, CancellationToken.None);

        // assert
        Assert.IsTrue(result);
        stubSource.Verify(m => m.DeleteRequestAsync(correlationId, DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task DeleteAllStubsAsync_Tenant_HappyFlow()
    {
        // arrange
        const string tenant = "tenant1";
        var stubSource = new Mock<IWritableStubSource>();

        var stub1 = new StubModel { Id = "stub1", Tenant = tenant };
        var stub2 = new StubModel { Id = "stub2", Tenant = $"{tenant}bla" };
        var stub3 = new StubModel { Id = "stub3", Tenant = tenant.ToUpper() };

        stubSource
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { GetTuple(stub1), GetTuple(stub2), GetTuple(stub3) });

        _stubSources.Add(stubSource.Object);

        var stubNotifyMock = _mocker.GetMock<IStubNotify>();
        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.DeleteAllStubsAsync(tenant, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteStubAsync(stub1.Id, DistrubutionKey, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub2.Id, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Never);
        stubSource.Verify(m => m.DeleteStubAsync(stub3.Id, DistrubutionKey, It.IsAny<CancellationToken>()), Times.Once);
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub1.Id, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub2.Id, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Never);
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub3.Id, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeleteAllStubsAsync_HappyFlow()
    {
        // arrange
        var stubSource = new Mock<IWritableStubSource>();

        var stub1 = new StubModel { Id = "stub1", Tenant = "tenant1" };
        var stub2 = new StubModel { Id = "stub2", Tenant = "tenant2" };
        var stub3 = new StubModel { Id = "stub3", Tenant = "tenant1" };

        stubSource
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { GetTuple(stub1), GetTuple(stub2), GetTuple(stub3) });

        _stubSources.Add(stubSource.Object);

        var stubNotifyMock = _mocker.GetMock<IStubNotify>();
        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.DeleteAllStubsAsync(CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteStubAsync(stub1.Id, DistrubutionKey, It.IsAny<CancellationToken>()));
        stubSource.Verify(m => m.DeleteStubAsync(stub2.Id, DistrubutionKey, It.IsAny<CancellationToken>()));
        stubSource.Verify(m => m.DeleteStubAsync(stub3.Id, DistrubutionKey, It.IsAny<CancellationToken>()));
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub1.Id, DistrubutionKey, It.IsAny<CancellationToken>()));
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub2.Id, DistrubutionKey, It.IsAny<CancellationToken>()));
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub3.Id, DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task UpdateAllStubsAsync_HappyFlow()
    {
        // arrange
        const string tenant1 = "tenant1";
        const string tenant2 = "tenant2";
        var stubSource = new Mock<IWritableStubSource>();

        var stub1 = new StubModel { Id = "stub1", Tenant = tenant1 };
        var stub2 = new StubModel { Id = "stub2", Tenant = tenant2 };
        var stub3 = new StubModel { Id = "stub3", Tenant = tenant1.ToUpper() };

        var newStubs = new[] { new StubModel { Id = stub2.Id }, new StubModel { Id = stub3.Id } };

        stubSource
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { GetTuple(stub1), GetTuple(stub2), GetTuple(stub3) });

        _stubSources.Add(stubSource.Object);

        var stubNotifyMock = _mocker.GetMock<IStubNotify>();
        var context = _mocker.CreateInstance<StubContext>();

        // act
        await context.UpdateAllStubs(tenant1, newStubs, CancellationToken.None);

        // assert
        stubSource.Verify(m => m.DeleteStubAsync(stub1.Id, DistrubutionKey, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub2.Id, DistrubutionKey, It.IsAny<CancellationToken>()), Times.Once);
        stubSource.Verify(m => m.DeleteStubAsync(stub3.Id, DistrubutionKey, It.IsAny<CancellationToken>()), Times.Once);
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub1.Id, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub2.Id, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
        stubNotifyMock.Verify(m => m.StubDeletedAsync(stub3.Id, DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);

        stubSource.Verify(
            m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub1.Id), DistrubutionKey,
                It.IsAny<CancellationToken>()),
            Times.Never);
        stubSource.Verify(
            m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub2.Id), DistrubutionKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
        stubSource.Verify(
            m => m.AddStubAsync(It.Is<StubModel>(s => s.Id == stub3.Id), DistrubutionKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
        stubNotifyMock.Verify(
            m => m.StubAddedAsync(It.Is<FullStubOverviewModel>(s => s.Stub.Id == stub1.Id),
                DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Never);
        stubNotifyMock.Verify(
            m => m.StubAddedAsync(It.Is<FullStubOverviewModel>(s => s.Stub.Id == stub2.Id),
                DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);
        stubNotifyMock.Verify(
            m => m.StubAddedAsync(It.Is<FullStubOverviewModel>(s => s.Stub.Id == stub3.Id),
                DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.IsTrue(newStubs.All(s => s.Tenant == tenant1));
    }

    [TestMethod]
    public async Task GetTenantNamesAsync_HappyFlow()
    {
        // arrange
        var stubSource = new Mock<IWritableStubSource>();

        var stub1 = new StubModel { Id = "stub1", Tenant = "tenant-2" };
        var stub2 = new StubModel { Id = "stub2", Tenant = "tenant-1" };
        var stub3 = new StubModel { Id = "stub3", Tenant = "tenant-1" };
        var stub4 = new StubModel { Id = "stub4", Tenant = null };
        var stub5 = new StubModel { Id = "stub5", Tenant = string.Empty };

        stubSource
            .Setup(m => m.GetStubsAsync(DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { GetTuple(stub1), GetTuple(stub2), GetTuple(stub3), GetTuple(stub4), GetTuple(stub5) });

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

    [TestMethod]
    public async Task IncreaseHitCountAsync_ScenarioNotSet_ShouldDoNothing()
    {
        // Arrange
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        // Act
        await context.IncreaseHitCountAsync(null, CancellationToken.None);

        // Assert
        stubSource.Verify(
            m => m.UpdateScenarioAsync(It.IsAny<string>(), It.IsAny<ScenarioStateModel>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task IncreaseHitCountAsync_ScenarioSet_ShouldUpdateHitCount()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var currentState = new ScenarioStateModel(scenario) { HitCount = 11, State = Guid.NewGuid().ToString() };
        stubSource
            .Setup(m => m.GetScenarioAsync(scenario, DistrubutionKey, CancellationToken.None))
            .ReturnsAsync(currentState);

        // Act
        await context.IncreaseHitCountAsync(scenario, CancellationToken.None);

        // Assert
        stubSource.Verify(
            m => m.UpdateScenarioAsync(scenario, It.Is<ScenarioStateModel>(s => s.HitCount == 12), DistrubutionKey,
                It.IsAny<CancellationToken>()));
        _mocker.GetMock<ICacheService>().Verify(s => s.SetScopedItem(CachingKeys.ScenarioState,
            It.Is<ScenarioStateModel>(m => m.State == currentState.State && m.HitCount == currentState.HitCount)));
        _mocker.GetMock<IScenarioNotify>().Verify(m =>
            m.ScenarioSetAsync(
                It.Is<ScenarioStateModel>(s => s.State == currentState.State && s.HitCount == currentState.HitCount),
                DistrubutionKey, CancellationToken.None));
    }

    [TestMethod]
    public async Task GetHitCountAsync_ScenarioNotSet_ShouldReturnNull()
    {
        // Arrange
        var context = _mocker.CreateInstance<StubContext>();

        // Act
        var result = await context.GetHitCountAsync(null, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetHitCountAsync_ScenarioSet_ShouldReturnHitCount()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var currentState = new ScenarioStateModel(scenario) { HitCount = 11, State = Guid.NewGuid().ToString() };
        stubSource
            .Setup(m => m.GetScenarioAsync(scenario, DistrubutionKey, CancellationToken.None))
            .ReturnsAsync(currentState);

        // Act
        var result = await context.GetHitCountAsync(scenario, CancellationToken.None);

        // Assert
        Assert.AreEqual(11, result);
    }

    [TestMethod]
    public async Task GetAllScenariosAsync_HappyFlow()
    {
        // Arrange
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var allScenarios = new[] { new ScenarioStateModel("scenario-1"), new ScenarioStateModel("scenario-2") };
        stubSource
            .Setup(m => m.GetAllScenariosAsync(DistrubutionKey, CancellationToken.None))
            .ReturnsAsync(allScenarios);

        // Act
        var result = await context.GetAllScenariosAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(allScenarios, result);
    }

    [TestMethod]
    public async Task GetScenarioAsync_HappyFlow()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var scenarioState = new ScenarioStateModel(scenario);
        stubSource
            .Setup(m => m.GetScenarioAsync(scenario, DistrubutionKey, CancellationToken.None))
            .ReturnsAsync(scenarioState);

        // Act
        var result = await context.GetScenarioAsync(scenario, CancellationToken.None);

        // Assert
        Assert.AreEqual(scenarioState, result);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioNotSet_ShouldDoNothing()
    {
        // Arrange
        var context = _mocker.CreateInstance<StubContext>();

        // Act
        await context.SetScenarioAsync(null, null, CancellationToken.None);

        // Assert
        _mocker.GetMock<IScenarioNotify>().Verify(
            m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>(), DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioStateNotSet_ShouldDoNothing()
    {
        // Arrange
        var context = _mocker.CreateInstance<StubContext>();

        // Act
        await context.SetScenarioAsync("scenario-1", null, CancellationToken.None);

        // Assert
        _mocker.GetMock<IScenarioNotify>().Verify(
            m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>(), DistrubutionKey, It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioNotFound_NewStateNotSet_ShouldAddAndSetStateToDefault()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var newState = new ScenarioStateModel(scenario) { State = string.Empty };

        // Act
        await context.SetScenarioAsync(scenario, newState, CancellationToken.None);

        // Assert
        stubSource.Verify(m => m.AddScenarioAsync(scenario,
            It.Is<ScenarioStateModel>(s => s.State == Constants.DefaultScenarioState),
            DistrubutionKey, It.IsAny<CancellationToken>()));
        _mocker.GetMock<IScenarioNotify>().Verify(
            m => m.ScenarioSetAsync(newState, DistrubutionKey, It.IsAny<CancellationToken>()));
        _mocker.GetMock<ICacheService>().Verify(m => m.SetScopedItem(CachingKeys.ScenarioState,
            It.Is<ScenarioStateModel>(s => s.State == Constants.DefaultScenarioState)));
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioNotFound_NewStateSet_ShouldAddAndSetState()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var newState = new ScenarioStateModel(scenario) { State = Guid.NewGuid().ToString() };

        // Act
        await context.SetScenarioAsync(scenario, newState, CancellationToken.None);

        // Assert
        stubSource.Verify(m => m.AddScenarioAsync(scenario,
            It.Is<ScenarioStateModel>(s => s.State == newState.State),
            DistrubutionKey, It.IsAny<CancellationToken>()));
        _mocker.GetMock<IScenarioNotify>().Verify(
            m => m.ScenarioSetAsync(newState, DistrubutionKey, It.IsAny<CancellationToken>()));
        _mocker.GetMock<ICacheService>().Verify(m => m.SetScopedItem(CachingKeys.ScenarioState,
            It.Is<ScenarioStateModel>(s => s.State == newState.State)));
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioFound_HitCountAndStateNotSet_ShouldSetNewValuesToCurrentValues()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var currentScenario = new ScenarioStateModel(scenario) { HitCount = 11, State = Guid.NewGuid().ToString() };
        stubSource
            .Setup(m => m.GetScenarioAsync(scenario, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentScenario);

        var newState = new ScenarioStateModel(scenario) { HitCount = -1, State = string.Empty };

        // Act
        await context.SetScenarioAsync(scenario, newState, CancellationToken.None);

        // Assert
        stubSource.Verify(m => m.UpdateScenarioAsync(scenario,
            It.Is<ScenarioStateModel>(s => s.State == currentScenario.State && s.HitCount == currentScenario.HitCount),
            DistrubutionKey, CancellationToken.None));
        _mocker.GetMock<ICacheService>().Verify(m => m.SetScopedItem(CachingKeys.ScenarioState,
            It.Is<ScenarioStateModel>(s =>
                s.State == currentScenario.State && s.HitCount == currentScenario.HitCount)));
        _mocker.GetMock<IScenarioNotify>().Verify(m =>
            m.ScenarioSetAsync(
                It.Is<ScenarioStateModel>(s =>
                    s.State == currentScenario.State && s.HitCount == currentScenario.HitCount),
                DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioFound_HitCountAndStateSet_ShouldSetNewValues()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        var currentScenario = new ScenarioStateModel(scenario) { HitCount = 11, State = Guid.NewGuid().ToString() };
        stubSource
            .Setup(m => m.GetScenarioAsync(scenario, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentScenario);

        var newState = new ScenarioStateModel(scenario) { HitCount = 13, State = Guid.NewGuid().ToString() };

        // Act
        await context.SetScenarioAsync(scenario, newState, CancellationToken.None);

        // Assert
        stubSource.Verify(m => m.UpdateScenarioAsync(scenario,
            It.Is<ScenarioStateModel>(s => s.State == newState.State && s.HitCount == newState.HitCount),
            DistrubutionKey, CancellationToken.None));
        _mocker.GetMock<ICacheService>().Verify(m => m.SetScopedItem(CachingKeys.ScenarioState,
            It.Is<ScenarioStateModel>(s => s.State == newState.State && s.HitCount == newState.HitCount)));
        _mocker.GetMock<IScenarioNotify>().Verify(m =>
            m.ScenarioSetAsync(
                It.Is<ScenarioStateModel>(s => s.State == newState.State && s.HitCount == newState.HitCount),
                DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task DeleteScenarioAsync_ScenarioNotSet_ShouldReturnFalse()
    {
        // Arrange
        var context = _mocker.CreateInstance<StubContext>();

        // Act
        var result = await context.DeleteScenarioAsync(null, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task DeleteScenarioAsync_ScenarioSet_ScenarioNotDeleted_ShouldReturnFalse()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        stubSource
            .Setup(m => m.DeleteScenarioAsync(scenario, DistrubutionKey, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await context.DeleteScenarioAsync(scenario, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        _mocker.GetMock<ICacheService>().Verify(m => m.DeleteScopedItem(CachingKeys.ScenarioState));
        _mocker.GetMock<IScenarioNotify>().Verify(m =>
            m.ScenarioDeletedAsync(scenario, DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task DeleteScenarioAsync_ScenarioSet_ScenarioDeleted_ShouldReturnTrue()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        stubSource
            .Setup(m => m.DeleteScenarioAsync(scenario, DistrubutionKey, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await context.DeleteScenarioAsync(scenario, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        _mocker.GetMock<ICacheService>().Verify(m => m.DeleteScopedItem(CachingKeys.ScenarioState));
        _mocker.GetMock<IScenarioNotify>().Verify(m =>
            m.ScenarioDeletedAsync(scenario, DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task DeleteAllScenariosAsync_HappyFlow()
    {
        // Arrange
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();

        // Act
        await context.DeleteAllScenariosAsync(CancellationToken.None);

        // Assert
        stubSource.Verify(m => m.DeleteAllScenariosAsync(DistrubutionKey, It.IsAny<CancellationToken>()));
        _mocker.GetMock<IScenarioNotify>()
            .Verify(m => m.AllScenariosDeletedAsync(DistrubutionKey, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task GetOrAddScenarioState_ScenarioNotFound_ShouldAddScenario()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();
        stubSource
            .Setup(m => m.GetScenarioAsync(scenario, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScenarioStateModel)null);

        var newState = new ScenarioStateModel(scenario);
        stubSource
            .Setup(m => m.AddScenarioAsync(scenario, It.Is<ScenarioStateModel>(s => s.Scenario == scenario),
                DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newState);

        // Act
        var result = await context.GetOrAddScenarioState(scenario, DistrubutionKey, CancellationToken.None);

        // Assert
        Assert.AreEqual(newState, result);
        _mocker.GetMock<ICacheService>().Verify(m =>
            m.SetScopedItem(CachingKeys.ScenarioState, It.Is<ScenarioStateModel>(s => s.Scenario == scenario)));
        _mocker.GetMock<IScenarioNotify>().Verify(m =>
            m.ScenarioSetAsync(It.Is<ScenarioStateModel>(s => s.Scenario == scenario), DistrubutionKey,
                It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task GetOrAddScenarioState_ScenarioFound_ShouldGetScenario()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stubSource = InitializeWritableStubSource();
        var context = _mocker.CreateInstance<StubContext>();
        var currentState = new ScenarioStateModel(scenario);
        stubSource
            .Setup(m => m.GetScenarioAsync(scenario, DistrubutionKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentState);

        // Act
        var result = await context.GetOrAddScenarioState(scenario, DistrubutionKey, CancellationToken.None);

        // Assert
        Assert.AreEqual(currentState, result);
        stubSource.Verify(
            m => m.AddScenarioAsync(scenario, It.IsAny<ScenarioStateModel>(), DistrubutionKey,
                It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ICacheService>()
            .Verify(m => m.SetScopedItem(CachingKeys.ScenarioState, It.IsAny<ScenarioStateModel>()), Times.Never);
        _mocker.GetMock<IScenarioNotify>()
            .Verify(
                m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>(), DistrubutionKey, It.IsAny<CancellationToken>()),
                Times.Never);
    }

    private Mock<IWritableStubSource> InitializeWritableStubSource()
    {
        var writableStubSource = new Mock<IWritableStubSource>();
        _stubSources.Add(writableStubSource.Object);
        return writableStubSource;
    }

    private static (StubModel Stub, Dictionary<string, string> Metadata) GetTuple(StubModel stub,
        string filename = null)
    {
        var metadata = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(filename))
        {
            metadata.Add(StubMetadataKeys.Filename, filename);
        }

        return (stub, metadata);
    }

    private static (StubOverviewModel Stub, Dictionary<string, string> Metadata) GetTuple(StubOverviewModel stub,
        string filename = null)
    {
        var metadata = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(filename))
        {
            metadata.Add(StubMetadataKeys.Filename, filename);
        }

        return (stub, metadata);
    }
}
