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

            var gitHubService = new ApiService<IGitHubService>("https://api.github.com");
            _ = await gitHubService.Initiated().GetUser(username).ConfigureAwait(false);
        }
    }
}
