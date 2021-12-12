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
        const string expectedStubId1 = "generated-93a77714a6d316c469acb4227430615e";
        const string expectedStubId2 = "generated-2e751b58fc0786b841770e7e0b6cc783";

        var requests = new[] { new HttpRequestModel(), new HttpRequestModel() };
        curlToHttpRequestMapperMock
            .Setup(m => m.MapCurlCommandsToHttpRequest(input))
            .Returns(requests);

        var conditions1 = new StubConditionsModel { Host = "host1"};
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[0]))
            .ReturnsAsync(conditions1);

        var conditions2 = new StubConditionsModel { Host = "host2"};
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
        var result = (await generator.GenerateCurlStubsAsync(input, false)).ToArray();

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
        const string expectedStubId1 = "generated-93a77714a6d316c469acb4227430615e";
        const string expectedStubId2 = "generated-2e751b58fc0786b841770e7e0b6cc783";

        var requests = new[] { new HttpRequestModel(), new HttpRequestModel() };
        curlToHttpRequestMapperMock
            .Setup(m => m.MapCurlCommandsToHttpRequest(input))
            .Returns(requests);

        var conditions1 = new StubConditionsModel { Host = "host1"};
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[0]))
            .ReturnsAsync(conditions1);

        var conditions2 = new StubConditionsModel { Host = "host2"};
        httpRequestToConditionsServiceMock
            .Setup(m => m.ConvertToConditionsAsync(requests[1]))
            .ReturnsAsync(conditions2);

        // Act
        var result = (await generator.GenerateCurlStubsAsync(input, true)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.IsTrue(result.All(s => s.Metadata != null));
        Assert.IsTrue(result.All(s => s.Stub.Response.Text == "OK!"));
        Assert.AreEqual(expectedStubId1, result[0].Stub.Id);
        Assert.AreEqual(expectedStubId2, result[1].Stub.Id);
        Assert.AreEqual(conditions1, result[0].Stub.Conditions);
        Assert.AreEqual(conditions2, result[1].Stub.Conditions);
        stubContextMock.Verify(m => m.DeleteStubAsync(It.IsAny<string>()), Times.Never);
        stubContextMock.Verify(m => m.AddStubAsync(It.IsAny<StubModel>()), Times.Never);
    }
}