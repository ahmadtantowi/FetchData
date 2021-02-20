using System;
using System.IO;
using System.Threading.Tasks;
using FetchData.Example.Services;
using FetchData.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FetchData.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
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

            serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            Console.Write("Input GitHub username: ");
            var username = Console.ReadLine();

            Console.WriteLine($"Get GitHub profile with username {username}");
            Console.WriteLine("Executing Fetch Data...");

            var githubService = serviceProvider.GetService<IApiService<IGitHubApi>>();
            var initiatedResult = await (await githubService.Initiated.GetUser(username)).Content.ReadAsStringAsync();
            var backgroundResult = await (await githubService.Background.GetUser(username)).Content.ReadAsStringAsync();
            var speculativeResult = await (await githubService.Speculative.GetUser(username)).Content.ReadAsStringAsync();
        }
    }
}
