---
title: Request validation using HttPlaceholder + .NET
date: 2022-10-02T16:48:46+02:00
description: Request validation using HttPlaceholder + .NET
---

The code for this post can be found in <https://github.com/dukeofharen/httplaceholder/blob/master/code-samples/dotnet>.

In this small tutorial, we are going to take a look on how to verify a request made to a specific stub using the [HttPlaceholder .NET client](https://www.nuget.org/packages/HttPlaceholder.Client/). In general, the following steps are executed:

* Add a stub to HttPlaceholder.
* Make a request to HttPlaceholder so the stub matches.
* Verify if the stub you just made was hit.

The code can be found [here](https://github.com/dukeofharen/httplaceholder/blob/master/code-samples/dotnet) and uses [Testcontainers.NET](https://github.com/testcontainers/testcontainers-dotnet) to run a HttPlaceholder Docker container to run the tests against.

## Setting up the container

The following code makes sure to start a Docker container with HttPlaceholder and maps the application to port `1337.`

```c#
var container = new TestcontainersBuilder<TestcontainersContainer>()
    .WithImage("dukeofharen/httplaceholder:2022.9.3.59")
    .WithName("httplaceholder")
    .WithPortBinding(1337, 5000)
    .WithExposedPort(5000)
    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5000))
    .Build();
await container.StartAsync();
```

## Creating the client

After the container has started, we can create an HttPlaceholder client.

```c#
var client = HttPlaceholderClientFactory.CreateHttPlaceholderClient(
    new HttPlaceholderClientConfiguration
    {
        RootUrl = "http://localhost:1337"
    });
```

## Creating a stub

```c#
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
```

The code above creates a simple stub which returns a plain text response.

## Performing the request

Now that the stub has been added, we can actually make a request to it and verify the correct response was returned.

```c#
var httpClient = new HttpClient();
using var response = await httpClient.GetAsync("http://localhost:1337/someUrl?queryParam=someValue");
response.EnsureSuccessStatusCode();
var contents = await response.Content.ReadAsStringAsync();
Assert.AreEqual("This is the response!", contents);
```

## Verify the stub has been called in HttPlaceholder

By using the verification method in the HttPlaceholder client, you can verify whether a specific stub was called and with what parameters.

```c#
await client.VerifyStubCalledAsync("some-stub-id", TimesModel.ExactlyOnce(), DateTime.UtcNow.AddSeconds(-10));
```

The code above checks whether the stub with stub ID `some-stub-id` (the stub we have defined above) has been called "exactly once". Furthermore, the request should not have been done more than 10 seconds ago. If this is all correct, the method will continue without throwing an exception. If the verification fails however, an exception will be thrown containing the verification details.

There are more overloads for the verification method, which you can read more about [here](/docs/#stub-request-validation).