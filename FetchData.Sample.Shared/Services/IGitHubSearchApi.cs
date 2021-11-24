using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FetchData.Sample.Shared.Models;
using Refit;

namespace FetchData.Sample.Shared.Services
{
    public interface IGitHubSearchApi : IGitHub
    {
        [Get("/search/users")]
        Task<GitHubSearchUser> SearchUsers([AliasAs("q")] string query);
    }
}