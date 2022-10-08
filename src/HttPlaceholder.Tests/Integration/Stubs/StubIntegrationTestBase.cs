using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Dto.v1.Scenarios;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Tests.Integration.Stubs;

public abstract class StubIntegrationTestBase : IntegrationTestBase
{
    private const string InputFilePath = @"D:\tmp\input.yml";
    protected Mock<IClientDataResolver> ClientDataResolverMock;
    protected Mock<IFileService> FileServiceMock;
    private YamlFileStubSource _stubSource;
    private Mock<IWritableStubSource> _writableStubSourceMock;
    protected Mock<IDateTime> DateTimeMock;
    protected MockHttpMessageHandler MockHttp;
    protected readonly IList<RequestResultModel> Requests = new List<RequestResultModel>();
    protected readonly IList<ResponseModel> Responses = new List<ResponseModel>();

    protected void InitializeStubIntegrationTest(string yamlFileName)
    {
        // Load the integration YAML file here.
        var path = Path.Combine(AssemblyHelper.GetCallingAssemblyRootPath(), yamlFileName);
        var integrationYml = File.ReadAllText(path);

        FileServiceMock = new Mock<IFileService>();
        FileServiceMock
            .Setup(m => m.ReadAllTextAsync(InputFilePath))
            .ReturnsAsync(integrationYml);
        FileServiceMock
            .Setup(m => m.FileExistsAsync(InputFilePath))
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
            new Mock<IStubModelValidator>().Object);
        _writableStubSourceMock = new Mock<IWritableStubSource>();
        _writableStubSourceMock
            .Setup(s => s.AddRequestResultAsync(It.IsAny<RequestResultModel>(), It.IsAny<ResponseModel>()))
            .Callback<RequestResultModel, ResponseModel>((req, res) =>
            {
                Requests.Add(req);
                Responses.Add(res);
            });

        MockHttp = new MockHttpMessageHandler();
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory
            .Setup(m => m.CreateClient("proxy"))
            .Returns(() => MockHttp.ToHttpClient());

        InitializeIntegrationTest(
            new (Type, object)[]
            {
                (typeof(IClientDataResolver), ClientDataResolverMock.Object),
                (typeof(IFileService), FileServiceMock.Object), (typeof(IDateTime), DateTimeMock.Object),
                (typeof(IHttpClientFactory), mockHttpClientFactory.Object)
            }, new IStubSource[] {_stubSource, _writableStubSourceMock.Object});
    }

    protected async Task SetScenario(string scenarioName, ScenarioStateInputDto scenario)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{BaseAddress}ph-api/scenarios/{scenarioName}"),
            Content = new StringContent(JsonConvert.SerializeObject(scenario), Encoding.UTF8, Constants.JsonMime)
        };
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
