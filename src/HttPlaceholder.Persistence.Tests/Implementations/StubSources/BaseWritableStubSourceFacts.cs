using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Implementations.StubSources;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources;

[TestClass]
public class BaseWritableStubSourceFacts
{
    [TestMethod]
    public async Task GetRequestResultsOverviewAsync_RequestParametersIsNull()
    {
        // Arrange
        var request = new RequestResultModel
        {
            RequestParameters = null,
            CorrelationId = Guid.NewGuid().ToString(),
            StubTenant = "tenant1",
            ExecutingStubId = "stub1",
            RequestBeginTime = DateTime.Now,
            RequestEndTime = DateTime.Now,
            HasResponse = true
        };
        var source = new TestStubSource();
        source.SetRequests(new[] {request});

        // Act
        var result = (await source.GetRequestResultsOverviewAsync(null, "key", CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var req = result[0];
        Assert.IsNull(req.Method);
        Assert.IsNull(req.Url);
        Assert.AreEqual(request.CorrelationId, req.CorrelationId);
        Assert.AreEqual(request.StubTenant, req.StubTenant);
        Assert.AreEqual(request.ExecutingStubId, req.ExecutingStubId);
        Assert.AreEqual(request.RequestBeginTime, req.RequestBeginTime);
        Assert.AreEqual(request.RequestEndTime, req.RequestEndTime);
        Assert.AreEqual(request.HasResponse, req.HasResponse);
    }

    [TestMethod]
    public async Task GetRequestResultsOverviewAsync_RequestParametersIsSet()
    {
        // Arrange
        var request = new RequestResultModel
        {
            RequestParameters = new RequestParametersModel {Method = "GET", Url = "/url"},
            CorrelationId = Guid.NewGuid().ToString(),
            StubTenant = "tenant1",
            ExecutingStubId = "stub1",
            RequestBeginTime = DateTime.Now,
            RequestEndTime = DateTime.Now,
            HasResponse = true
        };
        var source = new TestStubSource();
        source.SetRequests(new[] {request});

        // Act
        var result = (await source.GetRequestResultsOverviewAsync(null, "key", CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var req = result[0];
        Assert.AreEqual(request.RequestParameters.Method, req.Method);
        Assert.AreEqual(request.RequestParameters.Url, req.Url);
        Assert.AreEqual(request.CorrelationId, req.CorrelationId);
        Assert.AreEqual(request.StubTenant, req.StubTenant);
        Assert.AreEqual(request.ExecutingStubId, req.ExecutingStubId);
        Assert.AreEqual(request.RequestBeginTime, req.RequestBeginTime);
        Assert.AreEqual(request.RequestEndTime, req.RequestEndTime);
        Assert.AreEqual(request.HasResponse, req.HasResponse);
    }

    public class TestStubSource : BaseWritableStubSource
    {
        private IEnumerable<RequestResultModel> _requests;

        public void SetRequests(IEnumerable<RequestResultModel> requests) => _requests = requests;

        public override Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>> GetStubsOverviewAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(string stubId, string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task PrepareStubSourceAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task AddStubAsync(StubModel stub, string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task<bool> DeleteStubAsync(string stubId, string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
            string distributionKey = null,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public override Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(
            PagingModel pagingModel,
            string distributionKey = null,
            CancellationToken cancellationToken = default) => Task.FromResult(_requests);

        public override Task<RequestResultModel> GetRequestAsync(string correlationId, string distributionKey = null,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public override Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public override Task DeleteAllRequestResultsAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task<bool> DeleteRequestAsync(string correlationId, string distributionKey = null,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public override Task CleanOldRequestResultsAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public override Task<ScenarioStateModel> GetScenarioAsync(string scenario, string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task<ScenarioStateModel> AddScenarioAsync(string scenario,
            ScenarioStateModel scenarioStateModel, string distributionKey = null,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public override Task UpdateScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
            string distributionKey = null,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public override Task<IEnumerable<ScenarioStateModel>> GetAllScenariosAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task<bool> DeleteScenarioAsync(string scenario, string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public override Task DeleteAllScenariosAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
