using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Persistence.Tests.Implementations;

[TestClass]
public class InMemoryStubSourceFacts
{
    private static readonly Faker _faker = new();
    private readonly AutoMocker _mocker = new();
    private readonly SettingsModel _settings = new() {Storage = new StorageSettingsModel()};

    [TestInitialize]
    public void Initialize() => _mocker.Use(Options.Create(_settings));

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        var response = CreateResponseModel();

        // Act
        await source.AddRequestResultAsync(request, response, CancellationToken.None);

        // Assert
        Assert.AreEqual(request, source.RequestResultModels.Single());
        Assert.AreEqual(response, source.StubResponses.Single());
        Assert.AreEqual(response, source.RequestResponseMap[request]);
        Assert.IsTrue(request.HasResponse);
    }

    [TestMethod]
    public async Task AddRequestResultAsync_HappyFlow_ResponseNotSet()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();

        // Act
        await source.AddRequestResultAsync(request, null, CancellationToken.None);

        // Assert
        Assert.IsFalse(source.StubResponses.Any());
        Assert.IsFalse(source.RequestResponseMap.Any());
        Assert.IsFalse(request.HasResponse);
    }

    [TestMethod]
    public async Task AddStubAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = new StubModel();

        // Act
        await source.AddStubAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(stub, source.StubModels.Single());
    }

    [TestMethod]
    public async Task GetRequestResultsOverviewAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var requestModel = CreateRequestResultModel();
        source.RequestResultModels.Add(requestModel);

        // Act
        var result = (await source.GetRequestResultsOverviewAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var overviewRequest = result.Single();
        var reqParams = requestModel.RequestParameters;
        Assert.AreEqual(reqParams.Method, overviewRequest.Method);
        Assert.AreEqual(reqParams.Url, overviewRequest.Url);
        Assert.AreEqual(requestModel.CorrelationId, overviewRequest.CorrelationId);
        Assert.AreEqual(requestModel.StubTenant, overviewRequest.StubTenant);
        Assert.AreEqual(requestModel.ExecutingStubId, overviewRequest.ExecutingStubId);
        Assert.AreEqual(requestModel.RequestBeginTime, overviewRequest.RequestBeginTime);
        Assert.AreEqual(requestModel.RequestEndTime, overviewRequest.RequestEndTime);
        Assert.AreEqual(requestModel.HasResponse, overviewRequest.HasResponse);
    }

    [TestMethod]
    public async Task GetRequestAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        source.RequestResultModels.Add(request1);
        source.RequestResultModels.Add(request2);

        // Act
        var result = await source.GetRequestAsync(request2.CorrelationId, CancellationToken.None);

        // Assert
        Assert.AreEqual(request2, result);
    }

    [TestMethod]
    public async Task GetResponseAsync_RequestNotFound_ShouldReturnNull()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        source.RequestResultModels.Add(request);

        // Act
        var result = await source.GetResponseAsync(request.CorrelationId + "1", CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetResponseAsync_ResponseNotFound_ShouldReturnNull()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        source.RequestResultModels.Add(request);

        // Act
        var result = await source.GetResponseAsync(request.CorrelationId, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetResponseAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();

        var request = CreateRequestResultModel();
        source.RequestResultModels.Add(request);

        var response = CreateResponseModel();
        source.StubResponses.Add(response);
        source.RequestResponseMap.Add(request, response);

        // Act
        var result = await source.GetResponseAsync(request.CorrelationId, CancellationToken.None);

        // Assert
        Assert.AreEqual(response, result);
    }

    [TestMethod]
    public async Task DeleteAllRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        source.RequestResultModels.Add(CreateRequestResultModel());
        source.StubResponses.Add(CreateResponseModel());

        // Act
        await source.DeleteAllRequestResultsAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(source.RequestResultModels.Any());
        Assert.IsFalse(source.StubResponses.Any());
        Assert.IsFalse(source.RequestResponseMap.Any());
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestNotFound_ShouldReturnFalse()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        source.RequestResultModels.Add(request);

        // Act
        var result = await source.DeleteRequestAsync(request.CorrelationId + "1", CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(source.RequestResultModels.Any());
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestFound_ShouldReturnTrue_NoResponse()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        source.RequestResultModels.Add(request1);
        source.RequestResultModels.Add(request2);

        var response2 = CreateResponseModel();
        source.StubResponses.Add(response2);
        source.RequestResponseMap.Add(request2, response2);

        // Act
        var result = await source.DeleteRequestAsync(request1.CorrelationId, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(source.RequestResultModels.Any(r => r == request1));
        Assert.IsTrue(source.StubResponses.All(r => r == response2));
        Assert.IsTrue(source.RequestResponseMap.All(r => r.Key == request2));
    }

    [TestMethod]
    public async Task DeleteRequestAsync_RequestFound_ShouldReturnTrue_WithResponse()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request1 = CreateRequestResultModel();
        var request2 = CreateRequestResultModel();
        source.RequestResultModels.Add(request1);
        source.RequestResultModels.Add(request2);

        var response2 = CreateResponseModel();
        source.StubResponses.Add(response2);
        source.RequestResponseMap.Add(request2, response2);

        // Act
        var result = await source.DeleteRequestAsync(request2.CorrelationId, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(source.RequestResultModels.Any(r => r == request2));
        Assert.IsFalse(source.StubResponses.Any(r => r == response2));
        Assert.IsFalse(source.RequestResponseMap.Any(r => r.Key == request2));
    }

    [TestMethod]
    public async Task DeleteStubAsync_StubNotFound_ShouldReturnFalse()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        source.StubModels.Add(stub);

        // Act
        var result = await source.DeleteStubAsync(stub.Id + "1", CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(source.StubModels.Any());
    }

    [TestMethod]
    public async Task DeleteStubAsync_StubFound_ShouldReturnTrue()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        source.StubModels.Add(stub);

        // Act
        var result = await source.DeleteStubAsync(stub.Id, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(source.StubModels.Any());
    }

    [TestMethod]
    public async Task GetRequestResultsAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var request = CreateRequestResultModel();
        source.RequestResultModels.Add(request);

        // Act
        var result = await source.GetRequestResultsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(request, result.Single());
    }

    [TestMethod]
    public async Task GetStubsAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        source.StubModels.Add(stub);

        // Act
        var result = await source.GetStubsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(stub, result.Single());
    }

    [TestMethod]
    public async Task GetStubsOverviewAsync_HappyFlow()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        source.StubModels.Add(stub);

        // Act
        var result = (await source.GetStubsOverviewAsync(CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var overviewStub = result.Single();
        Assert.AreEqual(stub.Id, overviewStub.Id);
        Assert.AreEqual(stub.Tenant, overviewStub.Tenant);
        Assert.AreEqual(stub.Enabled, overviewStub.Enabled);
    }

    [TestMethod]
    public async Task GetStubAsync_StubNotFound_ShouldReturnNull()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        source.StubModels.Add(stub);

        // Act
        var result = await source.GetStubAsync(stub.Id + "1", CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetStubAsync_StubFound_ShouldReturnStub()
    {
        // Arrange
        var source = _mocker.CreateInstance<InMemoryStubSource>();
        var stub = CreateStubModel();
        source.StubModels.Add(stub);

        // Act
        var result = await source.GetStubAsync(stub.Id, CancellationToken.None);

        // Assert
        Assert.AreEqual(stub, result);
    }

    [TestMethod]
    public async Task CleanOldRequestResultsAsync_HappyFlow()
    {
        // Arrange
        _settings.Storage.OldRequestsQueueLength = 2;
        var source = _mocker.CreateInstance<InMemoryStubSource>();

        RequestResultModel CreateAndAddRequestResultModel(DateTime requestEndDate)
        {
            var request = CreateRequestResultModel();
            request.RequestEndTime = requestEndDate;
            source.RequestResultModels.Add(request);
            return request;
        }

        ResponseModel CreateAndAddResponseModel(RequestResultModel request)
        {
            var response = CreateResponseModel();
            source.StubResponses.Add(response);
            source.RequestResponseMap.Add(request, response);
            return response;
        }

        var now = DateTime.Now;
        var request1 = CreateAndAddRequestResultModel(now.AddSeconds(-1));
        var response1 = CreateAndAddResponseModel(request1);

        var request2 = CreateAndAddRequestResultModel(now.AddSeconds(-10));
        var response2 = CreateAndAddResponseModel(request2);

        var request3 = CreateAndAddRequestResultModel(now.AddSeconds(-9));

        // Act
        await source.CleanOldRequestResultsAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(2, source.RequestResultModels.Count);
        Assert.IsTrue(source.RequestResultModels.Contains(request1));
        Assert.IsTrue(source.StubResponses.Any(r => r == response1));
        Assert.IsTrue(source.RequestResponseMap.Any(r => r.Key == request1));

        Assert.IsFalse(source.RequestResultModels.Contains(request2));
        Assert.IsFalse(source.StubResponses.Any(r => r == response2));
        Assert.IsFalse(source.RequestResponseMap.Any(r => r.Key == request2));

        Assert.IsTrue(source.RequestResultModels.Contains(request3));
        Assert.IsFalse(source.RequestResponseMap.Any(r => r.Key == request3));
    }

    private static RequestResultModel CreateRequestResultModel()
    {
        var methods = new[] {"GET", "POST", "PUT", "DELETE"};
        return new RequestResultModel
        {
            CorrelationId = _faker.Random.Guid().ToString(),
            RequestParameters = new RequestParametersModel
            {
                Body = _faker.Random.Words(),
                Headers = new Dictionary<string, string> {{"X-Api-Key", _faker.Random.Guid().ToString()}},
                Method = _faker.PickRandom(methods),
                Url = _faker.Internet.Url(),
                ClientIp = _faker.Internet.Ip()
            },
            StubTenant = _faker.Random.Word(),
            ExecutingStubId = _faker.Random.Words(),
            RequestBeginTime = _faker.Date.Past(),
            RequestEndTime = _faker.Date.Past(),
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
        Headers = {{_faker.Random.Word(), _faker.Random.Word()}},
        StatusCode = _faker.Random.Int(100, 599)
    };
}
