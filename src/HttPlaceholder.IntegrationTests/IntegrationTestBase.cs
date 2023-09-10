using HttPlaceholder.Client;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.IntegrationTests.Clients;
using HttPlaceholder.IntegrationTests.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.IntegrationTests;

public abstract class IntegrationTestBase
{
    protected const string DistributionKeyHeaderKey = "X-HttPlaceholder-DistributionKey";
    private const string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";

    public IServiceProvider Provider { get; private set; }

    public HttPlaceholderSettings Settings => Provider.GetRequiredService<IOptions<HttPlaceholderSettings>>().Value;

    [TestInitialize]
    public void Initialize()
    {
        // Initialize config.
        var env = Environment.GetEnvironmentVariable(EnvironmentKey);
        if (string.IsNullOrWhiteSpace(env))
        {
            env = "Test";
        }

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Initialize DI.
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);

        var configSection = config.GetSection(nameof(HttPlaceholderSettings));
        services.Configure<HttPlaceholderSettings>(configSection);
        services.AddHttpClient();

        var settings = configSection.Get<HttPlaceholderSettings>();
        Assert.IsNotNull(settings);
        services
            .AddClientsModule();
        Provider = services.BuildServiceProvider();
        AfterInitialize();
    }

    public virtual void AfterInitialize()
    {
    }

    public IHttPlaceholderClient GetHttplClient(IDictionary<string, string> defaultRequestHeaders = null) =>
        HttPlaceholderClientFactory.CreateHttPlaceholderClient(new HttPlaceholderClientConfiguration
        {
            RootUrl = Settings.HttpUrl, DefaultHttpHeaders = defaultRequestHeaders
        }, true);

    public IHttPlaceholderClient GetHttplClient(string distributionKey) =>
        GetHttplClient(new Dictionary<string, string> {{DistributionKeyHeaderKey, distributionKey}});

    public TService GetService<TService>() => Provider.GetRequiredService<TService>();

    public string RootUrl => Settings.HttpUrl;
}
