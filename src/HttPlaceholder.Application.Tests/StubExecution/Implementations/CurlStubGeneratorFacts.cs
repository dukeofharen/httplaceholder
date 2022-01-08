using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

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
        const string expectedStubId1 = "generated-1614cb377bcd24439621eadcb4d8a18f";
        const string expectedStubId2 = "generated-3fac60c4739c66a7ec8c1dc681fd5024";

        var requests = new[] {new HttpRequestModel(), new HttpRequestModel()};
        curlToHttpRequestMapperMock
            .Setup(m => m.MapCurlCommandsToHttpRequest(input))
            .Returns(requests);

        var conditions1 = new StubConditionsModel {Host = "host1"};
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[0]))
            .ReturnsAsync(conditions1);

        var conditions2 = new StubConditionsModel {Host = "host2"};
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[1]))
            .ReturnsAsync(conditions2);

        var fullStub1 = new FullStubModel();
        stubContextMock
            .Setup(m => m.AddStubAsync(It.Is<StubModel>(s => s.Conditions == conditions1)))
            .ReturnsAsync(fullStub1);

        var fullStub2 = new FullStubModel();
        stubContextMock
            .Setup(m => m.AddStubAsync(It.Is<StubModel>(s => s.Conditions == conditions2)))
            .ReturnsAsync(fullStub2);

        // Act
        var result = (await generator.GenerateCurlStubsAsync(input, false, string.Empty)).ToArray();

        // Assert
        Assert.AreEqual(fullStub1, result[0]);
        Assert.AreEqual(fullStub2, result[1]);
        stubContextMock.Verify(m => m.DeleteStubAsync(expectedStubId1));
        stubContextMock.Verify(m => m.DeleteStubAsync(expectedStubId2));
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
        const string expectedStubId1 = "generated-f8ce4ba111b3c6f60616c4f7ca5cbdb5";
        const string expectedStubId2 = "generated-732b63a987d7856932a66471521d8ce8";

        var requests = new[] {new HttpRequestModel(), new HttpRequestModel()};
        curlToHttpRequestMapperMock
            .Setup(m => m.MapCurlCommandsToHttpRequest(input))
            .Returns(requests);

        var conditions1 = new StubConditionsModel
        {
            Host = "host1", Method = "GET", Url = new StubUrlConditionModel {Path = "/path1"}
        };
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[0]))
            .ReturnsAsync(conditions1);

        var conditions2 = new StubConditionsModel
        {
            Host = "host2", Method = "POST", Url = new StubUrlConditionModel {Path = "/path2"}
        };
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[1]))
            .ReturnsAsync(conditions2);

        // Act
        var result = (await generator.GenerateCurlStubsAsync(input, true, tenant)).ToArray();

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
        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>()), Times.Never);
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>()), Times.Never);
    }
}
