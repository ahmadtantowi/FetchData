using System;
using System.Net.Http;
using System.Threading.Tasks;
using FetchData.Sample.Shared.Models;
using Refit;

namespace FetchData.Sample.Shared.Services
{
    [Headers("User-Agent: FetchData-Sample")]
    public interface IGitHubApi : IGitHub
    {
        [Get("/users/{username}")]
        Task<GitHubProfile> GetUser(string username);
    }
}