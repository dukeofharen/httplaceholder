using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Persistence.Implementations.StubSources;
using HttPlaceholder.Web.Shared.Dto.v1.Scenarios;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Tests.Integration.Stubs;

public abstract class StubIntegrationTestBase : IntegrationTestBase
{
    private const string InputFilePath = @"D:\tmp\input.yml";
    private YamlFileStubSource _stubSource;
    protected Mock<IClientDataResolver> ClientDataResolverMock;
    protected Mock<IDateTime> DateTimeMock;
    protected Mock<IFileService> FileServiceMock;
    internal InMemoryStubSource InMemoryStubSource;
    protected MockHttpMessageHandler MockHttp;

    protected void InitializeStubIntegrationTest(string yamlFileName)
    {
        // Load the integration YAML file here.
        var path = Path.Combine(AssemblyHelper.GetCallingAssemblyRootPath(), yamlFileName);
        var integrationYml = File.ReadAllText(path);

        FileServiceMock = new Mock<IFileService>();
        FileServiceMock
            .Setup(m => m.ReadAllTextAsync(InputFilePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(integrationYml);
        FileServiceMock
            .Setup(m => m.FileExistsAsync(InputFilePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        DateTimeMock = new Mock<IDateTime>();
        DateTimeMock
            .Setup(m => m.Now)
            .Returns(() => DateTime.Now);
        DateTimeMock
            .Setup(m => m.UtcNow)
            .Returns(() => DateTime.UtcNow);

        ClientDataResolverMock = new Mock<IClientDataResolver>();
        Settings.Storage.InputFile = InputFilePath;

        _stubSource = new YamlFileStubSource(
            FileServiceMock.Object,
            new Mock<ILogger<YamlFileStubSource>>().Object,
            Options,
            new Mock<IStubModelValidator>().Object,
            new Mock<IStubNotify>().Object,
            DateTimeMock.Object);

        MockHttp = new MockHttpMessageHandler();
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(() => MockHttp.ToHttpClient());

        InMemoryStubSource = new InMemoryStubSource(Options);
        InitializeIntegrationTest(
        [
            (typeof(IClientDataResolver), ClientDataResolverMock.Object),
                (typeof(IFileService), FileServiceMock.Object), (typeof(IDateTime), DateTimeMock.Object),
                (typeof(IHttpClientFactory), mockHttpClientFactory.Object)
        ], new IStubSource[] { _stubSource, InMemoryStubSource });
    }

    protected async Task SetScenario(string scenarioName, ScenarioStateInputDto scenario)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{BaseAddress}ph-api/scenarios/{scenarioName}"),
            Content = new StringContent(JsonConvert.SerializeObject(scenario), Encoding.UTF8, MimeTypes.JsonMime)
        };
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    protected async Task<IEnumerable<RequestResultModel>> GetRequestsAsync() =>
        await InMemoryStubSource.GetRequestResultsAsync(null, null, CancellationToken.None);
}
