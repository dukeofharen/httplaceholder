using System.Linq;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Persistence.Tests.Db;

[TestClass]
public class RelationalDbStubCacheFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockLogger<RelationalDbStubCache> _mockLogger = new();

    [TestInitialize]
    public void Initialize() => _mocker.Use<ILogger<RelationalDbStubCache>>(_mockLogger);

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task GetOrUpdateStubCacheAsync_TrackingIdNotSetYet_ShouldInitCache()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        const string query = "GET TRACKING ID QUERY";
        mockQueryStore
            .Setup(m => m.GetStubUpdateTrackingIdQuery)
            .Returns(query);

        const string insertIdQuery = "INSERT TRACKING ID QUERY";
        mockQueryStore
            .Setup(m => m.InsertStubUpdateTrackingIdQuery)
            .Returns(insertIdQuery);

        mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(string.Empty);

        object capturedInsertParam = null;
        mockDatabaseContext
            .Setup(m => m.Execute(insertIdQuery, It.IsAny<object>()))
            .Callback<string, object>((_, param) => capturedInsertParam = param);

        // Act
        var result =
            await cache.GetOrUpdateStubCacheAsync(string.Empty, mockDatabaseContext.Object, CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedInsertParam);

        var parsedCapturedInsertParam = JObject.Parse(JsonConvert.SerializeObject(capturedInsertParam));
        Assert.AreEqual(cache.StubUpdateTrackingId, parsedCapturedInsertParam["StubUpdateTrackingId"].ToString());

        Assert.IsFalse(string.IsNullOrWhiteSpace(cache.StubUpdateTrackingId));
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.IsTrue(_mockLogger.ContainsWithExactText(LogLevel.Debug,
            "Initializing the cache, because there is no tracking ID in the database yet."));
    }

    [TestMethod]
    public async Task GetOrUpdateStubCacheAsync_StubCacheIsNull_ShouldInitCache()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        const string query = "GET TRACKING ID QUERY";
        mockQueryStore
            .Setup(m => m.GetStubUpdateTrackingIdQuery)
            .Returns(query);

        var trackingId = Guid.NewGuid().ToString();
        mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(trackingId);

        // Act
        var result =
            await cache.GetOrUpdateStubCacheAsync(string.Empty, mockDatabaseContext.Object, CancellationToken.None);

        // Assert
        Assert.AreEqual(trackingId, cache.StubUpdateTrackingId);
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.IsTrue(_mockLogger.ContainsWithExactText(LogLevel.Debug,
            "Initializing the cache, because either the local stub cache or tracking ID is not set yet."));
    }

    [TestMethod]
    public async Task GetOrUpdateStubCacheAsync_TrackingIdHasChanged_ShouldInitCache()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        var oldTrackingId = Guid.NewGuid().ToString();
        cache.StubUpdateTrackingId = oldTrackingId;

        const string query = "GET TRACKING ID QUERY";
        mockQueryStore
            .Setup(m => m.GetStubUpdateTrackingIdQuery)
            .Returns(query);

        var newTrackingId = Guid.NewGuid().ToString();
        mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(newTrackingId);

        // Act
        var result =
            await cache.GetOrUpdateStubCacheAsync(string.Empty, mockDatabaseContext.Object, CancellationToken.None);

        // Assert
        Assert.AreEqual(newTrackingId, cache.StubUpdateTrackingId);
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.IsTrue(_mockLogger.ContainsWithExactText(LogLevel.Debug,
            "Initializing the cache, because the tracking ID in the database has been changed."));
    }

    [TestMethod]
    public async Task GetOrUpdateStubCacheAsync_VerifyStubParsing()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        var oldTrackingId = Guid.NewGuid().ToString();
        cache.StubUpdateTrackingId = oldTrackingId;

        const string query = "GET TRACKING ID QUERY";
        mockQueryStore
            .Setup(m => m.GetStubUpdateTrackingIdQuery)
            .Returns(query);

        const string getStubsQuery = "GET STUBS QUERY";
        mockQueryStore
            .Setup(m => m.GetStubsQuery)
            .Returns(getStubsQuery);

        var newTrackingId = Guid.NewGuid().ToString();
        mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(newTrackingId);

        var dbStubModels = new[]
        {
            new DbStubModel
            {
                Id = 1,
                StubId = "stub-1",
                StubType = "json",
                Stub = JsonConvert.SerializeObject(new StubModel {Id = "stub-1"})
            },
            new DbStubModel
            {
                Id = 2,
                StubId = "stub-2",
                StubType = "yaml",
                Stub = YamlUtilities.Serialize(new StubModel {Id = "stub-2"})
            }
        };

        object capturedParam = null;
        mockDatabaseContext
            .Setup(m => m.Query<DbStubModel>(getStubsQuery, It.IsAny<object>()))
            .Callback<string, object>((_, param) => capturedParam = param)
            .Returns(dbStubModels);

        // Act
        var result =
            (await cache.GetOrUpdateStubCacheAsync(string.Empty, mockDatabaseContext.Object, CancellationToken.None))
            .ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.IsTrue(result.All(s => s.Id is "stub-1" or "stub-2"));
        Assert.IsNotNull(capturedParam);
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(string.Empty, parsedParam["DistributionKey"].ToString());
    }

    [TestMethod]
    public async Task GetOrUpdateStubCacheAsync_DistributionKeySet_ShouldNotCache()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        const string getStubsQuery = "GET STUBS QUERY";
        mockQueryStore
            .Setup(m => m.GetStubsQuery)
            .Returns(getStubsQuery);

        var dbStubModels = new[]
        {
            new DbStubModel
            {
                Id = 1,
                StubId = "stub-1",
                StubType = "json",
                Stub = JsonConvert.SerializeObject(new StubModel {Id = "stub-1"})
            },
            new DbStubModel
            {
                Id = 2,
                StubId = "stub-2",
                StubType = "yaml",
                Stub = YamlUtilities.Serialize(new StubModel {Id = "stub-2"})
            }
        };

        object capturedParam = null;
        mockDatabaseContext
            .Setup(m => m.Query<DbStubModel>(getStubsQuery, It.IsAny<object>()))
            .Callback<string, object>((_, param) => capturedParam = param)
            .Returns(dbStubModels);

        // Act
        var result =
            (await cache.GetOrUpdateStubCacheAsync("username", mockDatabaseContext.Object, CancellationToken.None))
            .ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.IsFalse(cache.StubCache.Any());

        Assert.IsTrue(result.All(s => s.Id is "stub-1" or "stub-2"));
        Assert.IsNotNull(capturedParam);
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual("username", parsedParam["DistributionKey"].ToString());
    }

    [TestMethod]
    public async Task GetOrUpdateStubCacheAsync_UnknownStubType_ShouldThrowNotImplementedException()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        var oldTrackingId = Guid.NewGuid().ToString();
        cache.StubUpdateTrackingId = oldTrackingId;

        const string query = "GET TRACKING ID QUERY";
        mockQueryStore
            .Setup(m => m.GetStubUpdateTrackingIdQuery)
            .Returns(query);

        const string getStubsQuery = "GET STUBS QUERY";
        mockQueryStore
            .Setup(m => m.GetStubsQuery)
            .Returns(getStubsQuery);

        var newTrackingId = Guid.NewGuid().ToString();
        mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(newTrackingId);

        var dbStubModels = new[]
        {
            new DbStubModel {Id = 1, StubId = "stub-1", StubType = "xml", Stub = "XML, BUT NOT SUPPORTED!"}
        };
        mockDatabaseContext
            .Setup(m => m.Query<DbStubModel>(getStubsQuery, It.IsAny<object>()))
            .Returns(dbStubModels);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<NotImplementedException>(() =>
                cache.GetOrUpdateStubCacheAsync(string.Empty, mockDatabaseContext.Object, CancellationToken.None));

        // Assert
        Assert.AreEqual("StubType 'xml' not supported: stub 'stub-1'.", exception.Message);
    }

    [TestMethod]
    public async Task GetOrUpdateStubCacheAsync_CallingMethodSeveralTimesShouldReturnSameResult()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        var oldTrackingId = Guid.NewGuid().ToString();
        cache.StubUpdateTrackingId = oldTrackingId;

        const string query = "GET TRACKING ID QUERY";
        mockQueryStore
            .Setup(m => m.GetStubUpdateTrackingIdQuery)
            .Returns(query);

        var newTrackingId = Guid.NewGuid().ToString();
        mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(newTrackingId);

        // Act / Assert
        Assert.IsTrue(
            (await cache.GetOrUpdateStubCacheAsync(string.Empty, mockDatabaseContext.Object, CancellationToken.None))
            .SequenceEqual(await cache.GetOrUpdateStubCacheAsync(string.Empty, mockDatabaseContext.Object,
                CancellationToken.None)));
    }

    [TestMethod]
    public async Task AddOrReplaceStubAsync_StubAlreadyExists_ShouldReplaceStub()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        var existingStub = new StubModel {Id = "stub1"};
        Assert.IsTrue(cache.StubCache.TryAdd(existingStub.Id, existingStub));

        var newStub = new StubModel {Id = "stub1"};

        // Act
        await cache.AddOrReplaceStubAsync(mockDatabaseContext.Object, newStub, CancellationToken.None);

        // Assert
        Assert.AreEqual(1, cache.StubCache.Count);
        Assert.IsTrue(cache.StubCache.Values.Contains(newStub));
    }

    [TestMethod]
    public async Task AddOrReplaceStubAsync_StubDoesNotExist_ShouldAddStub()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        var existingStub = new StubModel {Id = "stub2"};
        Assert.IsTrue(cache.StubCache.TryAdd(existingStub.Id, existingStub));

        var newStub = new StubModel {Id = "stub1"};

        // Act
        await cache.AddOrReplaceStubAsync(mockDatabaseContext.Object, newStub, CancellationToken.None);

        // Assert
        Assert.AreEqual(2, cache.StubCache.Count);
        Assert.IsTrue(cache.StubCache.Values.Contains(newStub));
        Assert.IsTrue(cache.StubCache.Values.Contains(existingStub));
    }

    [TestMethod]
    public async Task AddOrReplaceStubAsync_CheckTrackingIdIsUpdated()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();

        var trackingId = Guid.NewGuid().ToString();
        cache.StubUpdateTrackingId = trackingId;

        var newStub = new StubModel {Id = "stub1"};

        const string updateTrackingIdQuery = "UPDATE TRACKING ID";
        mockQueryStore
            .Setup(m => m.UpdateStubUpdateTrackingIdQuery)
            .Returns(updateTrackingIdQuery);

        object capturedInsertParam = null;
        mockDatabaseContext
            .Setup(m => m.ExecuteAsync(updateTrackingIdQuery, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedInsertParam = param);

        // Act
        await cache.AddOrReplaceStubAsync(mockDatabaseContext.Object, newStub, CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedInsertParam);

        var parsedCapturedInsertParam = JObject.Parse(JsonConvert.SerializeObject(capturedInsertParam));
        Assert.AreEqual(cache.StubUpdateTrackingId, parsedCapturedInsertParam["StubUpdateTrackingId"].ToString());
        Assert.AreNotEqual(cache.StubUpdateTrackingId, trackingId);
    }

    [TestMethod]
    public async Task DeleteStubAsync_StubFound_ShouldDeleteStubAndUpdateTrackingId()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();
        var mockQueryStore = _mocker.GetMock<IQueryStore>();

        var trackingId = Guid.NewGuid().ToString();
        cache.StubUpdateTrackingId = trackingId;

        var stub = new StubModel {Id = "stub1"};
        Assert.IsTrue(cache.StubCache.TryAdd(stub.Id, stub));

        const string updateTrackingIdQuery = "UPDATE TRACKING ID";
        mockQueryStore
            .Setup(m => m.UpdateStubUpdateTrackingIdQuery)
            .Returns(updateTrackingIdQuery);

        object capturedInsertParam = null;
        mockDatabaseContext
            .Setup(m => m.ExecuteAsync(updateTrackingIdQuery, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedInsertParam = param);

        // Act
        await cache.DeleteStubAsync(mockDatabaseContext.Object, stub.Id, CancellationToken.None);

        // Assert
        Assert.AreEqual(0, cache.StubCache.Count);

        Assert.IsNotNull(capturedInsertParam);

        var parsedCapturedInsertParam = JObject.Parse(JsonConvert.SerializeObject(capturedInsertParam));
        Assert.AreEqual(cache.StubUpdateTrackingId, parsedCapturedInsertParam["StubUpdateTrackingId"].ToString());
        Assert.AreNotEqual(cache.StubUpdateTrackingId, trackingId);
    }

    [TestMethod]
    public async Task DeleteStubAsync_StubNotFound_ShouldNotDeleteStub()
    {
        // Arrange
        var cache = _mocker.CreateInstance<RelationalDbStubCache>();
        var mockDatabaseContext = _mocker.GetMock<IDatabaseContext>();

        var trackingId = Guid.NewGuid().ToString();
        cache.StubUpdateTrackingId = trackingId;

        var stub = new StubModel {Id = "stub1"};
        Assert.IsTrue(cache.StubCache.TryAdd(stub.Id, stub));

        // Act
        await cache.DeleteStubAsync(mockDatabaseContext.Object, "stub2", CancellationToken.None);

        // Assert
        Assert.IsTrue(cache.StubCache.Values.Contains(stub));
        Assert.AreEqual(trackingId, cache.StubUpdateTrackingId);
        mockDatabaseContext.Verify(
            m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object>()), Times.Never);
    }
}
