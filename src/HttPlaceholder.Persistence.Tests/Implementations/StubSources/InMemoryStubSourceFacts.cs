using System.Collections.Generic;
using System.Linq;
using Bogus;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Implementations.StubSources;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class InMemoryStubSourceFacts
{
    private const string DistributionKey = "username";
    private static readonly Faker _faker = new();
    private readonly AutoMocker _mocker = new();
    private readonly SettingsModel _settings = new() { Storage = new StorageSettingsModel() };

    [TestInitialize]
    public void Initialize() => _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddRequestResultAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        var response = CreateResponseModel();
        var key = withDistributionKey ? DistributionKey : null;

        // Act
        await source.AddRequestResultAsync(request, response, key, CancellationToken.None);

        // Assert
        var item = source.GetCollection(key);
        Assert.AreEqual(request, item.RequestResultModels.Single());
        Assert.AreEqual(response, item.StubResponses.Single());
        Assert.AreEqual(response, item.RequestResponseMap[request]);
        Assert.IsTrue(request.HasResponse);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddRequestResultAsync_HappyFlow_ResponseNotSet(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;

        // Act
        await source.AddRequestResultAsync(request, null, key, CancellationToken.None);

        // Assert
        var item = source.GetCollection(key);
        Assert.IsFalse(item.StubResponses.Any());
        Assert.IsFalse(item.RequestResponseMap.Any());
        Assert.IsFalse(request.HasResponse);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddStubAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = new StubModel();
        var key = withDistributionKey ? DistributionKey : null;

        // Act
        await source.AddStubAsync(stub, key, CancellationToken.None);

        // Assert
        var item = source.GetCollection(key);
        Assert.AreEqual(stub, item.StubModels.Single());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request1);
        item.RequestResultModels.Add(request2);

        // Act
        var result = await source.GetRequestAsync(request2.CorrelationId, key, CancellationToken.None);

        // Assert
        Assert.AreEqual(request2, result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetResponseAsync_RequestNotFound_ShouldReturnNull(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request);

        // Act
        var result = await source.GetResponseAsync(request.CorrelationId + "1", key, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetResponseAsync_ResponseNotFound_ShouldReturnNull(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request);

        // Act
        var result = await source.GetResponseAsync(request.CorrelationId, key, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetResponseAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();

        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);

        var request = CreateRequestResultModel();
        item.RequestResultModels.Add(request);

        var response = CreateResponseModel();
        item.StubResponses.Add(response);
        item.RequestResponseMap.Add(request, response);

        // Act
        var result = await source.GetResponseAsync(request.CorrelationId, key, CancellationToken.None);

        // Assert
        Assert.AreEqual(response, result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteAllRequestResultsAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(CreateRequestResultModel());
        item.StubResponses.Add(CreateResponseModel());

        // Act
        await source.DeleteAllRequestResultsAsync(key, CancellationToken.None);

        // Assert
        Assert.IsFalse(item.RequestResultModels.Any());
        Assert.IsFalse(item.StubResponses.Any());
        Assert.IsFalse(item.RequestResponseMap.Any());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteRequestAsync_RequestNotFound_ShouldReturnFalse(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request);

        // Act
        var result = await source.DeleteRequestAsync(request.CorrelationId + "1", key, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(item.RequestResultModels.Any());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteRequestAsync_RequestFound_ShouldReturnTrue_NoResponse(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request1);
        item.RequestResultModels.Add(request2);

        var response2 = CreateResponseModel();
        item.StubResponses.Add(response2);
        item.RequestResponseMap.Add(request2, response2);

        // Act
        var result = await source.DeleteRequestAsync(request1.CorrelationId, key, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(item.RequestResultModels.Any(r => r == request1));
        Assert.IsTrue(item.StubResponses.All(r => r == response2));
        Assert.IsTrue(item.RequestResponseMap.All(r => r.Key == request2));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteRequestAsync_RequestFound_ShouldReturnTrue_WithResponse(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request1);
        item.RequestResultModels.Add(request2);

        var response2 = CreateResponseModel();
        item.StubResponses.Add(response2);
        item.RequestResponseMap.Add(request2, response2);

        // Act
        var result = await source.DeleteRequestAsync(request2.CorrelationId, key, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(item.RequestResultModels.Any(r => r == request2));
        Assert.IsFalse(item.StubResponses.Any(r => r == response2));
        Assert.IsFalse(item.RequestResponseMap.Any(r => r.Key == request2));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteStubAsync_StubNotFound_ShouldReturnFalse(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.StubModels.Add(stub);

        // Act
        var result = await source.DeleteStubAsync(stub.Id + "1", key, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(item.StubModels.Any());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteStubAsync_StubFound_ShouldReturnTrue(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.StubModels.Add(stub);

        // Act
        var result = await source.DeleteStubAsync(stub.Id, key, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(item.StubModels.Any());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestResultsAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request);

        // Act
        var result = await source.GetRequestResultsAsync(null, key, CancellationToken.None);

        // Assert
        Assert.AreEqual(request, result.Single());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestResultsAsync_FromIdentifierSet_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        var request3 = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request1);
        item.RequestResultModels.Add(request2);
        item.RequestResultModels.Add(request3);

        // Act
        var result = (await source.GetRequestResultsAsync(PagingModel.Create(request2.CorrelationId, null),
            key,
            CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(request2, result[0]);
        Assert.AreEqual(request1, result[1]);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetRequestResultsAsync_FromIdentifierAndItemsPerPageSet_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        var request3 = CreateRequestResultModel();
        var request4 = CreateRequestResultModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.RequestResultModels.Add(request1);
        item.RequestResultModels.Add(request2);
        item.RequestResultModels.Add(request3);
        item.RequestResultModels.Add(request4);

        // Act
        var result =
            (await source.GetRequestResultsAsync(
                PagingModel.Create(request3.CorrelationId, 2 ), key,
                CancellationToken.None))
            .ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(request3, result[0]);
        Assert.AreEqual(request2, result[1]);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubsAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.StubModels.Add(stub);

        // Act
        var result = await source.GetStubsAsync(key, CancellationToken.None);

        // Assert
        Assert.AreEqual(stub, result.Single().Stub);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubsOverviewAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.StubModels.Add(stub);

        // Act
        var result = (await source.GetStubsOverviewAsync(key, CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var overviewStub = result.Single();
        Assert.AreEqual(stub.Id, overviewStub.Stub.Id);
        Assert.AreEqual(stub.Tenant, overviewStub.Stub.Tenant);
        Assert.AreEqual(stub.Enabled, overviewStub.Stub.Enabled);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubAsync_StubNotFound_ShouldReturnNull(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.StubModels.Add(stub);

        // Act
        var result = await source.GetStubAsync(stub.Id + "1", key, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetStubAsync_StubFound_ShouldReturnStub(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        var key = withDistributionKey ? DistributionKey : null;
        var item = source.GetCollection(key);
        item.StubModels.Add(stub);

        // Act
        var result = await source.GetStubAsync(stub.Id, key, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.HasValue);
        Assert.AreEqual(stub, result.Value.Stub);
    }

    [TestMethod]
    public async Task CleanOldRequestResultsAsync_HappyFlow()
    {
        // Arrange
        _settings.Storage.OldRequestsQueueLength = 2;
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var item = source.GetCollection(null);

        var now = DateTime.Now;
        var request1 = CreateAndAddRequestResultModel(now.AddSeconds(-1));
        var response1 = CreateAndAddResponseModel(request1);

        var request2 = CreateAndAddRequestResultModel(now.AddSeconds(-10));
        var response2 = CreateAndAddResponseModel(request2);

        var request3 = CreateAndAddRequestResultModel(now.AddSeconds(-9));

        // Act
        await source.CleanOldRequestResultsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(2, item.RequestResultModels.Count);
        Assert.IsTrue(item.RequestResultModels.Contains(request1));
        Assert.IsTrue(item.StubResponses.Any(r => r == response1));
        Assert.IsTrue(item.RequestResponseMap.Any(r => r.Key == request1));

        Assert.IsFalse(item.RequestResultModels.Contains(request2));
        Assert.IsFalse(item.StubResponses.Any(r => r == response2));
        Assert.IsFalse(item.RequestResponseMap.Any(r => r.Key == request2));

        Assert.IsTrue(item.RequestResultModels.Contains(request3));
        Assert.IsFalse(item.RequestResponseMap.Any(r => r.Key == request3));
        return;

        ResponseModel CreateAndAddResponseModel(RequestResultModel request)
        {
            var response = CreateResponseModel();
            item.StubResponses.Add(response);
            item.RequestResponseMap.Add(request, response);
            return response;
        }

        RequestResultModel CreateAndAddRequestResultModel(DateTime requestEndDate)
        {
            var request = CreateRequestResultModel();
            request.RequestEndTime = requestEndDate;
            item.RequestResultModels.Add(request);
            return request;
        }
    }

    [TestMethod]
    public async Task GetScenarioAsync_ScenarioNotSet_ShouldReturnNull()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();

        // Act
        var result = await source.GetScenarioAsync(null, null, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetScenarioAsync_ScenarioSetAndFound_ShouldReturnScenario(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;
        var item = source.GetCollection(key);
        var scenarioStateModel = new ScenarioStateModel(scenario)
        {
            State = Guid.NewGuid().ToString(), Scenario = scenario, HitCount = 11
        };
        Assert.IsTrue(item.Scenarios.TryAdd(scenario, scenarioStateModel));

        // Act
        var result = await source.GetScenarioAsync(scenario, key, CancellationToken.None);

        // Assert
        Assert.AreEqual(scenarioStateModel.State, result.State);
        Assert.AreEqual(scenarioStateModel.Scenario, result.Scenario);
        Assert.AreEqual(scenarioStateModel.HitCount, result.HitCount);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetScenarioAsync_ScenarioSetButNotFound_ShouldReturnNull(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;

        // Act
        var result = await source.GetScenarioAsync(scenario, key, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddScenarioAsync_ScenarioAlreadyAdded_ShouldThrowInvalidOperationException(
        bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;
        var item = source.GetCollection(key);
        Assert.IsTrue(item.Scenarios.TryAdd(scenario, new ScenarioStateModel()));

        // Act
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            source.AddScenarioAsync(scenario, new ScenarioStateModel(scenario), key, CancellationToken.None));

        // Assert
        Assert.AreEqual($"Scenario state with key '{scenario}' already exists.", exception.Message);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AddScenarioAsync_ScenarioNotAddedYet_HappyFlow(
        bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;
        var input = new ScenarioStateModel(scenario)
        {
            Scenario = scenario, HitCount = 11, State = Guid.NewGuid().ToString()
        };

        // Act
        var result =
            await source.AddScenarioAsync(scenario, input, key, CancellationToken.None);

        // Assert
        Assert.AreEqual(input.Scenario, result.Scenario);
        Assert.AreEqual(input.HitCount, result.HitCount);
        Assert.AreEqual(input.State, result.State);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task UpdateScenarioAsync_ScenarioNotFound_ShouldDoNothing(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;

        // Act / Assert
        await source.UpdateScenarioAsync(scenario, new ScenarioStateModel(scenario), key, CancellationToken.None);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task UpdateScenarioAsync_ScenarioFound_ShouldUpdate(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;

        var currentStateModel = new ScenarioStateModel(scenario) { HitCount = 11, State = Guid.NewGuid().ToString() };
        var collection = source.GetCollection(key);
        Assert.IsTrue(collection.Scenarios.TryAdd(scenario, currentStateModel));

        var newStateModel = new ScenarioStateModel(scenario) { HitCount = 12, State = Guid.NewGuid().ToString() };

        // Act
        await source.UpdateScenarioAsync(scenario, newStateModel, key, CancellationToken.None);

        // Assert
        Assert.IsTrue(collection.Scenarios.TryGetValue(scenario, out var result));
        Assert.AreEqual(newStateModel.Scenario, result.Scenario);
        Assert.AreEqual(newStateModel.HitCount, result.HitCount);
        Assert.AreEqual(newStateModel.State, result.State);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task GetAllScenariosAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;

        var currentStateModel = new ScenarioStateModel(scenario) { HitCount = 11, State = Guid.NewGuid().ToString() };
        var collection = source.GetCollection(key);
        Assert.IsTrue(collection.Scenarios.TryAdd(scenario, currentStateModel));

        // Act
        var result = (await source.GetAllScenariosAsync(key, CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(currentStateModel.Scenario, result[0].Scenario);
        Assert.AreEqual(currentStateModel.HitCount, result[0].HitCount);
        Assert.AreEqual(currentStateModel.State, result[0].State);
    }

    [TestMethod]
    public async Task DeleteScenarioAsync_ScenarioNotSet_ShouldReturnFalse()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();

        // Act
        var result = await source.DeleteScenarioAsync(null, null, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteScenarioAsync_ScenarioNotFound_ShouldReturnFalse(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;

        var collection = source.GetCollection(key);
        Assert.IsTrue(collection.Scenarios.TryAdd(scenario + "1", new ScenarioStateModel(scenario + "1")));

        // Act
        var result = await source.DeleteScenarioAsync(scenario, key, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(1, collection.Scenarios.Count);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteScenarioAsync_ScenarioFound_ShouldReturnTrue(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;

        var collection = source.GetCollection(key);
        Assert.IsTrue(collection.Scenarios.TryAdd(scenario, new ScenarioStateModel(scenario)));

        // Act
        var result = await source.DeleteScenarioAsync(scenario, key, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(0, collection.Scenarios.Count);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DeleteAllScenariosAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        const string scenario = "scenario-1";
        var key = withDistributionKey ? Guid.NewGuid().ToString() : null;

        var collection = source.GetCollection(key);
        Assert.IsTrue(collection.Scenarios.TryAdd(scenario, new ScenarioStateModel(scenario)));

        // Act
        await source.DeleteAllScenariosAsync(key, CancellationToken.None);

        // Assert
        Assert.AreEqual(0, collection.Scenarios.Count);
    }

    private static RequestResultModel CreateRequestResultModel()
    {
        var methods = new[] { "GET", "POST", "PUT", "DELETE" };
        return new RequestResultModel
        {
            CorrelationId = _faker.Random.Guid().ToString(),
            RequestParameters = new RequestParametersModel
            {
                Body = _faker.Random.Words(),
                Headers = new Dictionary<string, string> { { "X-Api-Key", _faker.Random.Guid().ToString() } },
                Method = _faker.PickRandom(methods),
                Url = _faker.Internet.Url(),
                ClientIp = _faker.Internet.Ip()
            },
            StubTenant = _faker.Random.Word(),
            ExecutingStubId = _faker.Random.Words(),
            RequestBeginTime = DateTime.Now,
            RequestEndTime = DateTime.Now,
            HasResponse = false
        };
    }

    private static StubModel CreateStubModel() => new()
    {
        Id = _faker.Random.Word(), Tenant = _faker.Random.Word(), Enabled = _faker.Random.Bool()
    };

    private static ResponseModel CreateResponseModel() => new()
    {
        Body = _faker.Random.Bytes(100),
        BodyIsBinary = true,
        Headers = { { _faker.Random.Word(), _faker.Random.Word() } },
        StatusCode = _faker.Random.Int(100, 599)
    };
}
