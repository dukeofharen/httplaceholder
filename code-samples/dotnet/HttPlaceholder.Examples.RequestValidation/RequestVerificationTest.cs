using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using HttPlaceholder.Client;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Client.StubBuilders;
using static HttPlaceholder.Client.Utilities.DtoExtensions;
using static HttPlaceholder.Client.Verification.Dto.TimesModel;

namespace HttPlaceholder.Examples.RequestValidation;

[TestClass]
public class RequestVerificationTest
{
    [TestMethod]
    public async Task TestRequestVerification()
    {
        await using var testcontainers = BuildTestContainer();
        await testcontainers.StartAsync();

        // Create a HttPlaceholder client instance.
        var client = HttPlaceholderClientFactory.CreateHttPlaceholderClient(new HttPlaceholderClientConfiguration
        {
            RootUrl = "http://localhost:1337"
        });

        // Create a stub.
        var stub = StubBuilder.Begin()
            .WithId("some-stub-id")
            .WithConditions(StubConditionBuilder.Begin()
                .WithHttpMethod(HttpMethod.Get)
                .WithPath("/someUrl")
                .WithQueryStringParameter("queryParam", StringEquals("someValue")))
            .WithResponse(StubResponseBuilder.Begin()
                .WithTextResponseBody("This is the response!"))
            .Build();
        await client.CreateStubAsync(stub);

        // Perform a request.
        var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync("http://localhost:1337/someUrl?queryParam=someValue");
        response.EnsureSuccessStatusCode();
        var contents = await response.Content.ReadAsStringAsync();

        // Assert response text.
        Assert.AreEqual("This is the response!", contents);

        // Now, let's verify through the API a request for the specific stub has been called.
        await client.VerifyStubCalledAsync("some-stub-id", ExactlyOnce(), DateTime.UtcNow.AddSeconds(-10));
    }

    [TestMethod]
    public async Task TestRequestVerification_VerificationFails()
    {
        
        await using var testcontainers = BuildTestContainer();
        await testcontainers.StartAsync();

        // Create a HttPlaceholder client instance.
        var client = HttPlaceholderClientFactory.CreateHttPlaceholderClient(
            new HttPlaceholderClientConfiguration
            {
                RootUrl = "http://localhost:1337"
            });

        // Create a stub.
        var stub = StubBuilder.Begin()
            .WithId("some-stub-id")
            .WithConditions(StubConditionBuilder.Begin()
                .WithHttpMethod(HttpMethod.Get)
                .WithPath("/someUrl")
                .WithQueryStringParameter("queryParam",
                    StringCheckingDtoBuilder.Begin().StringEquals("someValue")))
            .WithResponse(StubResponseBuilder.Begin()
                .WithTextResponseBody("This is the response!"))
            .Build();
        await client.CreateStubAsync(stub);

        // Perform a request.
        var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync("http://localhost:1337/someUrl?queryParam=wrongValue");

        // Assert status code.
        // Since we called the request incorrectly, it should return HTTP 501.
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);

        // Now, let's verify through the API a request for the specific stub has NOT been called.
        await client.VerifyStubCalledAsync("some-stub-id", Never(), DateTime.UtcNow.AddSeconds(-10));
    }

    private static TestcontainersContainer BuildTestContainer()
    {
        // Start a HttPlaceholder Docker container.
        // Make sure you can execute Docker on your system without sudo, or else the container can't start.
        return new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("dukeofharen/httplaceholder:2022.11.20.108")
            .WithName("httplaceholder")
            .WithPortBinding(1337, 5000)
            .WithExposedPort(5000)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5000))
            .Build();
    }
}