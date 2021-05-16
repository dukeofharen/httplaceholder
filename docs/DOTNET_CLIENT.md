# HttPlaceholder REST API client for .NET

There is a NuGet package available for HttPlaceholder. You can find this client
here: https://www.nuget.org/packages/HttPlaceholder.Client/.

## General

This client was built from the ground up. It exposes methods for easily adding the HttPlaceholder client to
the `IServiceCollection` or, if you don't use the .NET Core dependency injection, a factory for easily creating new
client instances.

All methods exposed by the API are available in the client and you can build stub models by using a stub model builder.

## Getting started

You can see some examples in the following
project: https://github.com/dukeofharen/httplaceholder/tree/master/src/HttPlaceholder.Client.Examples.

### When using .NET Core service collection

Use the following code to add the client to your .NET Core application.

```c#
...
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttPlaceholderClient(config => 
    {
        config.RootUrl = "http://localhost:5000/"; // The HttPlaceholder root URL.
        config.Username = "username"; // The HttPlaceholder root URL.
        config.Password = "password";
    })
}
...
```

Now you can inject an object of type `IHttPlaceholderClient` in your class of choice and call the HttPlaceholder API.

### When not using .NET Core service collection

If you do not use .NET Core or the .NET Core dependency container, use this method to create a new client.

```c#
...
var config = new HttPlaceholderClientConfiguration
{
    RootUrl = "http://localhost:5000", // The HttPlaceholder root URL.
    Username = "username",// The HttPlaceholder root URL.
    Password = "password"
};
var client = HttPlaceholderClientFactory.CreateHttPlaceholderClient(config);
...
```

### Example: create a new stub

When you've initialized the client, you can call the HttPlaceholder API endpoints. Here is an example for how you add a
simple stub.

```c#
...
var createdStub = await client.CreateStubAsync(new StubDto
{
    Id = "test-stub-123",
    Conditions = new StubConditionsDto
    {
        Method = "GET",
        Url = new StubUrlConditionsDto
        {
            Path = "/test-path"
        }
    },
    Response = new StubResponseDto
    {
        StatusCode = 200,
        Json = @"{""key1"":""val1"", ""key2"":""val2""}"
    }
});
...
```

This method will create the stub and will also return the created stub. Because this way of adding stubs can get very
verbose very quick, another way of adding stubs with the client has been added: the StubBuilder. This is a fluent
builder which can also be used to create new stubs. Here is the same example, but now with using the StubBuilder:

```c#
...
var createdStub = await client.CreateStubAsync(StubBuilder.Begin()
    .WithId("test-stub-123")
    .WithConditions(StubConditionBuilder.Begin()
        .WithHttpMethod(HttpMethod.Get)
        .WithPath("/test-path"))
    .WithResponse(StubResponseBuilder.Begin()
        .WithHttpStatusCode(HttpStatusCode.Ok)
        .WithJsonBody(new {key1 = "val1", key2 = "val2"})));
...
```

This method is a bit shorter and is more readable.