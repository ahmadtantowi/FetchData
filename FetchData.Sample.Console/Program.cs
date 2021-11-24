using FetchData;
using FetchData.Extensions;
using FetchData.Sample.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// configure console logging
LoggerFactory
    .Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
    .SetFetchDataLoggerFactory();

// read appsetting.json file
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
var githubConf = builder.GetSection("GitHubService").Get<ApiConfiguration>();
var githubServiceConf = new ApiServiceConfiguration
{
    Configuration = githubConf,
    Modules = new[] { typeof(IGitHub) }
};

// setup dependency injection
var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddApiServices(githubServiceConf)
    .BuildServiceProvider();

Console.Write("Input GitHub username: ");
var username = Console.ReadLine();

Console.WriteLine($"Get GitHub profile with username {username}");
Console.WriteLine("Executing Fetch Data...");

var githubUserApi = serviceProvider.GetService<IApiService<IGitHubUserApi>>();
var resultUser = await githubUserApi.Initiated.GetUser(username).ConfigureAwait(false);
var resultRepos = await githubUserApi.Initiated.GetUserRepositories(username).ConfigureAwait(false);

// var githubSeachApi = serviceProvider.GetService<IApiService<IGitHubSearchApi>>();
// var resultSearch = await githubSeachApi.Initiated.SearchUsers("ahmad");

await Task.Delay(100);

Console.WriteLine();
Console.WriteLine($"Name: {resultUser.Name}");
Console.WriteLine($"Company: {resultUser.Company}");
Console.WriteLine($"Twitter: {resultUser.TwitterUsername}");
Console.WriteLine($"Blog: {resultUser.Blog}");
Console.WriteLine($"Public Repos: {string.Join(", ", resultRepos.Select(x => $"{x.Name} ({x.Language})"))}");

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
