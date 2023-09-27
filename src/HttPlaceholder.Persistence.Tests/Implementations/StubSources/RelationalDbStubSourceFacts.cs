using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class RelationalDbStubSourceFacts
{
    private const string DistributionKey = "username";
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
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
    }

    [TestCleanup]
    public void Cleanup() => _mockDatabaseContext.VerifyAll();

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddRequestResultAsync_ShouldAddRequestSuccessfully_NoResponse(bool withDistributionKey)
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
        await stubSource.AddRequestResultAsync(requestResult, null, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedParam);
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(requestResult.CorrelationId, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(requestResult.ExecutingStubId, parsedParam["ExecutingStubId"].ToString());
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
        Assert.AreEqual(requestResult.RequestBeginTime, DateTime.Parse(parsedParam["RequestBeginTime"].ToString()));
        Assert.AreEqual(requestResult.RequestEndTime, DateTime.Parse(parsedParam["RequestEndTime"].ToString()));
        Assert.AreEqual(JsonConvert.SerializeObject(requestResult), parsedParam["Json"].ToString());

        _mockDatabaseContext.Verify(
            m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<object>()), Times.Once());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddRequestResultAsync_ShouldAddRequestSuccessfully_WithResponse(bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();

        const string addRequestQuery = "ADD REQUEST QUERY";

        mockQueryStore
            .Setup(m => m.AddRequestQuery)
            .Returns(addRequestQuery);

        const string addResponseQuery = "ADD RESPONSE QUERY";
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
            Headers = {{HeaderKeys.ContentType, "text/plain"}},
            StatusCode = 200,
            BodyIsBinary = true
        };

        // Act
        await stubSource.AddRequestResultAsync(requestResult, responseModel,
            withDistributionKey ? DistributionKey : null, CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedAddResponseParam);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedAddResponseParam));
        Assert.AreEqual(requestResult.CorrelationId, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(responseModel.StatusCode, (int)parsedParam["StatusCode"]);
        Assert.IsTrue(parsedParam["Headers"].ToString().Contains("text/plain"));
        Assert.AreEqual("AQID", parsedParam["Body"].ToString());
        Assert.IsTrue((bool)parsedParam["BodyIsBinary"]);
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddStubAsync_ShouldAddStubSuccessfully(bool withDistributionKey)
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
        await stubSource.AddStubAsync(stub, withDistributionKey ? DistributionKey : null, CancellationToken.None);

        // Assert
        Assert.IsNotNull(capturedParam);
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(stub.Id, parsedParam["StubId"].ToString());
        Assert.AreEqual(JsonConvert.SerializeObject(stub), parsedParam["Stub"].ToString());
        Assert.AreEqual("json", parsedParam["StubType"].ToString());
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());

        _mocker.GetMock<IRelationalDbStubCache>()
            .Verify(m => m.AddOrReplaceStubAsync(_mockDatabaseContext.Object, stub, It.IsAny<CancellationToken>()),
                withDistributionKey ? Times.Never : Times.Once);
    }

    [TestMethod]
    public async Task CleanOldRequestResultsAsync_ShouldCleanOldRequests()
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();

        const string distinctKeysQuery = "GET DISTINCT KEYS";
        mockQueryStore
            .Setup(m => m.GetDistinctRequestDistributionKeysQuery)
            .Returns(distinctKeysQuery);

        const string query = "CLEAN REQUESTS QUERY";
        mockQueryStore
            .Setup(m => m.CleanOldRequestsQuery)
            .Returns(query);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        var keys = new[] {"", "key1"};
        _mockDatabaseContext
            .Setup(m => m.QueryAsync<string>(distinctKeysQuery, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .ReturnsAsync(keys);

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
        Assert.IsTrue(keys.Contains(parsedParam["DistributionKey"].ToString()));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestAsync_RequestFound_ShouldReturnRequestSuccessfully(bool withDistributionKey)
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
        var result = await stubSource.GetRequestAsync(correlationIdInput, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.AreEqual(correlationIdInput, result.CorrelationId);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationIdInput, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestAsync_RequestNotFound_ShouldReturnNull(bool withDistributionKey)
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
        var result = await stubSource.GetRequestAsync(correlationIdInput, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.IsNull(result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationIdInput, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetResponseAsync_ResponseNotFound_ShouldReturnNull(bool withDistributionKey)
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
        var result = await stubSource.GetResponseAsync("123", withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetResponseAsync_ResponseFound_ShouldReturnResponse(bool withDistributionKey)
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
            Headers = JsonConvert.SerializeObject(
                new Dictionary<string, string> {{HeaderKeys.ContentType, "text/plain"}}),
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
        var result = await stubSource.GetResponseAsync("123", withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(capturedParam);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual("123", parsedParam["CorrelationId"].ToString());

        Assert.AreEqual(expectedResponse.StatusCode, result.StatusCode);
        Assert.AreEqual(1, result.Headers.Count);
        Assert.AreEqual("text/plain", result.Headers[HeaderKeys.ContentType]);
        Assert.IsFalse(result.BodyIsBinary);
        Assert.AreEqual("555", Encoding.UTF8.GetString(result.Body));
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteAllRequestResultsAsync_ShouldDeleteRequestsSuccessfully(bool withDistributionKey)
    {
        // Arrange
        const string query = "DELETE REQUESTS QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.DeleteAllRequestsQuery)
            .Returns(query);

        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(),
                It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        await stubSource.DeleteAllRequestResultsAsync(withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteRequestAsync_NoRecordsUpdated_ShouldReturnFalse(bool withDistributionKey)
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
        var result = await stubSource.DeleteRequestAsync(correlationId, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.IsFalse(result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationId, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteRequestAsync_RecordsUpdated_ShouldReturnTrue(bool withDistributionKey)
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
        var result = await stubSource.DeleteRequestAsync(correlationId, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(correlationId, parsedParam["CorrelationId"].ToString());
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(1, true, true)]
    [DataRow(0, true, false)]
    [DataRow(1, false, true)]
    [DataRow(0, false, false)]
    public async Task DeleteStubAsync_ShouldDeleteStub(int numberOfRecordsUpdated, bool withDistributionKey,
        bool expectedResult)
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
        var result = await stubSource.DeleteStubAsync(stubId, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        Assert.AreEqual(stubId, parsedParam["StubId"].ToString());
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());

        _mocker.GetMock<IRelationalDbStubCache>()
            .Verify(m => m.DeleteStubAsync(_mockDatabaseContext.Object, stubId, It.IsAny<CancellationToken>()),
                withDistributionKey ? Times.Never : Times.Once);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestResultsAsync_ShouldReturnRequestsSuccessfully(bool withDistributionKey)
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
            .Setup(m => m.QueryAsync<DbRequestModel>(query, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .ReturnsAsync(requests);

        // Act
        var result =
            (await stubSource.GetRequestResultsAsync(null, withDistributionKey ? DistributionKey : null,
                CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("12345", result.Single().CorrelationId);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestResultsAsync_FromIdentifierSet_ShouldReturnRequestsSuccessfully(
        bool withDistributionKey)
    {
        // Arrange
        const string correlationIdsQuery = "GET CORRELATION IDS QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetPagedRequestCorrelationIdsQuery)
            .Returns(correlationIdsQuery);

        const string requestsByCorrelationIdsQuery = "GET REQUESTS BY CORRELATION IDS QUERY";
        mockQueryStore
            .Setup(m => m.GetRequestsByCorrelationIdsQuery)
            .Returns(requestsByCorrelationIdsQuery);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        var ids = new[] {Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
        _mockDatabaseContext
            .Setup(m => m.QueryAsync<string>(correlationIdsQuery, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .ReturnsAsync(ids);

        var requests = new[]
        {
            new DbRequestModel
            {
                Id = 1,
                Json = @"{""CorrelationId"": ""12345""}",
                CorrelationId = ids[1],
                ExecutingStubId = "stub1",
                RequestBeginTime = DateTime.Today,
                RequestEndTime = DateTime.Today.AddSeconds(2)
            }
        };
        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.QueryAsync<DbRequestModel>(requestsByCorrelationIdsQuery, It.IsAny<CancellationToken>(),
                It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(requests);

        // Act
        var result =
            (await stubSource.GetRequestResultsAsync(new PagingModel {FromIdentifier = ids[1]},
                withDistributionKey ? DistributionKey : null, CancellationToken.None))
            .ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("12345", result.Single().CorrelationId);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        var capturedCorrelationIds = parsedParam.SelectToken("$.CorrelationIds").ToObject<string[]>();

        Assert.AreEqual(1, capturedCorrelationIds.Length);
        Assert.AreEqual(ids[1], capturedCorrelationIds[0]);
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestResultsAsync_FromIdentifierAndItemsPerPageSet_ShouldReturnRequestsSuccessfully(
        bool withDistributionKey)
    {
        // Arrange
        const string correlationIdsQuery = "GET CORRELATION IDS QUERY";
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        mockQueryStore
            .Setup(m => m.GetPagedRequestCorrelationIdsQuery)
            .Returns(correlationIdsQuery);

        const string requestsByCorrelationIdsQuery = "GET REQUESTS BY CORRELATION IDS QUERY";
        mockQueryStore
            .Setup(m => m.GetRequestsByCorrelationIdsQuery)
            .Returns(requestsByCorrelationIdsQuery);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        var ids = new[]
        {
            Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        };
        _mockDatabaseContext
            .Setup(m => m.QueryAsync<string>(correlationIdsQuery, It.IsAny<CancellationToken>(), It.IsAny<object>()))
            .ReturnsAsync(ids);

        var requests = new[]
        {
            new DbRequestModel
            {
                Id = 1,
                Json = @"{""CorrelationId"": ""12345""}",
                CorrelationId = ids[1],
                ExecutingStubId = "stub1",
                RequestBeginTime = DateTime.Today,
                RequestEndTime = DateTime.Today.AddSeconds(2)
            },
            new DbRequestModel
            {
                Id = 2,
                Json = @"{""CorrelationId"": ""54321""}",
                CorrelationId = ids[2],
                ExecutingStubId = "stub1",
                RequestBeginTime = DateTime.Today,
                RequestEndTime = DateTime.Today.AddSeconds(2)
            }
        };
        object capturedParam = null;
        _mockDatabaseContext
            .Setup(m => m.QueryAsync<DbRequestModel>(requestsByCorrelationIdsQuery, It.IsAny<CancellationToken>(),
                It.IsAny<object>()))
            .Callback<string, CancellationToken, object>((_, _, param) => capturedParam = param)
            .ReturnsAsync(requests);

        // Act
        var result =
            (await stubSource.GetRequestResultsAsync(new PagingModel {FromIdentifier = ids[1], ItemsPerPage = 2},
                withDistributionKey ? DistributionKey : null,
                CancellationToken.None))
            .ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(capturedParam));
        var capturedCorrelationIds = parsedParam.SelectToken("$.CorrelationIds").ToObject<string[]>();

        Assert.AreEqual(2, capturedCorrelationIds.Length);
        Assert.AreEqual(ids[1], capturedCorrelationIds[0]);
        Assert.AreEqual(ids[2], capturedCorrelationIds[1]);
        Assert.AreEqual(withDistributionKey ? DistributionKey : string.Empty,
            parsedParam["DistributionKey"].ToString());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubsAsync_ShouldReturnStubsCorrectly(bool withDistributionKey)
    {
        // Arrange
        var stubs = new[] {new StubModel {Id = "stub-id"}};

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(withDistributionKey ? DistributionKey : string.Empty,
                _mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result =
            await stubSource.GetStubsAsync(withDistributionKey ? DistributionKey : null, CancellationToken.None);

        // Assert
        Assert.AreEqual(stubs, result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubsOverviewAsync_ShouldReturnStubsCorrectly(bool withDistributionKey)
    {
        // Arrange
        var stubs = new[]
        {
            new StubModel {Id = "stub-id1", Tenant = "tenant1", Enabled = true},
            new StubModel {Id = "stub-id2", Tenant = "tenant2", Enabled = false}
        };

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(withDistributionKey ? DistributionKey : string.Empty,
                _mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result =
            (await stubSource.GetStubsOverviewAsync(withDistributionKey ? DistributionKey : null,
                CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.AreEqual(stubs[0].Id, result[0].Id);
        Assert.AreEqual(stubs[0].Tenant, result[0].Tenant);
        Assert.AreEqual(stubs[0].Enabled, result[0].Enabled);

        Assert.AreEqual(stubs[1].Id, result[1].Id);
        Assert.AreEqual(stubs[1].Tenant, result[1].Tenant);
        Assert.AreEqual(stubs[1].Enabled, result[1].Enabled);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubAsync_StubFound_ShouldReturnStub(bool withDistributionKey)
    {
        // Arrange
        const string stubId = "stub-id";
        var cachedStubs = new[] {new StubModel {Id = "other-stub-id"}, new StubModel {Id = stubId}};

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(withDistributionKey ? DistributionKey : string.Empty,
                _mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedStubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = await stubSource.GetStubAsync(stubId, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.AreEqual(cachedStubs[1], result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubAsync_StubNotFound_ShouldReturnNull(bool withDistributionKey)
    {
        // Arrange
        var stubs = new[] {new StubModel {Id = "stub-id"}};

        var mockRelationalDbStubCache = _mocker.GetMock<IRelationalDbStubCache>();
        mockRelationalDbStubCache
            .Setup(m => m.GetOrUpdateStubCacheAsync(withDistributionKey ? DistributionKey : string.Empty,
                _mockDatabaseContext.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubs);

        var stubSource = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result =
            await stubSource.GetStubsAsync(withDistributionKey ? DistributionKey : null, CancellationToken.None);

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
            m.GetOrUpdateStubCacheAsync(string.Empty, _mockDatabaseContext.Object, It.IsAny<CancellationToken>()));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetScenarioAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string scenario = "scenario-1";

        const string query = "GET SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.GetScenarioQuery)
            .Returns(query);

        var expectedResult = new ScenarioStateModel();
        var captured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<ScenarioStateModel>(query, It.IsAny<CancellationToken>(),
                Capture.In(captured)))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await source.GetScenarioAsync(scenario, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);

        var input = captured.Single();
        CheckDbParam(input, "$.Scenario", scenario);
        CheckDbParam(input, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddScenarioAsync_ScenarioExists_ShouldThrowInvalidOperationException(bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string scenario = "scenario-1";

        const string query = "GET SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.GetScenarioQuery)
            .Returns(query);

        var expectedResult = new ScenarioStateModel();
        var captured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<ScenarioStateModel>(query, It.IsAny<CancellationToken>(),
                Capture.In(captured)))
            .ReturnsAsync(expectedResult);

        // Act
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            source.AddScenarioAsync(scenario, new ScenarioStateModel(), withDistributionKey ? DistributionKey : null,
                CancellationToken.None));

        // Assert
        Assert.AreEqual($"Scenario state with key '{scenario}' already exists.", exception.Message);

        var input = captured.Single();
        CheckDbParam(input, "$.Scenario", scenario);
        CheckDbParam(input, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddScenarioAsync_ScenarioDoesntExist_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string scenario = "scenario-1";

        const string getQuery = "GET SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.GetScenarioQuery)
            .Returns(getQuery);

        const string addQuery = "ADD SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.AddScenarioQuery)
            .Returns(addQuery);

        var getCaptured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<ScenarioStateModel>(getQuery, It.IsAny<CancellationToken>(),
                Capture.In(getCaptured)))
            .ReturnsAsync((ScenarioStateModel)null);

        var addCaptured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(addQuery, It.IsAny<CancellationToken>(), Capture.In(addCaptured)))
            .ReturnsAsync(1);

        var input = new ScenarioStateModel(scenario) {State = Guid.NewGuid().ToString(), HitCount = 11};

        // Act
        var result = await source.AddScenarioAsync(scenario, input,
            withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.AreEqual(input, result);

        var getInput = getCaptured.Single();
        CheckDbParam(getInput, "$.Scenario", scenario);
        CheckDbParam(getInput, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);

        var addInput = addCaptured.Single();
        CheckDbParam(addInput, "$.Scenario", scenario);
        CheckDbParam(addInput, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
        CheckDbParam(addInput, "$.State", input.State);
        CheckDbParam(addInput, "$.HitCount", input.HitCount);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task UpdateScenarioAsync_ScenarioDoesntExist_ShouldThrowInvalidOperationException(
        bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string scenario = "scenario-1";

        const string query = "GET SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.GetScenarioQuery)
            .Returns(query);

        var captured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<ScenarioStateModel>(query, It.IsAny<CancellationToken>(),
                Capture.In(captured)))
            .ReturnsAsync((ScenarioStateModel)null);

        // Act
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            source.UpdateScenarioAsync(scenario, new ScenarioStateModel(), withDistributionKey ? DistributionKey : null,
                CancellationToken.None));

        // Assert
        Assert.AreEqual($"Scenario state with key '{scenario}' not found.", exception.Message);

        var input = captured.Single();
        CheckDbParam(input, "$.Scenario", scenario);
        CheckDbParam(input, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task UpdateScenarioAsync_ScenarioExists_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string scenario = "scenario-1";

        const string getQuery = "GET SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.GetScenarioQuery)
            .Returns(getQuery);

        const string updateQuery = "UPDATE SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.UpdateScenarioQuery)
            .Returns(updateQuery);

        var getCaptured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.QueryFirstOrDefaultAsync<ScenarioStateModel>(getQuery, It.IsAny<CancellationToken>(),
                Capture.In(getCaptured)))
            .ReturnsAsync(new ScenarioStateModel());

        var updateCaptured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(updateQuery, It.IsAny<CancellationToken>(), Capture.In(updateCaptured)))
            .ReturnsAsync(1);

        var input = new ScenarioStateModel(scenario) {State = Guid.NewGuid().ToString(), HitCount = 11};

        // Act
        await source.UpdateScenarioAsync(scenario, input,
            withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        var getInput = getCaptured.Single();
        CheckDbParam(getInput, "$.Scenario", scenario);
        CheckDbParam(getInput, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);

        var addInput = updateCaptured.Single();
        CheckDbParam(addInput, "$.Scenario", scenario);
        CheckDbParam(addInput, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
        CheckDbParam(addInput, "$.State", input.State);
        CheckDbParam(addInput, "$.HitCount", input.HitCount);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetAllScenariosAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string query = "GET ALL SCENARIOS QUERY";
        mockQueryStore
            .Setup(m => m.GetAllScenariosQuery)
            .Returns(query);

        var expectedResult = new List<ScenarioStateModel>();
        var captured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.QueryAsync<ScenarioStateModel>(query, It.IsAny<CancellationToken>(),
                Capture.In(captured)))
            .ReturnsAsync(expectedResult);

        // Act
        var result =
            await source.GetAllScenariosAsync(withDistributionKey ? DistributionKey : null, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);

        var addInput = captured.Single();
        CheckDbParam(addInput, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
    }

    [TestMethod]
    public async Task DeleteScenarioAsync_ScenarioNotSet_ShouldReturnFalse()
    {
        // Arrange
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        // Act
        var result = await source.DeleteScenarioAsync(null, null, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow(true, 1, true)]
    [DataRow(true, 0, false)]
    [DataRow(false, 1, true)]
    [DataRow(false, 0, false)]
    public async Task DeleteScenarioAsync_HappyFlow(bool withDistributionKey, int deleteCount, bool expectedResult)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string scenario = "scenario-1";
        const string query = "DELETE SCENARIO QUERY";
        mockQueryStore
            .Setup(m => m.DeleteScenarioQuery)
            .Returns(query);

        var captured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(),
                Capture.In(captured)))
            .ReturnsAsync(deleteCount);

        // Act
        var result = await source.DeleteScenarioAsync(scenario, withDistributionKey ? DistributionKey : null,
            CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);

        var deleteInput = captured.Single();
        CheckDbParam(deleteInput, "$.Scenario", scenario);
        CheckDbParam(deleteInput, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteAllScenariosAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mockQueryStore = _mocker.GetMock<IQueryStore>();
        var source = _mocker.CreateInstance<RelationalDbStubSource>();

        const string query = "DELETE ALL SCENARIOS QUERY";
        mockQueryStore
            .Setup(m => m.DeleteAllScenariosQuery)
            .Returns(query);

        var captured = new List<object>();
        _mockDatabaseContext
            .Setup(m => m.ExecuteAsync(query, It.IsAny<CancellationToken>(),
                Capture.In(captured)))
            .ReturnsAsync(1);

        // Act
        await source.DeleteAllScenariosAsync(withDistributionKey ? DistributionKey : null, CancellationToken.None);

        // Assert
        var addInput = captured.Single();
        CheckDbParam(addInput, "$.DistributionKey", withDistributionKey ? DistributionKey : string.Empty);
    }

    private void CheckDbParam<TResultType>(object input, string jsonPath, TResultType expectedValue)
    {
        var parsedParam = JObject.Parse(JsonConvert.SerializeObject(input));
        var result = parsedParam.SelectToken(jsonPath);
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedValue, result.Value<TResultType>());
    }
}
