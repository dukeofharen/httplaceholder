using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Persistence.Tests.Db
{
    [TestClass]
    public class RelationalDbStubCacheFacts
    {
        private readonly Mock<IQueryStore> _mockQueryStore = new();
        private readonly Mock<IDatabaseContext> _mockDatabaseContext = new();
        private readonly Mock<ILogger<RelationalDbStubCache>> _mockLogger = new();
        private RelationalDbStubCache _cache;

        [TestInitialize]
        public void Initialize() => _cache = new RelationalDbStubCache(_mockQueryStore.Object, _mockLogger.Object);

        [TestCleanup]
        public void Cleanup()
        {
            _mockQueryStore.VerifyAll();
            _mockDatabaseContext.VerifyAll();
        }

        [TestMethod]
        public void ClearStubCache_ShouldClearCacheAndUpdateTrackingId()
        {
            // Arrange
            var originalId = Guid.NewGuid().ToString();
            _cache.StubUpdateTrackingId = originalId;
            _cache.StubCache = new List<StubModel>();

            var query = "UPDATE TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.UpdateStubUpdateTrackingIdQuery)
                .Returns(query);

            object capturedParam = null;
            _mockDatabaseContext
                .Setup(m => m.Execute(query, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedParam = param);

            // Act
            _cache.ClearStubCache(_mockDatabaseContext.Object);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(_cache.StubUpdateTrackingId));
            Assert.IsFalse(originalId == _cache.StubUpdateTrackingId);
            Assert.IsNull(_cache.StubCache);

            Assert.IsNotNull(capturedParam);

            var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
            Assert.AreEqual(_cache.StubUpdateTrackingId, parsedParam["StubUpdateTrackingId"].ToString());
        }

        [TestMethod]
        public async Task GetOrUpdateStubCache_TrackingIdNotSetYet_ShouldInitCache()
        {
            // Arrange
            var query = "GET TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubUpdateTrackingIdQuery)
                .Returns(query);

            var insertIdQuery = "INSERT TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.InsertStubUpdateTrackingIdQuery)
                .Returns(insertIdQuery);

            _mockDatabaseContext
                .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
                .ReturnsAsync(string.Empty);

            object capturedInsertParam = null;
            _mockDatabaseContext
                .Setup(m => m.Execute(insertIdQuery, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedInsertParam = param);

            // Act
            var result = await _cache.GetOrUpdateStubCache(_mockDatabaseContext.Object);

            // Assert
            Assert.IsNotNull(capturedInsertParam);

            var parsedCapturedInsertParam = JObject.Parse(JsonConvert.SerializeObject(capturedInsertParam));
            Assert.AreEqual(_cache.StubUpdateTrackingId, parsedCapturedInsertParam["StubUpdateTrackingId"].ToString());

            Assert.IsFalse(string.IsNullOrWhiteSpace(_cache.StubUpdateTrackingId));
            Assert.IsNotNull(result);
            Assert.IsNotNull(_cache.StubCache);
        }

        [TestMethod]
        public async Task GetOrUpdateStubCache_StubCacheIsNull_ShouldInitCache()
        {
            // Arrange
            var query = "GET TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubUpdateTrackingIdQuery)
                .Returns(query);

            var trackingId = Guid.NewGuid().ToString();
            _mockDatabaseContext
                .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
                .ReturnsAsync(trackingId);

            // Act
            var result = await _cache.GetOrUpdateStubCache(_mockDatabaseContext.Object);

            // Assert
            Assert.AreEqual(trackingId, _cache.StubUpdateTrackingId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(_cache.StubCache);
        }

        [TestMethod]
        public async Task GetOrUpdateStubCache_TrackingIdHasChanged_ShouldInitCache()
        {
            // Arrange
            var oldTrackingId = Guid.NewGuid().ToString();
            _cache.StubUpdateTrackingId = oldTrackingId;

            var query = "GET TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubUpdateTrackingIdQuery)
                .Returns(query);

            var newTrackingId = Guid.NewGuid().ToString();
            _mockDatabaseContext
                .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
                .ReturnsAsync(newTrackingId);

            // Act
            var result = await _cache.GetOrUpdateStubCache(_mockDatabaseContext.Object);

            // Assert
            Assert.AreEqual(newTrackingId, _cache.StubUpdateTrackingId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(_cache.StubCache);
        }

        [TestMethod]
        public async Task GetOrUpdateStubCache_VerifyStubParsing()
        {
            // Arrange
            var oldTrackingId = Guid.NewGuid().ToString();
            _cache.StubUpdateTrackingId = oldTrackingId;

            var query = "GET TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubUpdateTrackingIdQuery)
                .Returns(query);

            var getStubsQuery = "GET STUBS QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubsQuery)
                .Returns(getStubsQuery);

            var newTrackingId = Guid.NewGuid().ToString();
            _mockDatabaseContext
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
            _mockDatabaseContext
                .Setup(m => m.Query<DbStubModel>(getStubsQuery, null))
                .Returns(dbStubModels);

            // Act
            var result = (await _cache.GetOrUpdateStubCache(_mockDatabaseContext.Object)).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual("stub-1", result[0].Id);
            Assert.AreEqual("stub-2", result[1].Id);
        }

        [TestMethod]
        public async Task GetOrUpdateStubCache_UnknownStubType_ShouldThrowNotImplementedException()
        {
            // Arrange
            var oldTrackingId = Guid.NewGuid().ToString();
            _cache.StubUpdateTrackingId = oldTrackingId;

            var query = "GET TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubUpdateTrackingIdQuery)
                .Returns(query);

            var getStubsQuery = "GET STUBS QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubsQuery)
                .Returns(getStubsQuery);

            var newTrackingId = Guid.NewGuid().ToString();
            _mockDatabaseContext
                .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
                .ReturnsAsync(newTrackingId);

            var dbStubModels = new[]
            {
                new DbStubModel {Id = 1, StubId = "stub-1", StubType = "xml", Stub = "XML, BUT NOT SUPPORTED!"}
            };
            _mockDatabaseContext
                .Setup(m => m.Query<DbStubModel>(getStubsQuery, null))
                .Returns(dbStubModels);

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<NotImplementedException>(() =>
                    _cache.GetOrUpdateStubCache(_mockDatabaseContext.Object));

            // Assert
            Assert.AreEqual("StubType 'xml' not supported: stub 'stub-1'.", exception.Message);
        }

        [TestMethod]
        public async Task GetOrUpdateStubCache_CallingMethodSeveralTimesShouldReturnSameResult()
        {
            // Arrange
            var oldTrackingId = Guid.NewGuid().ToString();
            _cache.StubUpdateTrackingId = oldTrackingId;

            var query = "GET TRACKING ID QUERY";
            _mockQueryStore
                .Setup(m => m.GetStubUpdateTrackingIdQuery)
                .Returns(query);

            var newTrackingId = Guid.NewGuid().ToString();
            _mockDatabaseContext
                .Setup(m => m.QueryFirstOrDefaultAsync<string>(query, null))
                .ReturnsAsync(newTrackingId);

            // Act / Assert
            Assert.AreEqual(
                await _cache.GetOrUpdateStubCache(_mockDatabaseContext.Object),
                await _cache.GetOrUpdateStubCache(_mockDatabaseContext.Object));
        }
    }
}
