using System;
using System.Net.Http;
using System.Threading.Tasks;
using FetchData.Sample.Shared.Models;
using Refit;

namespace FetchData.Sample.Shared.Services
{
    public interface IGitHubUserApi : IGitHub
    {
        [Get("/users/{username}")]
        Task<GitHubProfile> GetUser(string username);

        [Get("/users/{username}/repos")]
        Task<IEnumerable<GitHubRepository>> GetUserRepositories(string username);
    }
}