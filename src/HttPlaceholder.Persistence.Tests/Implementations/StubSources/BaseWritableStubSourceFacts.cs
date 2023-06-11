using System.Collections.Generic;
using System.Linq;
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
        var result = (await source.GetRequestResultsOverviewAsync(CancellationToken.None)).ToArray();

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
        var result = (await source.GetRequestResultsOverviewAsync(CancellationToken.None)).ToArray();

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

        public override Task<IEnumerable<StubModel>> GetStubsAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task<IEnumerable<StubOverviewModel>>
            GetStubsOverviewAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        public override Task<StubModel> GetStubAsync(string stubId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task PrepareStubSourceAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task AddStubAsync(StubModel stub, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(
            CancellationToken cancellationToken) => Task.FromResult(_requests);

        public override Task<RequestResultModel> GetRequestAsync(string correlationId,
            CancellationToken cancellationToken) => throw new NotImplementedException();

        public override Task<ResponseModel>
            GetResponseAsync(string correlationId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public override Task CleanOldRequestResultsAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
