using HttPlaceholder.Client;
using HttPlaceholder.IntegrationTests.Clients;
using HttPlaceholder.IntegrationTests.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.IntegrationTests;

public abstract class IntegrationTestBase
{
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
            .AddJsonFile($"appsettings.{env}.json")
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
            .AddHttPlaceholderClient(c =>
            {
                c.RootUrl = settings.HttpUrl;
                // c.Username = "";
                // c.Password = "";
            })
            .AddClientsModule();
        Provider = services.BuildServiceProvider();
    }
}
