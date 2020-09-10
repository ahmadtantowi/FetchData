using System;

namespace FetchData
{
    public interface IApiService<T> where T : class
    {
        T Initiated(string baseEndpoint = null);
        T Background(string baseEndpoint = null);
        T Speculative(string baseEndpoint = null);
    }
}