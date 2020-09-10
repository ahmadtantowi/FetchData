using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace FetchData.Example.Services
{
    [Headers("User-Agent: FetchData-Example")]
    public interface IGitHubService
    {
        [Get("/users/{username}")]
        Task<HttpResponseMessage> GetUser(string username);
    }
}