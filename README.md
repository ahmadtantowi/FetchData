# Fetch Data
A helper to fetch API services using [Refit](https://github.com/reactiveui/refit) and [Fusillade](https://github.com/reactiveui/fusillade)

[![NuGet](https://img.shields.io/nuget/v/FetchData.svg?label=NuGet)](https://www.nuget.org/packages/FetchData)

## Configuration
Canfiguration can be done via `appsetting.json` file on inside code. You can configure host, timeout and serialize mode that represent `ApiConfiguration` object.

### Configure Mail Server
Object of `ApiConfiguration`
```json
"GitHubService": {
    "host": "https://api.github.com",
    "timeout": 360,
    "serializeMode": "SnakeCase"
}
```

### Configure Interface Modules
You need to define base interface for each module that api services provided, this will enable dependency injection registration on runtime and no need register each Refit interface. To provide it, you can create empty interface, then interit it to each Refit interface services.<br>
Interface module:
```csharp
public interface IGitHub {}
```
Refit interface that inherit interface module:
```csharp
[Headers("User-Agent: FetchData-Example")]
public interface IGitHubApi : IGitHub
{
    [Get("/users/{username}")]
    Task<GitHubProfile> GetUser(string username);
}
```

### Configure Dependency Injection Registration
First, you need `ApiConfiguration` instance before register services,
- From `appsettings.json`<br>
    ```csharp
    var githubConf = builder.GetSection("GitHubService").Get<ApiConfiguration>();
    ```
- From new instance<bR>
    ```csharp
    var githubConf = new ApiConfiguration
    {
        Host = "https://api.github.com",
        Timeout = 360,
        SerializeMode = SerializeNamingProperty.SnakeCase
    }
    ```
Then, create `ApiServiceConfiguration` instance with previous configuration
```csharp
var githubServiceConf = new ApiServiceConfiguration
{
    Configuration = githubConf,
    Modules = new[] { typeof(IGitHub) }
};
```
Register to dependency injection container in `Startup.cs`
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // configure api services here

    services.AddApiServices(githubServiceConf);
}
```

### Configure Logging
The default implementation of `DelegatingHandler` provided this library, will only log http request and response in debug level. To enable it, provide `ILoggerFactory` to this library.
```csharp
LoggerFactory
    .Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
    .SetFetchDataLoggerFactory();
```

### Customize DelegatingHandler
You can provide your own `DelegatingHandler` implementation to customize authentication header & other stuff, and let `FetchData` use it. Add your `DelegatingHandler` before register services to dependency injection.
```csharp
githubServiceConf.DelegatingHandler = typeof(YourOwnDelegatingHandler);
```

## Usage
`FetchData` also use Fusillade to drive `HttpClient`, you can read more about it in [here](https://github.com/reactiveui/Fusillade)
### Get `IApiService<T>` instance from DI container
```csharp
var githubService = serviceProvider.GetService<IApiService<IGitHubApi>>();
```
### User Initiated Request
```csharp
var result = await githubService.Initiated.GetUser(username).ConfigureAwait(false);
```
### Background Request
```csharp
var result = await githubService.Background.GetUser(username).ConfigureAwait(false);
```
### Speculative Request
```csharp
var result = await githubService.Speculative.GetUser(username).ConfigureAwait(false);
```