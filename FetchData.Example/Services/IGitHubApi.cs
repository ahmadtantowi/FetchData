using System;
using System.Net.Http;
using System.Threading.Tasks;
using FetchData.Example.Models;
using Refit;

namespace FetchData.Example.Services
{
    [Headers("User-Agent: FetchData-Example")]
    public interface IGitHubApi : IGitHub
    {
        [Get("/users/{username}")]
        Task<GitHubProfile> GetUser(string username);
    }
}