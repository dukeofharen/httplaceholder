using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class RelationalDbStubSourceFacts
{
    private readonly Mock<IDatabaseContext> _mockDatabaseContext = new();
    private readonly AutoMocker _mocker = new();

    private readonly SettingsModel _settings = new()
    {
        Storage = new StorageSettingsModel {OldRequestsQueueLength = 100}
    };

    [TestInitialize]
    public void Initialize()
    {
        var mockDatabaseContextFactory = _mocker.GetMock<IDatabaseContextFactory>();
        mockDatabaseContextFactory
            .Setup(m => m.CreateDatabaseContext())
            .Returns(_mockDatabaseContext.Object);
        _mocker.Use(Options.Create(_settings));
    }

    [TestCleanup]
    public void Cleanup() => _mockDatabaseContext.VerifyAll();

    [TestMethod]
    public async Task AddRequestResultAsync_ShouldAddRequestSuccessfully_NoResponse()
    {
        // Arrange
        const string query = "ADD REQUEST QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.AddRequestQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(1);

        var requestResult = new RequestResultModel
        {
            CorrelationId = Guid.NewGuid().ToString(),
            ExecutingStubId = "stub",
            RequestBeginTime = DateTime.Today.AddSeconds(-2),
            RequestEndTime = DateTime.Today
        };

        // Act
        await stubSource.AddRequestResultAsync(requestResult, null, CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedParam);
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(requestResult.CorrelationId, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(requestResult.ExecutingStubId, parsedParam["ExecutingStubId"].ToString());
        Assert.AreEqual(requestResult.RequestBeginTime, DateTime.Parse(parsedParam["RequestBeginTime"].ToString()));
        Assert.AreEqual(requestResult.RequestEndTime, DateTime.Parse(parsedParam["RequestEndTime"].ToString()));
        Assert.AreEqual(JsonConvert.SerializeObject(requestResult), parsedParam["Json"].ToString());

        _mockDatabaseContext.Verify(
            m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object>()), Times.Once());
    }

    [TestMethod]
    public async Task AddRequestResultAsync_ShouldAddRequestSuccessfully_WithResponse()
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();

        const string addRequestQuery = "ADD REQUEST QUERY";

        mockQueryStore
            .Setup(m => m.AddRequestQuery)
            .Returns(addRequestQuery);

        const string addResponseQuery = "ADD REQUEST QUERY";
        mockQueryStore
            .Setup(m => m.AddResponseQuery)
            .Returns(addResponseQuery);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        object capturedAddResponseParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(addRequestQuery, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .ReturnsAsync(1);
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(addResponseQuery, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedAddResponseParam = param)
            .ReturnsAsync(1);

        var requestResult = new RequestResultModel
        {
            CorrelationId = Guid.NewGuid().ToString(),
            ExecutingStubId = "stub",
            RequestBeginTime = DateTime.Today.AddSeconds(-2),
            RequestEndTime = DateTime.Today
        };
        var responseModel = new ResponseModel
        {
            Body = new byte[] {1, 2, 3},
            Headers = {{"Content-Type", "text/plain"}},
            StatusCode = 200,
            BodyIsBinary = true
        };

        // Act
        await stubSource.AddRequestResultAsync(requestResult, responseModel, CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedAddResponseParam);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedAddResponseParam));
        Assert.AreEqual(requestResult.CorrelationId, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(responseModel.StatusCode, (int)parsedParam["StatusCode"]);
        Assert.IsTrue(parsedParam["Headers"].ToString().Contains("text/plain"));
        Assert.AreEqual("AQID", parsedParam["Body"].ToString());
        Assert.IsTrue((bool)parsedParam["BodyIsBinary"]);
    }

    [TestMethod]
    public async Task AddStubAsync_ShouldAddStubSuccessfully()
    {
        // Arrange
        const string query = "ADD STUB QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.AddStubQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(1);

        var stub = new StubModel {Id = "stub-id"};

        // Act
        await stubSource.AddStubAsync(stub, CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedParam);
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(stub.Id, parsedParam["StubId"].ToString());
        Assert.AreEqual(JsonConvert.SerializeObject(stub), parsedParam["Stub"].ToString());
        Assert.AreEqual("json", parsedParam["StubType"].ToString());

        _mocker.GetMock<IRelationalDbStubCache>()
            .Verify(m => m.AddOrReplaceStubAsync(_mockDatabaseContext.Object, stub, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task CleanOldRequestResultsAsync_ShouldCleanOldRequests()
    {
        // Arrange
        const string query = "CLEAN REQUESTS QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.CleanOldRequestsQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(1);

        // Act
        await stubSource.CleanOldRequestResultsAsync(CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedParam);
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual("100", parsedParam["Limit"].ToString());
    }

    [TestMethod]
    public async Task GetRequestResultsOverviewAsync_ShouldReturnRequestsSuccessfully()
    {
        // Arrange
        const string query = "GET REQUESTS QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetRequestsQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        var request1 = new RequestResultModel
        {
            RequestParameters = new RequestParametersModel {Method = "GET", Url = "http://localhost:5000"},
            CorrelationId = Guid.NewGuid().ToString(),
            StubTenant = "tenant-name",
            ExecutingStubId = "stub1",
            RequestBeginTime = DateTime.Today,
            RequestEndTime = DateTime.Today.AddSeconds(2),
            HasResponse = true
        };
        var requests = new[] {new DbRequestModel {Json = JsonConvert.SerializeObject(request1)}};
        _mockDatabaseContext
            .Setup(m => m.QueryAsync<DbRequestModel>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(requests);

        // Act
        var result = (await stubSource.GetRequestResultsOverviewAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var overviewModel = result.Single();
        Assert.AreEqual(request1.RequestParameters.Method, overviewModel.Method);
        Assert.AreEqual(request1.RequestParameters.Url, overviewModel.Url);
        Assert.AreEqual(request1.CorrelationId, overviewModel.CorrelationId);
        Assert.AreEqual(request1.StubTenant, overviewModel.StubTenant);
        Assert.AreEqual(request1.RequestBeginTime, overviewModel.RequestBeginTime);
        Assert.AreEqual(request1.RequestEndTime, overviewModel.RequestEndTime);
        Assert.AreEqual(request1.HasResponse, overviewModel.HasResponse);
    }

    [TestMethod]
    public async Task GetRequestAsync_RequestFound_ShouldReturnRequestSuccessfully()
    {
        // Arrange
        const string query = "GET REQUEST QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetRequestQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        var correlationIdInput = Guid.NewGuid().ToString();
        var request = new DbRequestModel {Json = $@"{{""CorrelationId"": ""{correlationIdInput}""}}"};
        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<DbRequestModel>(query, It.IsAny<CancellationToken>(),
                It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(request);

        // Act
        var result = await stubSource.GetRequestAsync(correlationIdInput, CancellationToken.None);

        // Assert
        Assert.AreEqual(correlationIdInput, result.CorrelationId);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationIdInput, parsedParam["CorrelationId"].ToString());
    }

    [TestMethod]
    public async Task GetRequestAsync_RequestNotFound_ShouldReturnNull()
    {
        // Arrange
        const string query = "GET REQUEST QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetRequestQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        var correlationIdInput = Guid.NewGuid().ToString();
        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<DbRequestModel>(query, It.IsAny<CancellationToken>(),
                It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync((DbRequestModel)null);

        // Act
        var result = await stubSource.GetRequestAsync(correlationIdInput, CancellationToken.None);

        // Assert
        Assert.IsNull(result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationIdInput, parsedParam["CorrelationId"].ToString());
    }

    [TestMethod]
    public async Task GetResponseAsync_ResponseNotFound_ShouldReturnNull()
    {
        // Arrange
        const string query = "GET RESPONSE QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetResponseQuery)
            .Returns(query);

        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<DbResponseModel>(query, It.IsAny<CancellationToken>(),
                It.IsAny<object>()))
            .ReturnsAsync((DbResponseModel)null);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = await stubSource.GetResponseAsync("123", CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetResponseAsync_ResponseFound_ShouldReturnResponse()
    {
        // Arrange
        const string query = "GET RESPONSE QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetResponseQuery)
            .Returns(query);

        var expectedResponse = new DbResponseModel
        {
            Id = 1,
            Body = Convert.ToBase64String(Encoding.UTF8.GetBytes("555")),
            Headers = JsonConvert.SerializeObject(new Dictionary<string, string> {{"Content-Type", "text/plain"}}),
            StatusCode = 200,
            BodyIsBinary = false
        };

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<DbResponseModel>(query, It.IsAny<CancellationToken>(),
                It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(expectedResponse);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = await stubSource.GetResponseAsync("123", CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(capturedParam);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual("123", parsedParam["CorrelationId"].ToString());

        Assert.AreEqual(expectedResponse.StatusCode, result.StatusCode);
        Assert.AreEqual(1, result.Headers.Count);
        Assert.AreEqual("text/plain", result.Headers["Content-Type"]);
        Assert.IsFalse(result.BodyIsBinary);
        Assert.AreEqual("555", Encoding.UTF8.GetString(result.Body));
    }

    [TestMethod]
    public async Task DeleteAllRequestResultsAsync_ShouldDeleteRequestsSuccessfully()
    {
        // Arrange
        const string query = "DELETE REQUESTS QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.DeleteAllRequestsQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        await stubSource.DeleteAllRequestResultsAsync(CancellationToken.None);

        // Assert
        _mockDatabaseContext.Verify(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(), null));
    }

    [TestMethod]
    public async Task DeleteRequestAsync_NoRecordsUpdated_ShouldReturnFalse()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        const string query = "DELETE REQUEST QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.DeleteRequestQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(0);

        // Act
        var result = await stubSource.DeleteRequestAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationId, parsedParam["CorrelationId"].ToString());
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RecordsUpdated_ShouldReturnTrue()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        const string query = "DELETE REQUEST QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.DeleteRequestQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(1);

        // Act
        var result = await stubSource.DeleteRequestAsync(correlationId, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationId, parsedParam["CorrelationId"].ToString());
    }

    [DataTestMethod]
    [DataRow(1, true)]
    [DataRow(0, false)]
    public async Task DeleteStubAsync_ShouldDeleteStub(int numberOfRecordsUpdated, bool expectedResult)
    {
        // Arrange
        const string query = "DELETE STUB QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.DeleteStubQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(numberOfRecordsUpdated);

        const string stubId = "stub";

        // Act
        var result = await stubSource.DeleteStubAsync(stubId, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(stubId, parsedParam["StubId"].ToString());

        _mocker.GetMock<IRelationalDbStubCache>()
            .Verify(m => m.DeleteStubAsync(_mockDatabaseContext.Object, stubId, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task GetRequestResultsAsync_ShouldReturnRequestsSuccessfully()
    {
        // Arrange
        const string query = "GET REQUESTS QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetRequestsQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

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
            .Setup(m => m.QueryAsync<DbRequestModel>(query, It.IsAny<CancellationToken>(), null))
            .ReturnsAsync(requests);

        // Act
        var result = (await stubSource.GetRequestResultsAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("12345", result.Single().CorrelationId);
    }

    [TestMethod]
    public async Task GetStubsAsync_ShouldReturnStubsCorrectly()
    {
        // Arrange
        var stubs = new[] {new StubModel {Id = "stub-id"}};

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(_mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = await stubSource.GetStubsAsync(CancellationToken.None);

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

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(_mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = (await stubSource.GetStubsOverviewAsync(CancellationToken.None)).ToArray();

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
        const string stubId = "stub-id";
        var cachedStubs = new[] {new StubModel {Id = "other-stub-id"}, new StubModel {Id = stubId}};

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(_mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedStubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = await stubSource.GetStubAsync(stubId, CancellationToken.None);

        // Assert
        Assert.AreEqual(cachedStubs[1], result);
    }

    [TestMethod]
    public async Task GetStubAsync_StubNotFound_ShouldReturnNull()
    {
        // Arrange
        var stubs = new[] {new StubModel {Id = "stub-id"}};

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(_mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = await stubSource.GetStubsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(stubs, result);
    }

    [TestMethod]
    public async Task PrepareStubSourceAsync_ShouldPrepareDatabaseAndLocalCache()
    {
        // Arrange
        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        await stubSource.PrepareStubSourceAsync(CancellationToken.None);

        // Assert
        _mocker.GetMock<IRelationalDbMigrator>()
            .Verify(m => m.MigrateAsync(_mockDatabaseContext.Object, It.IsAny<CancellationToken>()));
        _mocker.GetMock<IRelationalDbStubCache>().Verify(m =>
            m.GetOrUpdateStubCacheAsync(_mockDatabaseContext.Object, It.IsAny<CancellationToken>()));
    }
}
