using System;
using System.Net.Http;
using System.Threading.Tasks;
using FetchData.Example.Services;

namespace FetchData.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Input GitHub username: ");
            var username = Console.ReadLine();

            Console.WriteLine($"Get GitHub profile with username {username}");
            Console.WriteLine("Executing Fetch Data...");

            var gitHubService = new ApiService<IGitHubService>("https://api.github.com");
            var result = await gitHubService.Initiated().GetUser(username).ConfigureAwait(false);
            var response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            Console.WriteLine(response);
        }
    }
}
