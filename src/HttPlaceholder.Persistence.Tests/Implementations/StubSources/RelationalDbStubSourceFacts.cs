using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources
{
    [TestClass]
    public class RelationalDbStubSourceFacts
    {
        private readonly SettingsModel _settings = new()
        {
            Storage = new StorageSettingsModel {OldRequestsQueueLength = 100}
        };

        private readonly Mock<IQueryStore> _mockQueryStore = new();
        private readonly Mock<IDatabaseContext> _mockDatabaseContext = new();

        private readonly Mock<IDatabaseContextFactory>
            _mockDatabaseContextFactory = new();

        private readonly Mock<IRelationalDbStubCache> _mockRelationalDbStubCache = new();

        private RelationalDbStubSource _stubSource;

        [TestInitialize]
        public void Initialize()
        {
            _mockDatabaseContextFactory
                .Setup(m => m.CreateDatabaseContext())
                .Returns(_mockDatabaseContext.Object);
            _stubSource = new RelationalDbStubSource(
                Options.Create(_settings),
                _mockQueryStore.Object,
                _mockDatabaseContextFactory.Object,
                _mockRelationalDbStubCache.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _mockQueryStore.VerifyAll();
            _mockDatabaseContext.VerifyAll();
            _mockDatabaseContextFactory.VerifyAll();
            _mockRelationalDbStubCache.VerifyAll();
        }

        [TestMethod]
        public async Task AddRequestResultAsync_ShouldAddRequestSuccessfully()
        {
            // Arrange
            var query = "ADD REQUEST QUERY";
            _mockQueryStore
                .Setup(m => m.AddRequestQuery)
                .Returns(query);

            object capturedParam = null;
            _mockDatabaseContext
                .Setup(m => m.ExecuteAsync(query, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedParam = param)
                .ReturnsAsync(1);

            var requestResult = new RequestResultModel
            {
                CorrelationId = Guid.NewGuid().ToString(),
                ExecutingStubId = "stub",
                RequestBeginTime = DateTime.Today.AddSeconds(-2),
                RequestEndTime = DateTime.Today
            };

            // Act
            await _stubSource.AddRequestResultAsync(requestResult);

            // Assert
            Assert.IsNotNull(capturedParam);
            var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
            Assert.AreEqual(requestResult.CorrelationId, parsedParam["CorrelationId"].ToString());
            Assert.AreEqual(requestResult.ExecutingStubId, parsedParam["ExecutingStubId"].ToString());
            Assert.AreEqual(requestResult.RequestBeginTime, DateTime.Parse(parsedParam["RequestBeginTime"].ToString()));
            Assert.AreEqual(requestResult.RequestEndTime, DateTime.Parse(parsedParam["RequestEndTime"].ToString()));
            Assert.AreEqual(JsonConvert.SerializeObject(requestResult), parsedParam["Json"].ToString());
        }

        [TestMethod]
        public async Task AddStubAsync_ShouldAddStubSuccessfully()
        {
            // Arrange
            var query = "ADD STUB QUERY";
            _mockQueryStore
                .Setup(m => m.AddStubQuery)
                .Returns(query);

            object capturedParam = null;
            _mockDatabaseContext
                .Setup(m => m.ExecuteAsync(query, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedParam = param)
                .ReturnsAsync(1);

            var stub = new StubModel {Id = "stub-id"};

            // Act
            await _stubSource.AddStubAsync(stub);

            // Assert
            Assert.IsNotNull(capturedParam);
            var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
            Assert.AreEqual(stub.Id, parsedParam["StubId"].ToString());
            Assert.AreEqual(JsonConvert.SerializeObject(stub), parsedParam["Stub"].ToString());
            Assert.AreEqual("json", parsedParam["StubType"].ToString());

            _mockRelationalDbStubCache.Verify(m => m.ClearStubCache(_mockDatabaseContext.Object));
        }

        [TestMethod]
        public async Task CleanOldRequestResultsAsync_ShouldCleanOldRequests()
        {
            // Arrange
            var query = "CLEAN REQUESTS QUERY";
            _mockQueryStore
                .Setup(m => m.CleanOldRequestsQuery)
                .Returns(query);

            object capturedParam = null;
            _mockDatabaseContext
                .Setup(m => m.ExecuteAsync(query, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedParam = param)
                .ReturnsAsync(1);

            // Act
            await _stubSource.CleanOldRequestResultsAsync();

            // Assert
            Assert.IsNotNull(capturedParam);
            var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
            Assert.AreEqual("100", parsedParam["Limit"].ToString());
        }

        [TestMethod]
        public async Task GetRequestResultsOverviewAsync_ShouldReturnRequestsSuccessfully()
        {
            // Arrange
            var query = "GET REQUESTS QUERY";
            _mockQueryStore
                .Setup(m => m.GetRequestsQuery)
                .Returns(query);

            var request1 = new RequestResultModel
            {
                RequestParameters = new RequestParametersModel {Method = "GET", Url = "http://localhost:5000",},
                CorrelationId = Guid.NewGuid().ToString(),
                StubTenant = "tenant-name",
                ExecutingStubId = "stub1",
                RequestBeginTime = DateTime.Today,
                RequestEndTime = DateTime.Today.AddSeconds(2)
            };
            var requests = new[] {new DbRequestModel {Json = JsonConvert.SerializeObject(request1)}};
            _mockDatabaseContext
                .Setup(m => m.QueryAsync<DbRequestModel>(query, null))
                .ReturnsAsync(requests);

            // Act
            var result = (await _stubSource.GetRequestResultsOverviewAsync()).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);

            var overviewModel = result.Single();
            Assert.AreEqual(request1.RequestParameters.Method, overviewModel.Method);
            Assert.AreEqual(request1.RequestParameters.Url, overviewModel.Url);
            Assert.AreEqual(request1.CorrelationId, overviewModel.CorrelationId);
            Assert.AreEqual(request1.StubTenant, overviewModel.StubTenant);
            Assert.AreEqual(request1.RequestBeginTime, overviewModel.RequestBeginTime);
            Assert.AreEqual(request1.RequestEndTime, overviewModel.RequestEndTime);
        }

        [TestMethod]
        public async Task GetRequestAsync_RequestFound_ShouldReturnRequestSuccessfully()
        {
            // Arrange
            var query = "GET REQUEST QUERY";
            _mockQueryStore
                .Setup(m => m.GetRequestQuery)
                .Returns(query);

            var correlationIdInput = Guid.NewGuid().ToString();
            var request = new DbRequestModel {Json = $@"{{""CorrelationId"": ""{correlationIdInput}""}}"};
            object capturedParam = null;
            _mockDatabaseContext
                .Setup(m => m.QueryFirstOrDefaultAsync<DbRequestModel>(query, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedParam = param)
                .ReturnsAsync(request);

            // Act
            var result = await _stubSource.GetRequestAsync(correlationIdInput);

            // Assert
            Assert.AreEqual(correlationIdInput, result.CorrelationId);

            var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
            Assert.AreEqual(correlationIdInput, parsedParam["CorrelationId"].ToString());
        }

        [TestMethod]
        public async Task GetRequestAsync_RequestNotFound_ShouldReturnNull()
        {
            // Arrange
            var query = "GET REQUEST QUERY";
            _mockQueryStore
                .Setup(m => m.GetRequestQuery)
                .Returns(query);

            var correlationIdInput = Guid.NewGuid().ToString();
            object capturedParam = null;
            _mockDatabaseContext
                .Setup(m => m.QueryFirstOrDefaultAsync<DbRequestModel>(query, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedParam = param)
                .ReturnsAsync((DbRequestModel)null);

            // Act
            var result = await _stubSource.GetRequestAsync(correlationIdInput);

            // Assert
            Assert.IsNull(result);

            var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
            Assert.AreEqual(correlationIdInput, parsedParam["CorrelationId"].ToString());
        }

        [TestMethod]
        public async Task DeleteAllRequestResultsAsync_ShouldDeleteRequestsSuccessfully()
        {
            // Arrange
            var query = "DELETE REQUESTS QUERY";
            _mockQueryStore
                .Setup(m => m.DeleteAllRequestsQuery)
                .Returns(query);

            // Act
            await _stubSource.DeleteAllRequestResultsAsync();

            // Assert
            _mockDatabaseContext.Verify(m => m.ExecuteAsync(query, null));
        }

        [DataTestMethod]
        [DataRow(1, true)]
        [DataRow(0, false)]
        public async Task DeleteStubAsync_ShouldDeleteStub(int numberOfRecordsUpdated, bool expectedResult)
        {
            // Arrange
            var query = "DELETE STUB QUERY";
            _mockQueryStore
                .Setup(m => m.DeleteStubQuery)
                .Returns(query);

            object capturedParam = null;
            _mockDatabaseContext
                .Setup(m => m.ExecuteAsync(query, It.IsAny<object>()))
                .Callback<string, object>((_, param) => capturedParam = param)
                .ReturnsAsync(numberOfRecordsUpdated);

            var stubId = "stub";

            // Act
            var result = await _stubSource.DeleteStubAsync(stubId);

            // Assert
            Assert.AreEqual(expectedResult, result);

            var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
            Assert.AreEqual(stubId, parsedParam["StubId"].ToString());

            _mockRelationalDbStubCache.Verify(m => m.ClearStubCache(_mockDatabaseContext.Object));
        }

        [TestMethod]
        public async Task GetRequestResultsAsync_ShouldReturnRequestsSuccessfully()
        {
            // Arrange
            var query = "GET REQUESTS QUERY";
            _mockQueryStore
                .Setup(m => m.GetRequestsQuery)
                .Returns(query);

            var requests = new[]
            {
                new DbRequestModel
                {
                    Id = 1,
                    Json = @"{""CorrelationId"": ""12345""}",
                    CorrelationId = Guid.NewGuid().ToString(),
                    ExecutingStubId = "stub1",
                    RequestBeginTime = DateTime.Today,
                    RequestEndTime = DateTime.Today.AddSeconds(2)
                }
            };
            _mockDatabaseContext
                .Setup(m => m.QueryAsync<DbRequestModel>(query, null))
                .ReturnsAsync(requests);

            // Act
            var result = (await _stubSource.GetRequestResultsAsync()).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("12345", result.Single().CorrelationId);
        }

        [TestMethod]
        public async Task GetStubsAsync_ShouldReturnStubsCorrectly()
        {
            // Arrange
            var stubs = new[] {new StubModel {Id = "stub-id"}};

            _mockRelationalDbStubCache
                .Setup(m => m.GetOrUpdateStubCache(_mockDatabaseContext.Object))
                .ReturnsAsync(stubs);

            // Act
            var result = await _stubSource.GetStubsAsync();

            // Assert
            Assert.AreEqual(stubs, result);
        }

        [TestMethod]
        public async Task GetStubsOverviewAsync_ShouldReturnStubsCorrectly()
        {
            // Arrange
            var stubs = new[]
            {
                new StubModel {Id = "stub-id1", Tenant = "tenant1", Enabled = true},
                new StubModel {Id = "stub-id2", Tenant = "tenant2", Enabled = false}
            };

            _mockRelationalDbStubCache
                .Setup(m => m.GetOrUpdateStubCache(_mockDatabaseContext.Object))
                .ReturnsAsync(stubs);

            // Act
            var result = (await _stubSource.GetStubsOverviewAsync()).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual(stubs[0].Id, result[0].Id);
            Assert.AreEqual(stubs[0].Tenant, result[0].Tenant);
            Assert.AreEqual(stubs[0].Enabled, result[0].Enabled);

            Assert.AreEqual(stubs[1].Id, result[1].Id);
            Assert.AreEqual(stubs[1].Tenant, result[1].Tenant);
            Assert.AreEqual(stubs[1].Enabled, result[1].Enabled);
        }

        [TestMethod]
        public async Task GetStubAsync_StubFound_ShouldReturnStub()
        {
            // Arrange
            var stubId = "stub-id";
            var cachedStubs = new[] {new StubModel {Id = "other-stub-id"}, new StubModel {Id = stubId}};

            _mockRelationalDbStubCache
                .Setup(m => m.GetOrUpdateStubCache(_mockDatabaseContext.Object))
                .ReturnsAsync(cachedStubs);

            // Act
            var result = await _stubSource.GetStubAsync(stubId);

            // Assert
            Assert.AreEqual(cachedStubs[1], result);
        }

        [TestMethod]
        public async Task GetStubAsync_StubNotFound_ShouldReturnNull()
        {
            // Arrange
            var stubs = new[] {new StubModel {Id = "stub-id"}};

            _mockRelationalDbStubCache
                .Setup(m => m.GetOrUpdateStubCache(_mockDatabaseContext.Object))
                .ReturnsAsync(stubs);

            // Act
            var result = await _stubSource.GetStubsAsync();

            // Assert
            Assert.AreEqual(stubs, result);
        }

        [TestMethod]
        public async Task PrepareStubSourceAsync_ShouldPrepareDatabase()
        {
            // Arrange
            var query = "PREPARE STUB SOURCE QUERY";
            _mockQueryStore
                .Setup(m => m.MigrationsQuery)
                .Returns(query);

            // Act
            await _stubSource.PrepareStubSourceAsync();

            // Assert
            _mockDatabaseContext.Verify(m => m.ExecuteAsync(query, null));
        }
    }
}
