using System.Linq;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class CurlStubGeneratorFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task GenerateCurlStubsAsync_SaveStub()
    {
        // Arrange
        var curlToHttpRequestMapperMock = _mocker.GetMock<ICurlToHttpRequestMapper>();
        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var generator = _mocker.CreateInstance<CurlStubGenerator>();

        const string input = "curl commands";
        const string prefix = "prefix";
        const string expectedStubId1 = "prefixgenerated-819b15bb074e8525c3f5a92637ed5450";
        const string expectedStubId2 = "prefixgenerated-4b2683c2e53ed8d0a5829887373ce578";

        var requests = new[] {new HttpRequestModel(), new HttpRequestModel()};
        curlToHttpRequestMapperMock
            .Setup(m => m.MapCurlCommandsToHttpRequest(input))
            .Returns(requests);

        var conditions1 = new StubConditionsModel {Host = "host1"};
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(conditions1);

        var conditions2 = new StubConditionsModel {Host = "host2"};
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[1], It.IsAny<CancellationToken>()))
            .ReturnsAsync(conditions2);

        var fullStub1 = new FullStubModel();
        stubContextMock
            .Setup(m => m.AddStubAsync(It.Is<StubModel>(s => s.Conditions == conditions1),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fullStub1);

        var fullStub2 = new FullStubModel();
        stubContextMock
            .Setup(m => m.AddStubAsync(It.Is<StubModel>(s => s.Conditions == conditions2),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fullStub2);

        // Act
        var result = (await generator.GenerateStubsAsync(input, false, string.Empty, prefix, CancellationToken.None))
            .ToArray();

        // Assert
        Assert.AreEqual(fullStub1, result[0]);
        Assert.AreEqual(fullStub2, result[1]);
        stubContextMock.Verify(m => m.DeleteStubAsync(expectedStubId1, It.IsAny<CancellationToken>()));
        stubContextMock.Verify(m => m.DeleteStubAsync(expectedStubId2, It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task GenerateCurlStubsAsync_DoNotSaveStub()
    {
        // Arrange
        var curlToHttpRequestMapperMock = _mocker.GetMock<ICurlToHttpRequestMapper>();
        var httpRequestToConditionsServiceMock = _mocker.GetMock<IHttpRequestToConditionsService>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var generator = _mocker.CreateInstance<CurlStubGenerator>();

        const string input = "curl commands";
        const string tenant = "tenant1";
        const string prefix = "prefix";
        const string expectedStubId1 = "prefixgenerated-f75323d717c88403012eb92133d7cab4";
        const string expectedStubId2 = "prefixgenerated-e78f1fa3ca61910d7bcea4ea35e8184f";

        var requests = new[] {new HttpRequestModel(), new HttpRequestModel()};
        curlToHttpRequestMapperMock
            .Setup(m => m.MapCurlCommandsToHttpRequest(input))
            .Returns(requests);

        var conditions1 = new StubConditionsModel
        {
            Host = "host1", Method = "GET", Url = new StubUrlConditionModel {Path = "/path1"}
        };
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(conditions1);

        var conditions2 = new StubConditionsModel
        {
            Host = "host2", Method = "POST", Url = new StubUrlConditionModel {Path = "/path2"}
        };
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[1], It.IsAny<CancellationToken>()))
            .ReturnsAsync(conditions2);

        // Act
        var result = (await generator.GenerateStubsAsync(input, true, tenant, prefix, CancellationToken.None))
            .ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.IsTrue(result.All(s => s.Metadata != null));
        Assert.IsTrue(result.All(s => s.Stub.Response.Text == "OK!"));
        Assert.IsTrue(result.All(s => s.Stub.Tenant == tenant));

        Assert.AreEqual(expectedStubId1, result[0].Stub.Id);
        Assert.AreEqual("GET request to path /path1", result[0].Stub.Description);
        Assert.AreEqual("POST request to path /path2", result[1].Stub.Description);
        Assert.AreEqual(expectedStubId2, result[1].Stub.Id);
        Assert.AreEqual(conditions1, result[0].Stub.Conditions);
        Assert.AreEqual(conditions2, result[1].Stub.Conditions);
        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
