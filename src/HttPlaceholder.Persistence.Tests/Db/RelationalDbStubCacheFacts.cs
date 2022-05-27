using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using HttPlaceholder.TestUtilities.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
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
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
            .ReturnsAsync(string.Empty);

        object capturedInsertParam = null;
        mockDatabaseContext
            .Setup(m => m.Execute(insertIdQuery, It.IsAny<object>()))
            .Callback<string, object>((_, param) => capturedInsertParam = param);

        // Act
        var result = await cache.GetOrUpdateStubCacheAsync(mockDatabaseContext.Object);

        // Assert
        Assert.IsNotNull(capturedInsertParam);

        var parsedCapturedInsertParam = JObject.Parse(JsonConvert.SerializeObject(capturedInsertParam));
        Assert.AreEqual(cache.StubUpdateTrackingId, parsedCapturedInsertParam["StubUpdateTrackingId"].ToString());

        Assert.IsFalse(string.IsNullOrWhiteSpace(cache.StubUpdateTrackingId));
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.IsTrue(_mockLogger.ContainsWithExactText(LogLevel.Information,
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
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
            .ReturnsAsync(trackingId);

        // Act
        var result = await cache.GetOrUpdateStubCacheAsync(mockDatabaseContext.Object);

        // Assert
        Assert.AreEqual(trackingId, cache.StubUpdateTrackingId);
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.IsTrue(_mockLogger.ContainsWithExactText(LogLevel.Information,
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
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
            .ReturnsAsync(newTrackingId);

        // Act
        var result = await cache.GetOrUpdateStubCacheAsync(mockDatabaseContext.Object);

        // Assert
        Assert.AreEqual(newTrackingId, cache.StubUpdateTrackingId);
        Assert.IsNotNull(result);
        Assert.IsNotNull(cache.StubCache);
        Assert.IsTrue(_mockLogger.ContainsWithExactText(LogLevel.Information,
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
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
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
        mockDatabaseContext
            .Setup(m => m.Query<DbStubModel>(getStubsQuery, null))
            .Returns(dbStubModels);

        // Act
        var result = (await cache.GetOrUpdateStubCacheAsync(mockDatabaseContext.Object)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.AreEqual("stub-2", result[0].Id);
        Assert.AreEqual("stub-1", result[1].Id);
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
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
            .ReturnsAsync(newTrackingId);

        var dbStubModels = new[]
        {
            new DbStubModel {Id = 1, StubId = "stub-1", StubType = "xml", Stub = "XML, BUT NOT SUPPORTED!"}
        };
        mockDatabaseContext
            .Setup(m => m.Query<DbStubModel>(getStubsQuery, null))
            .Returns(dbStubModels);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<NotImplementedException>(() =>
                cache.GetOrUpdateStubCacheAsync(mockDatabaseContext.Object));

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
            .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
            .ReturnsAsync(newTrackingId);

        // Act / Assert
        Assert.IsTrue(
            (await cache.GetOrUpdateStubCacheAsync(mockDatabaseContext.Object))
            .SequenceEqual(await cache.GetOrUpdateStubCacheAsync(mockDatabaseContext.Object)));
    }
}
