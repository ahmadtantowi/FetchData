using System;
using System.Threading.Tasks;
using FetchData.Example.Services;
using FetchData.Extensions;
using Microsoft.Extensions.Logging;

namespace FetchData.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            LoggerFactory
                .Create(builder => builder.AddConsole())
                .SetFetchDataLoggerFactory();

            Console.Write("Input GitHub username: ");
            var username = Console.ReadLine();

            Console.WriteLine($"Get GitHub profile with username {username}");
            Console.WriteLine("Executing Fetch Data...");

            var gitHubService = new ApiService<IGitHubService>(new ApiConfiguration("https://api.github.com"));
            var initiatedResult = await (await gitHubService.Initiated.GetUser(username)).Content.ReadAsStringAsync();
            var backgroundResult = await (await gitHubService.Background.GetUser(username)).Content.ReadAsStringAsync();
            var speculativeResult = await (await gitHubService.Speculative.GetUser(username)).Content.ReadAsStringAsync();
        }
    }
}
